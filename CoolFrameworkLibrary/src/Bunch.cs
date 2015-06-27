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
    /// A flat collection that is OO.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Bunch<T> {
        private List<T> _Group;
        public Bunch() {
            _Group = new List<T>();
        }

        public void EachDo(Action<T> Do) {
            foreach (T item in _Group) {
                Do(item);
            }
        }

        public int Count { get { return _Group.Count; } }

        public void Add(T one) {
            _Group.Add(one);
        }

        public bool Has(T one,Block Do) {
            bool result = _Group.Contains(one);
            if (result) Do();
            return result;
        }

        public void Remove(T one) {
            _Group.Remove(one);
        }

        public void Consume(Action<T> Do) {
            foreach (T one in _Group) {
                Do(one);
            }
            _Group.Clear();
        }

        public delegate bool FilterStrategy(T one);

        public void AllMatching(FilterStrategy Filter , Action<T> Do) {
            foreach (T one in _Group) {
                if (Filter(one)) {
                    Do(one);
                }
            }
        }

        public void AllExceptMatching(FilterStrategy Filter, Action<T> Do) {
            AllMatching((T one) => { return !Filter(one); }, Do);
        }
    }

} //--namespace--
