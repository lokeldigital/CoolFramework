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
using Lokel.CoolFramework.Extensions;

namespace Lokel.CoolFramework {
    public class UniqueSet<T> {
        //private HashSet<T> _Collection;
        private Dictionary<T, T> _Collection;

        public UniqueSet() {
            //_Collection = new HashSet<T>();
            _Collection = new Dictionary<T, T>();
        }

        public void Add(T one) {
            _Collection[one] = one;
        }

        public bool Has(T one, Block Do = null) {
            bool result = _Collection.ContainsKey(one);
            if (result && Do != null) Do();
            return result;
        }

        public void Remove(T one) {
            _Collection.Remove(one);
        }

        public void Consume(Action<T> Do) {
            foreach (T one in _Collection.Keys) {
                Do(one);
            }
            _Collection.Clear();
        }

        public void Clear() {
            _Collection.Clear();
        }

        public void EachDo(Action<T> Do) {
            int i = 0;
            T item;
            T[] vals = new T[_Collection.Count];
            foreach(T v in _Collection.Values) {
                vals[i] = v;
                i++;
            }
            for (i = 0; i < vals.Length; i++) {
                item = vals[i];
                Do(item);
            }
        }

        public int Count { get { return _Collection.Count; } }

        public delegate bool FilterStrategy(T one);

        public void AllMatching(FilterStrategy Filter, Action<T> Do) {
            foreach (T one in _Collection.Keys) {
                if (Filter(one)) {
                    Do(one);
                }
            }
        }

        public void AllExceptMatching(FilterStrategy Filter, Action<T> Do) {
            AllMatching((T one) => { return !Filter(one); }, Do);
        }
    }
} //--namespace --
