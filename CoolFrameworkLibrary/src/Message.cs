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

    public class Message {
        public Type Origin { get; private set; }

        public IMessageSubscriber SenderInstance { get; private set; }
        public bool HasInstance { get { return SenderInstance != null; } }

        public Type TargetRole { get; private set; }
        public IMessageSubscriber TargetInstance { get; private set; }
        public bool HasTargetInstance { get { return TargetInstance != null; } }

        public object EventType { get; private set; }

        public object Payload { get; set; }
        public bool HasPayload { get { return Payload != null; } }

        internal IMessageSubscriber ActualSender {get; set;}

        private void AssertValid(Type _TargetRole) {
            if (!TargetRole.IsInterface) {
                throw new ArgumentException("_TargetRole should be a Role interface, not " + _TargetRole.Name);
            }
        }

        private void AssertValid(IMessageSubscriber _Instance, Type _Origin) {
            if ( _Instance != null && (_Instance.GetType() != _Origin && ! _Instance.GetType().IsSubclassOf(_Origin) )) {
                throw new ArgumentException("Sending instance expected to be of type " + _Origin.Name);
            }
        }

        private Message(Type _Origin, Type _TargetRole, object _EventType, IMessageSubscriber _Instance, object _Payload) {
            Origin = _Origin;
            SenderInstance = _Instance;
            EventType = _EventType;
            TargetRole = _TargetRole;
            Payload = _Payload;
            TargetInstance = null;

            AssertValid(_TargetRole);
            AssertValid(_Instance, _Origin);
        }

        public Message(Type _Origin, Type _TargetRole, object _EventType)
            : this(_Origin, _TargetRole, _EventType, null, null) {
        }

        public Message(IMessageSubscriber _OriginInstance, Type _TargetRole, object _EventType)
            : this(_OriginInstance.GetType(), _TargetRole, _EventType, _OriginInstance, null) {
        }

        public Message(Type _Origin, Type _TargetRole, object _EventType, object _Payload)
            : this(_Origin, _TargetRole, _EventType, null, _Payload) {
        }

        public Message(IMessageSubscriber _OriginInstance, Type _TargetRole, object _EventType, object _Payload)
            : this(_OriginInstance.GetType(), _TargetRole, _EventType, _OriginInstance, _Payload) {
        }

        /// <summary>
        /// Identifies sender both as type and as a specific instance.  Useful for an observed type
        /// like a model.
        /// </summary>
        /// <param name="_Origin">Sender type, for grouping.</param>
        /// <param name="_TargetRole">Role that member instances must implement to receive message.</param>
        /// <param name="_EventType">The event type or function number.</param>
        /// <param name="_Instance">The source instance (of type _Origin)</param>
        public Message(Type _Origin, Type _TargetRole, object _EventType, IMessageSubscriber _Instance)
            : this(_Origin, _TargetRole, _EventType, _Instance, null) { }

        /// <summary>
        /// Where the destination's instance is known, this can be used to short circuit the
        /// message send addressing scheme.
        /// It allows a call to go down a hierarchy: e.g. Controller -> Renderer -> Palette
        /// It allows a response to a received message.
        /// </summary>
        /// <param name="Target">The instance to exclusively receive this message.</param>
        public void SetTargetInstance(IMessageSubscriber Target) {
            TargetInstance = Target;
        }

        public void ReuseAs(object _EventType, object _Payload = null) {
            EventType = _EventType;
            Payload = _Payload;
        }

        public override string ToString() {
            string val = string.Format("MESSAGE - Origin {0}  EventType {1}  Target {2}\r\n"
                +"HasTargetInstance? {3} Has Payload ? {4}",
                Origin.Name, EventType, TargetRole.Name,
                HasTargetInstance, HasPayload
                );
            return val;
        }
    } // -- Message --

} // -- namespace --
