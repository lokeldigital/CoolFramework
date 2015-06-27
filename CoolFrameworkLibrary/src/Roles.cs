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
using System.Reflection;


namespace Lokel.CoolFramework {

    public interface R_Role { }

    public delegate void Block();

    public class RoleSet {
                // --- Database of Framework Roles ---
        private static List<Type> __RoleTypes = null;
        public static void Init() {
            //String txt = "All Roles: ";

            __RoleTypes = new List<Type>();
            Type[] types = typeof(SubscriberInfo).Assembly.GetTypes();

            foreach (Type t in types) {
                if (t.IsInterface) {
                    Type[] interfaces = t.GetInterfaces();
                    foreach (Type r in interfaces) {
                        if (r == typeof(R_Role)) {
                            __RoleTypes.Add(t);
                            //txt += "> " + t.Name;
                        }
                    }
                }
            }
        }

        public static bool RoleFilter(Type TypeTocheck, System.Object Criteria) {
            List<Type> AnyOf = Criteria as List<Type>;
            bool result = false;

            if (AnyOf == null) {
                throw new ArgumentException("Criteria object should be List<Type> not " + Criteria.GetType().Name);
            }
            foreach (Type filter in AnyOf) {
                if (filter == TypeTocheck) {
                    result = true;
                    break;
                }
            }
            return result;
        }


        private HashSet<Type> _Set;

        public RoleSet(Type _Class) {
            TypeFilter filter = new TypeFilter(RoleFilter);
            Type[] Roles = _Class.FindInterfaces(filter, (object)__RoleTypes);
            _Set = new HashSet<Type>();
            foreach (Type r in Roles) {
                _Set.Add(r);
            }
        }

        public bool DoIfContains(Type _role, Block Do) {
            bool result = _Set.Contains(_role);
            if (result) Do();
            return result;
        }

        public override string ToString() {
            string txt = "[Roles ";
            foreach (Type role in _Set) {
                txt += "|" + role.Name;
            }
            txt += "]";
            return txt;
        }
    } //-- RoleSet

} // -- namespace --
