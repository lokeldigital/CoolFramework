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
using System.Text;

namespace Lokel.CoolFramework {
    public class MessageSubscriber : IMessageSubscriber {
        private MessageBus _Bus;

        public MessageSubscriber() {
            _Bus = null;
        }

        public virtual void ReceiveUnhandled(Message msg) {
        }

        public void SetMessageBus(MessageBus Bus) {
            _Bus = Bus;
            Bus.Register(this);
        }

        public void DeregisterHandlers() {
            _Bus.Deregister(this);
        }

        protected void Send(Message msg) {
            MessageSubscriber.SendVia(msg, _Bus, this);
        }

        public static void SendVia(Message msg, MessageBus MsgBus, IMessageSubscriber Caller) {
            msg.ActualSender = Caller;
            MsgBus.Send(msg);
        }

    } //- MessageSubscriber

} //-- namespace --
