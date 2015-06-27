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
using System.Text;

using Lokel.CoolFramework;
using Lokel.CoolFramework.Extensions;

namespace Lokel.CoolFramework {

    public class SubscriberInfo {
        private UniqueSet<IMessageSubscriber> _ObservedSet;

        internal Type SubscriberType { get; private set; }
        internal RoleSet Roles { get; private set; }

        internal bool HasRole(Type role, Block Do) {
            return Roles.DoIfContains(role, Do);
        }

        internal IMessageSubscriber Instance { get; private set; }

        internal SubscriberInfo(IMessageSubscriber Subscriber) {
            SubscriberType = Subscriber.GetType();
            Instance = Subscriber;
            Roles = new RoleSet(SubscriberType);
            _ObservedSet = new UniqueSet<IMessageSubscriber>();
        }

        public void SetObservedObject(IMessageSubscriber ObservedObject) {
            _ObservedSet.Add(ObservedObject);
        }

        /// <summary>
        /// Removes the observed-object from the observer if and only if
        /// the given object was the observed object.
        /// </summary>
        /// <param name="ClearIfObserved">The object to cease observing</param>
        /// <returns>True if the given object was being observed by THIS object.</returns>
        public bool ClearObservedObject(IMessageSubscriber ClearIfObserved) {
            return _ObservedSet.Has( ClearIfObserved, () =>
            {
                _ObservedSet.Remove( ClearIfObserved );
            });
        }

        public bool PerformIfMessageFromObservedObject(Message msg, Block Do) {
            DoOnceBlock DoOnce = new DoOnceBlock(Do);
            bool result = false;
            msg.HasInstance.IfTrue(() =>
            {
                result = _ObservedSet.Has(msg.SenderInstance, DoOnce);
            });
            msg.HasInstance.IfTrue(() =>
            {
                result = result || _ObservedSet.Has(msg.ActualSender, DoOnce);
            });

            if (result) {
                Do();
            }
            return result;
        }

        public override string ToString() {
            string tmp = string.Format("SubscriberInfo type:{0}  roles:{1} instance:{2}",
                SubscriberType.Name, Roles, Instance.GetHashCode());
            return tmp;
        }
    }

    public delegate void MessageHandler(Message msg);

    internal class HandlerInfo {
        private MessageHandler HandlerDelegate { get; set; }
        internal Type Origin { get; private set; }

        internal object EventType { get; private set; }

        internal HandlerInfo(SubscribeToAttribute attr) {
            Origin = attr.Origin;
            EventType = attr.EventType;
        }

        internal void UpdateHandler(MessageHandler Handler) {
            if (HandlerDelegate != null) {
                MessageHandler mh = HandlerDelegate + Handler;
                HandlerDelegate = mh;
            } else {
                HandlerDelegate = Handler;
            }
        }

        internal void CallHandler(Message msg) {
            if (HandlerDelegate != null) {
                HandlerDelegate(msg);
            }
        }

        public override string ToString() {
            string txt = string.Format("HandlerInfo: delegate OK {0}  Origin Type: {1}  ID: {2}",
                HandlerDelegate != null ? "Yes" : "No",
                    Origin.Name,
                    EventType
                );
            txt += "\r\nHandler Details:\r\n"
                + "   Name: " + HandlerDelegate.Method.Name + "\r\n"
                + "   Type: " + HandlerDelegate.Method.DeclaringType
                ;
            return txt;
        }
    }

} //-- namespace --
