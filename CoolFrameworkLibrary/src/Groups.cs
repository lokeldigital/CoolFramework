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

    /// <summary>
    /// A collection of groups in one container with operations
    /// to perform across all group entities or by group.
    /// A Group is a list of entities defined by a group key.
    /// </summary>
    /// <typeparam name="G">Group Key</typeparam>
    /// <typeparam name="T">Member in Group</typeparam>
    public class Groups<G,T> {
        public class Members : List<T> { };

        private Dictionary<G, Groups<G,T>.Members> _Group;
        public Groups() {
            _Group = new Dictionary<G, Groups<G, T>.Members>();
        }

        public void CreateGroup(G group) {
            if (_Group.ContainsKey(group)) {
                return;
            } else {
                _Group.Add(group, new Groups<G, T>.Members());
            }
        }

        public int GroupSize(G group) {
            int num = 0;
            if (_Group.ContainsKey(group)) {
                num = _Group[group].Count;
            }
            return num;
        }

        public int GroupCount { get { return _Group.Count; } }

        public void DestroyGroup(G group) {
            if (_Group.ContainsKey(group)) {
                _Group.Remove(group);
            }
        }

        public void EachGroupDo(Action<G> _action) {
            foreach (G group_key in _Group.Keys) {
                _action(group_key);
            }
        }

        public delegate bool FilterAction(G group);

        public void EachGroupDo(FilterAction criteria, Action<G> _action) {
            List<G> groupset = new List<G>();
            foreach (G group_key in _Group.Keys) {
                if (criteria(group_key)) {
                    groupset.Add(group_key);
                }
            }

            foreach (G group in groupset) {
                _action(group);
            }
        }

        public void FirstMatchingGroupDo(FilterAction criteria, Action<G> _action) {
            foreach (G group_key in _Group.Keys) {
                if (criteria(group_key)) {
                    _action(group_key);
                    break;
                }
            }
        }

        public void EachElementDo(Action<T> _action) {
            foreach (List<T> elements in _Group.Values) {
                foreach (T one in elements) {
                    _action(one);
                }
            }
        }

        public void ForMembersOfGroup(G group, Action<T> Do) {
            if (_Group.ContainsKey(group) && Do != null) {
                Members items = new Members();
                items.AddRange(_Group[group]);
                foreach (T one in items) {
                    Do(one);
                }
            }
        }

        public void ToGroup(G group, Action<Groups<G, T>.Members, T> Do, T with) {
            if (_Group.ContainsKey(group) && Do != null) {
                Do(_Group[group], with);
            }
        }

        public void ToGroup(G group, Action<Groups<G, T>.Members> Do) {
            if (_Group.ContainsKey(group) && Do != null) {
                Do(_Group[group]);
            }
        }


        public void AllGroupsBut(G group, Action<T> Do) {
            foreach (G grp in _Group.Keys) {
                if (!grp.Equals(group) && grp.GetHashCode() != group.GetHashCode()) {
                    foreach (T one in _Group[grp]) {
                        Do(one);
                    }
                }
            }
        }

        public void AllElementsBut(T skip, Action<T> Do) {
            EachElementDo((T one) =>
            {
                if (one.GetHashCode() != skip.GetHashCode()) {
                    Do(one);
                }
            });
        }
    } // Groups

} //-- namespace --
