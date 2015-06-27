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

    [AttributeUsage(AttributeTargets.Method)]
    public class SubscribeToAttribute : Attribute {
        private Type _Origin;
        private object _EventType;
        private bool _InstanceSpecific;

        public Type Origin { get { return _Origin; } }
        public object EventType { get { return _EventType; } }

        public bool InstanceSpecific { get { return _InstanceSpecific; } }

        public SubscribeToAttribute(Type Origin, object EventType, bool InstanceSpecific) {
            _Origin = Origin;
            _EventType = EventType;
            _InstanceSpecific = InstanceSpecific;
        }

        public SubscribeToAttribute(Type Origin, object EventType)
            : this(Origin, EventType, false) { }

        public override string ToString() {
            string val = string.Format("SubscribeTo Origin: {0} EventType: {1} SenderInstance Spec? {2}",
                    Origin.Name,
                    EventType,
                    InstanceSpecific
                );
            return val;
        }
    }


} //-- namespace --
