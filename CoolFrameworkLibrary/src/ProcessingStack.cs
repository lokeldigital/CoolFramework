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
using System.Linq;
using System.Text;

namespace Lokel.CoolFramework {

    /// <summary>
    /// A stack that stores items and state information, generically,
    /// so that recursive routines can be written using an explicit
    /// stack rather than the VM stack.
    /// </summary>
    /// <typeparam name="T">Type of the objects to store.</typeparam>
    /// <typeparam name="EnumType">An enum type that tracks state.</typeparam>
    public class ProcessingStack<T, EnumType> {
        private struct StackNode {
            internal T _Item;
            internal EnumType _Step;
        }

        private StackNode[] _Stack;
        private int _StackPos;

        public const int DEFAULT_SIZE = 8;

        public ProcessingStack() {
            _Stack = new StackNode[ DEFAULT_SIZE ];
            _StackPos = 0;
        }

        public void Push(T Item, EnumType nextStep) {
            if (_StackPos == _Stack.Length) {
                Array.Resize(ref _Stack, _Stack.Length * 2);
            }
            _Stack[_StackPos] = new StackNode()
            {
                _Item = Item,
                _Step = nextStep
            };
            _StackPos++;
        }

        public EnumType Pop(ref T Item) {
            StackNode node = default(StackNode);
            EnumType status = default(EnumType);

            if (_StackPos > 0) {
                _StackPos--;
                node = _Stack[_StackPos];
                Item = node._Item;
                status = node._Step;
            }
            return status;
        }

        public void Pop() {
            if (_StackPos > 0) {
                _StackPos--;
            }
        }

        public EnumType Top(ref T Item) {
            StackNode node = default(StackNode);
            EnumType status = default(EnumType);

            if (_StackPos > 0) {
                node = _Stack[_StackPos - 1];
                Item = node._Item;
                status = node._Step;
            }
            return status;
        }

        /// <summary>
        /// Returns an indication of the current stack depth (0 means empty).
        /// </summary>
        public int StackDepth { get { return _StackPos; } }

        /// <summary>
        /// Indicates if stack were empty or not.
        /// </summary>
        public bool Empty { get { return _StackPos == 0; } }

        public EnumType StackTop_Step {
            get {
                if (_StackPos > 0) {
                    return _Stack[_StackPos - 1]._Step;
                } else {
                    return default(EnumType);
                }
            }
            set {
                if (_StackPos > 0) {
                    _Stack[_StackPos - 1]._Step = value;
                }
            }
        }

        public T StackTop_Item {
            get {
                if (_StackPos > 0) {
                    return _Stack[_StackPos - 1]._Item;
                } else {
                    return default(T);
                }
            }
        }

    } //-ProcessingStack

} //- namespace

