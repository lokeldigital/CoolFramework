/*
 *  CoolFramework
 *  -------------
 *  A C#.NET library for Object Oriented containers and publish-subscribe
 *  inter-component middleware.  This has been built to allow a more abstract
 *  level of composability based on role that has a more loosely defined
 *  contract than interfaces or inheritance provide.
 *  
 *  Copyright (C) 2014, 2015 Lokel Digital Pty Ltd
 *  
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 *  USA
 */


using System;
using System.Collections.Generic;
using System.Reflection;

using Lokel.CoolFramework.Extensions;

namespace Lokel.CoolFramework {

    public interface IMessageSubscriber {
        void ReceiveUnhandled(Message msg);

        void SetMessageBus(MessageBus Bus);

        void DeregisterHandlers();
    }

    class MessageDelivery {
        private HandlerInfo _Handler;
        private Message _Message;

        internal MessageDelivery(HandlerInfo Handler, Message Msg) {
            _Handler = Handler;
            _Message = Msg;
        }

        internal HandlerInfo Handler { get { return _Handler; } }
        internal Message Msg { get { return _Message; } }
    }

    /// <summary>
    /// There should be one MessageBus instance per framework.
    /// It is the conduit for all object interactions.  An instance defines
    /// an object address space.
    /// </summary>
    public class MessageBus {
        private static bool __Initialised = false;

        private Groups<SubscriberInfo, HandlerInfo> _MessageHandlers;
        private Logger _Logger;
        private UniqueSet<MessageDelivery> _PendingDelivery, _ActiveDelivery; // for swapping an acti

        public MessageBus() {
            lock (typeof(MessageBus)) {
                if (!__Initialised){
                    RoleSet.Init();
                    __Initialised = true;
                }
            }
            _MessageHandlers = new Groups<SubscriberInfo, HandlerInfo>();
            _PendingDelivery = new UniqueSet<MessageDelivery>();
            _ActiveDelivery = new UniqueSet<MessageDelivery>();
            _Logger = new Logger();
        }

        public void Register(IMessageSubscriber Subscriber) {
            SubscriberInfo subInfo = GetInfoFor(Subscriber);

            if (subInfo == null) {
                subInfo = new SubscriberInfo(Subscriber);
            } else {
                _Logger.LogAt(LogLevel.LL_DebugFramework,
                    "Already had a subscriber for same instance: "+ subInfo.ToString()
                    );
                return;
            }

            _Logger.LogAt(LogLevel.LL_Info, "Creating Group: " + subInfo.SubscriberType.Name);
            _MessageHandlers.CreateGroup(subInfo);

            _Logger.LogAt(LogLevel.LL_DebugFramework, "START --Adding Handlers--");
            foreach (HandlerInfo info in FindHandlers(subInfo)) {
                _Logger.LogAt(LogLevel.LL_DebugFramework, "Handler: "+info.ToString()
                    );
                _MessageHandlers.ToGroup(
                    subInfo,
                    (Groups<SubscriberInfo, HandlerInfo>.Members handlers) => { handlers.Add(info); }
                );
            }
            _Logger.LogAt(LogLevel.LL_DebugFramework, "END --Adding Handlers--");
        }

        public void MakeObserver(IMessageSubscriber Observer, IMessageSubscriber ObservedObject) {
            SubscriberInfo Info = GetInfoFor(Observer);
            Info.SetObservedObject( ObservedObject );
        }

        public void UndoObserver(IMessageSubscriber Observer, IMessageSubscriber ObservedObject) {
            SubscriberInfo Info = GetInfoFor(Observer);
            Info.ClearObservedObject(ObservedObject);
        }

        public void Deregister(IMessageSubscriber Subscriber) {
            SubscriberInfo info = GetInfoFor(Subscriber);
            _MessageHandlers.DestroyGroup(info);
        }

        public void Deliver() {
            UniqueSet<MessageDelivery> tmp = _PendingDelivery;
            _ActiveDelivery.Clear();
            _PendingDelivery = _ActiveDelivery;
            tmp.EachDo((MessageDelivery msgDel) =>
            {
                msgDel.Handler.CallHandler(msgDel.Msg);
            });
            _ActiveDelivery = tmp;
        }

        public void Send(Message message) {
            Action<HandlerInfo, Message> Msg_PtToPt = (HandlerInfo handler, Message msg) =>
            {
                (handler.EventType.Equals(msg.EventType)).IfTrue(() =>
                {
                    _PendingDelivery.Add(new MessageDelivery(handler, msg));
                });
            };
            Action<HandlerInfo, Message> Msg_Observer = (HandlerInfo handler, Message msg) =>
            {
                _PendingDelivery.Add(new MessageDelivery(handler, msg));
            };
            Action<HandlerInfo, Message> Msg_RoleEvent = (HandlerInfo handler, Message msg) =>
            {
                _PendingDelivery.Add(new MessageDelivery(handler, msg));
            };

            Action UponNoRecipients = () =>
            {
                message.HasInstance.IfTrue(() =>
                {
                    _Logger.LogAt(LogLevel.LL_DebugFramework, "Unhandled message:");
                    message.SenderInstance.ReceiveUnhandled(message);
                }).IfFalse(() =>
                {
                    //(msg.ActualSender != null).IfTrue(() =>{
                    message.ActualSender.ReceiveUnhandled(message);
                    //});
                    _Logger.LogAt(LogLevel.LL_DebugFramework, "Could not match handlers for message [" + message + "]");
                });
            };

            MatchHandlers(message, Msg_PtToPt, Msg_Observer, Msg_RoleEvent, UponNoRecipients);
        }

        internal void MatchHandlers(
            Message msg,
            Action<HandlerInfo, Message> Blk_PointToPointMatch,
            Action<HandlerInfo, Message> Blk_RoleObserserMatch,
            Action<HandlerInfo, Message> Blk_RoleEventMatch,
            Action Blk_Unhandled
        ) {
            Action<SubscriberInfo, HandlerInfo, string> do_nothing = (SubscriberInfo sub, HandlerInfo h, string s) => { };

            MatchHandlers(msg,
                Blk_PointToPointMatch: Blk_PointToPointMatch,   Almost_PointToPointMatch: do_nothing,
                Blk_RoleObserserMatch: Blk_RoleObserserMatch,   Almost_RoleObserverMatch: do_nothing,
                Blk_RoleEventMatch: Blk_RoleEventMatch,         Almost_RoleEventMatch: do_nothing,
                Blk_Unhandled: Blk_Unhandled
            );
        }

        internal void MatchHandlers(
            Message msg,
            Action<HandlerInfo, Message>            Blk_PointToPointMatch,
            Action<SubscriberInfo, HandlerInfo, string> Almost_PointToPointMatch,
            Action<HandlerInfo, Message>            Blk_RoleObserserMatch,
            Action<SubscriberInfo, HandlerInfo, string> Almost_RoleObserverMatch,
            Action<HandlerInfo, Message>            Blk_RoleEventMatch,
            Action<SubscriberInfo, HandlerInfo, string> Almost_RoleEventMatch,
            Action Blk_Unhandled
        ) {
            int num_recipients;
            Groups<SubscriberInfo, HandlerInfo>.FilterAction RoleMatcher = (SubscriberInfo sub) =>
            {
                bool result = false;
                sub.Roles.DoIfContains(msg.TargetRole, () =>
                {
                    result = true;
                });

                return result;
            };
            Groups<SubscriberInfo, HandlerInfo>.FilterAction InstanceMatcher = (SubscriberInfo sub) =>
            {
                bool result = false;
                (msg.HasTargetInstance && sub.Instance == msg.TargetInstance).IfTrue(() =>
                {
                    result = true;
                });

                return result;
            };

            num_recipients = 0;
            _MessageHandlers.FirstMatchingGroupDo(InstanceMatcher, (SubscriberInfo group) =>
            {
                // This block supports direct, point-to-point sending to a known instance.
                _MessageHandlers.ForMembersOfGroup(group, (HandlerInfo handler) =>
                {
                    (handler.EventType.Equals(msg.EventType)).IfTrue(() =>
                    {
                        Blk_PointToPointMatch(handler, msg);
                        num_recipients++;
                    }).IfFalse(() =>
                    {
                        string reason = string.Format("Wrong event code - expected {0} not {1} ",
                            msg.EventType, handler.EventType);
                        Almost_PointToPointMatch(group, handler, reason);
                    });
                });
            });
            if (num_recipients > 0) {
                // Return immediately as there are no more groups to consider.
                return;
            }

            _MessageHandlers.EachGroupDo(RoleMatcher, (SubscriberInfo group) =>
            {
                // This block allows a subscriber to observe a message sender...
                group.PerformIfMessageFromObservedObject(msg, () =>
                {
                    _MessageHandlers.ForMembersOfGroup(group, (HandlerInfo handler) =>
                    {
                        (handler.EventType.Equals(msg.EventType)).IfTrue(() =>
                        {
                            Blk_RoleObserserMatch(handler, msg);
                            num_recipients++;
                        }).IfFalse(() =>
                        {
                            string reason = string.Format("Wrong event code - expected {0} not {1} ",
                                msg.EventType, handler.EventType);
                            Almost_RoleObserverMatch(group, handler, reason);
                        });
                    });
                });
                _MessageHandlers.ForMembersOfGroup(group, (HandlerInfo handler) =>
                {
                    // This block supports the 'normal' selection by role.
                    (handler.Origin == msg.Origin).IfTrue(() =>
                    {
                        (handler.EventType.Equals(msg.EventType)).IfTrue(() =>
                        {
                            bool contained = false;
                            group.Roles.DoIfContains(msg.TargetRole, () =>
                            {
                                contained = true;
                                num_recipients++;
                                Blk_RoleEventMatch(handler, msg);
                            });
                            contained.IfFalse(() =>
                            {
                                string reason = string.Format(
                                    "SHOULD NEVER HAPPEN!\r\n"
                                    + "Matched subscriber, origin and event type but not target role. "
                                    + "Message targeted role {0} but subscriber {1} was not in that role",
                                    msg.TargetRole, group.SubscriberType.Name
                                );
                                Almost_RoleEventMatch(group, handler, reason);
                            });
                        }).IfFalse(() =>
                        {
                            string reason = string.Format(
                                "Matched Role, Origin type but event type is not matching.  "
                                + "Wanted {0} but handler was for {1}.",
                                msg.EventType, handler.EventType
                            );
                            Almost_RoleEventMatch(group, handler, reason);
                        });
                    }).IfFalse(() =>
                    {
                        string reason = string.Format(
                            "Matched Role but not origin.  "
                            + "Wanted {0} but handler was for {1}.",
                            msg.Origin, handler.Origin
                        );
                        Almost_RoleEventMatch(group, handler, reason);
                    });
                });
            });

            (num_recipients == 0).IfTrue( Blk_Unhandled );
        }


        public void LogSubscribers() {
            string tmp;
            _MessageHandlers.EachGroupDo((SubscriberInfo group) =>
            {
                tmp = "Subscriber - " + group.ToString() + "\r\n";
                _MessageHandlers.ForMembersOfGroup(group, (HandlerInfo handler) =>
                {
                    tmp += "Message Handler: " + handler.ToString() + "\r\n";
                });
                _Logger.LogAt(LogLevel.LL_Warning, tmp);
            });
        }

        public void SetLogger(LoggerTask lt) {
            _Logger.AddTask(lt);
        }

        public void SetLogLevel(LogLevel levelFlag) {
            _Logger.Level = levelFlag;
        }

        // ----- PRIVATE -------

        private SubscriberInfo GetInfoFor(IMessageSubscriber Subscriber) {
            SubscriberInfo result = null;
            // Could have Group of Groups - type hierarchy above subscriber...
            //_MessageHandlers.ForMembersOfGroup(Subscriber.GetType(), (SubscriberInfo info) =>
            //{
            //});
            _MessageHandlers.EachGroupDo((SubscriberInfo info) =>
            {
                if (info.Instance == Subscriber) result = info;
            });
            return result;
        }

        private IEnumerable<HandlerInfo> FindHandlers(SubscriberInfo SubInfo) {
            HandlerInfo handlerInfo = null;
            MethodInfo[] methods = SubInfo.SubscriberType.GetMethods(
                BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.FlattenHierarchy
                );

            //_Logger.LogAt(LogLevel.LL_DebugFramework, "FindHandlers - "+methods.Length + " methods to process.");

            foreach (MethodInfo mthd in methods) {
                //_Logger.LogAt(LogLevel.LL_DebugFramework, "Method: " + mthd.Name);
                object[] attribs = mthd.GetCustomAttributes(typeof(SubscribeToAttribute), false);
                if (attribs != null && attribs.Length > 0) {
                    //_Logger.LogAt(LogLevel.LL_DebugFramework, "Found" + attribs.Length + " SubscribeTo attributes.");
                    foreach (object attr in attribs) {
                        SubscribeToAttribute subTo = attr as SubscribeToAttribute;
                        
                        if (subTo != null) {
                            _Logger.LogAt(LogLevel.LL_DebugFramework, subTo.ToString());
                            Delegate del = Delegate.CreateDelegate(typeof(MessageHandler), SubInfo.Instance, mthd, false);
                            MessageHandler handler = (MessageHandler)del;
                            if (handler != null) {
                                handlerInfo = new HandlerInfo(subTo);
                                handlerInfo.UpdateHandler(handler);
                                yield return handlerInfo;
                            }
                        } else {
                            _Logger.LogAt(LogLevel.LL_DebugFramework, "SubscribeTo was NULL");
                        }
                    }
                }
            }
            yield break;
        }
    } //-- MessageBus --

} //-- namespace --
