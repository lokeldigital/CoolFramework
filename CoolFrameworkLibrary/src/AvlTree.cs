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

using Lokel.CoolFramework.Extensions;

namespace Lokel.CoolFramework {

    public class AvlTree<T> {

        internal class Node {
            internal T _Value;
            internal Node _Left, _Right, _Parent;
            internal int _Depth;

            private Node(T val, Node Left, Node Right, Node Parent) {
                _Value = val;
                _Left = Left;
                _Right = Right;
                _Parent = Parent;
                //_DepthDifference = 0;
                _Depth = 0;
            }

            public Node(T val)
                : this(val, null, null, null) { }

            public Node() {
                _Value = default(T);
                _Left = _Right = null;
                _Parent = null;
                //_DepthDifference = 0;
            }
        } //-Node

        private Node _Root;
        private Func<T, T, bool> _IsSmaller;

        /// <summary>
        /// Create a tree instance and provide the comparison method
        /// for determining which was larger - for sorting purposes.
        /// </summary>
        /// <param name="Comparator">Delegate to method returning bool (A < B) </param>
        public AvlTree(Func<T, T, bool> Comparator) {
            _Root = null;
            _IsSmaller = Comparator;
        }

        enum Step {
            VISIT_LEFT,
            RETURN_VAL,
            VISIT_RIGHT,
            POP_CENTRE
        }

        

        public void Add(T val) {
            if (_Root == null) {
                _Root = new Node(val);
            } else {
                // Insert into correct spot.
                Node current = _Root;
                Func<T, bool> Check;
                Func<Node, bool> Do;

                Check = (v) => { return (_IsSmaller(val,v)); };
                Do = (node) =>
                {
                    if (! (node._Left != null && node._Right != null) ) {
                        if (node._Left == null && _IsSmaller(val, node._Value)) {
                            node._Left = new Node(val);
                            node._Left._Parent = node;
                            current = node;
                        } else if (node._Right == null && _IsSmaller(node._Value, val)) {
                            node._Right = new Node(val);
                            node._Right._Parent = node;
                            current = node;
                        }
                        
                        return false;
                    }
                    return true;
                };
                InOrder(_Root, Do, Check);
                //Depth(current);
                UpdateDepth(_Root);
                
                Do = (node) =>
                {
                    bool isRoot = node == _Root;
                    bool changed = Rebalance(ref node);
                    if (isRoot && changed) {
                        _Root = node;
                        UpdateDepth(node);
                    }
                    return ! changed;
                };
                Check = (v) => { return true; };
                PostOrder(_Root, Do, Check);
                //UpdateDepth(_Root);

                //Rebalance(ref current);
            }
        }

        //private void UpdateDepthDifference(Node subtree) {
        //    Func<T, bool> Check = (v) => { return true; };
        //    Func<Node, bool> DoEach = (node) =>
        //    {
        //        int left, right;
        //        if (node._Left != null) {
        //            left = node._Left._DepthDifference + 1;
        //        } else {
        //            left = 0;
        //        }
        //        if (node._Right != null) {
        //            right = node._Right._DepthDifference - 1;
        //        } else {
        //            right = 0;
        //        }
        //        node._DepthDifference = left - right;
        //        return true;
        //    };
        //    PostOrder(subtree, DoEach, Check);
        //}

        private bool Rebalance(ref Node node) {
            //Node sub = node;
            int left;
            int right;
            int diff;
            bool changed = false;

            left = node._Left != null ? (1 + node._Left._Depth) : 0;
            right = node._Right != null ? (1 + node._Right._Depth) : 0;
            diff = left - right;

            switch (diff) {
                case 0: break;
                case 1: break;
                case -1: break;
                case 2:
                    if (node._Left != null && node._Left._Left != null) {
                        Console.WriteLine("Rotate Right");
                        RotateRight(ref node);
                    } else if(node._Left != null && node._Left._Right != null){
                        Console.WriteLine("Double Rotate Right");
                        RotateDoubleRight(ref node);
                    }
                    changed = true;
                    break;
                case -2:
                    if (node._Right != null && node._Right._Right != null) {
                        Console.WriteLine("Rotate Left");
                        RotateLeft(ref node);
                    } else if (node._Right != null && node._Right._Left != null) {
                        Console.WriteLine("Double Rotate Left");
                        RotateDoubleLeft(ref node);
                    }
                    changed = true;
                    break;
                default:
                    break;
            }
            return changed;
        }

        

        // Take the given node as a sub-root
        private void RotateLeft(ref Node node) {
            Node T1, A, B, T2, T3, Parent;
            if (node != null && node._Right != null) {
                Parent = node._Parent;
                A = node;
                T1 = A._Left;
                B = A._Right;
                T2 = B._Left;
                T3 = B._Right;

                A._Right = T2; if (T2 != null) T2._Parent = A;
                B._Left = A; B._Parent = A._Parent; A._Parent = B;
                B._Parent = Parent;
                (Parent != null && Parent._Left == A).IfTrue(() => { Parent._Left = B; });
                (Parent != null && Parent._Right == A).IfTrue(() => { Parent._Right = B; });
                node = B;
            }
        }

        private void RotateRight(ref Node node) {
            Node T1, A, T2, B, T3, Parent;

            B = node;
            Parent = B._Parent;
            A = B._Left;
            T1 = A._Left;
            T2 = A._Right;
            T3 = B._Right;

            A._Right = B; A._Parent = Parent; B._Parent = A;
            (Parent != null && Parent._Left == B).IfTrue(() => { Parent._Left = A; });
            (Parent != null && Parent._Right == B).IfTrue(() => { Parent._Right = A; });
            node = A;

            B._Left = T2; if (T2 != null) T2._Parent = B;
        }

        private void RotateDoubleRight(ref Node node) {
            Node C, A, T1, B, T2, T3, T4, Parent;
            if (node != null && node._Left != null && node._Left._Right != null) {
                Parent = node._Parent;
                C = node;
                T4 = C._Right;
                A = C._Left;
                T1 = A._Left;
                B = A._Right;
                T2 = B._Left;
                T3 = B._Right;

                A._Right = T2; if (T2 != null) T2._Parent = A;
                C._Left = T3; if (T3 != null) T3._Parent = C;
                B._Left = A; A._Parent = B;
                B._Parent = Parent;
                (Parent != null && Parent._Left == C).IfTrue(() => { Parent._Left = B; });
                (Parent != null && Parent._Right == C).IfTrue(() => { Parent._Right = B; });
                B._Right = C; C._Parent = B;
                node = B;
            }
        }

        private void RotateDoubleLeft(ref Node node) {
            Node A, B, C, T1, T2, T3, T4, Parent;
            if (node != null && node._Right != null && node._Right._Left != null) {
                Parent = node._Parent;
                A = node;
                C = A._Right;
                B = C._Left;
                T1 = A._Left;
                T2 = B._Left;
                T3 = B._Right;
                T4 = C._Right;

                A._Right = T2; if (T2 != null) T2._Parent = A;
                C._Left = T3; if (T3 != null) T3._Parent = C;
                B._Left = A; A._Parent = B;
                B._Right = C; C._Parent = B;
                B._Parent = Parent;
                (Parent != null && Parent._Left == C).IfTrue(() => { Parent._Left = B; });
                (Parent != null && Parent._Right == C).IfTrue(() => { Parent._Right = C; });
                node = B;
            }
        }

        public void InOrder(Action<T> Do) {
            Func<Node,bool> DoNode = (node) =>
            {
                Do(node._Value);
                return true;
            };
            InOrder(_Root, DoNode, null);
        }

        private void InOrder(Node SubRoot, Func<Node, bool> Do, Func<T,bool> Check) {
            Node leaf = SubRoot;
            bool ContinueOn = true;
            ProcessingStack<Node, Step> Stack = new ProcessingStack<Node, Step>();

            Stack.Push(leaf, Step.VISIT_LEFT);
            while (! Stack.Empty && ContinueOn) {
                leaf = Stack.StackTop_Item;
                switch (Stack.StackTop_Step) {
                    case Step.VISIT_LEFT:
                        Stack.StackTop_Step = Step.RETURN_VAL;
                        if (leaf._Left != null) {
                            if (Check == null) {
                                Stack.Push(leaf._Left, Step.VISIT_LEFT);
                            } else if (Check(leaf._Left._Value)) {
                                Stack.Push(leaf._Left, Step.VISIT_LEFT);
                            }
                        }
                        break;
                    case Step.RETURN_VAL:
                        Stack.StackTop_Step = Step.VISIT_RIGHT;
                        ContinueOn = Do( Stack.StackTop_Item );
                        break;
                    case Step.VISIT_RIGHT:
                        Stack.StackTop_Step = Step.POP_CENTRE;
                        if (leaf._Right != null ){
                            if (Check == null) {
                                Stack.Push(leaf._Right, Step.VISIT_LEFT);
                            } else if (! Check(leaf._Right._Value)) {
                                Stack.Push(leaf._Right, Step.VISIT_LEFT);
                            }
                        }
                        break;
                    case Step.POP_CENTRE:
                        Stack.Pop();
                        break;
                }
            }
        }

        public void ReverseOrder(Action<T> Do) {
            Func<T, bool> Check = (v) => { return true; };
            Func<Node, bool> DoNode = (node) => { Do(node._Value); return true; };
            ReverseOrder(_Root, DoNode, Check);
        }

        private void ReverseOrder(Node SubRoot, Func<Node, bool> Do, Func<T,bool> Check) {
            Node leaf = SubRoot;
            bool ContinueOn = true;
            ProcessingStack<Node, Step> Stack = new ProcessingStack<Node, Step>();

            Stack.Push(leaf, Step.VISIT_RIGHT);
            while (! Stack.Empty && ContinueOn) {
                leaf = Stack.StackTop_Item;
                switch (Stack.StackTop_Step) {
                    case Step.VISIT_LEFT:
                        Stack.StackTop_Step = Step.POP_CENTRE;
                        if (leaf._Left != null && Check( leaf._Left._Value) ) {
                            Stack.Push(leaf._Left, Step.VISIT_LEFT);
                        }
                        break;
                    case Step.RETURN_VAL:
                        Stack.StackTop_Step = Step.VISIT_LEFT;
                        ContinueOn = Do(Stack.StackTop_Item);
                        break;
                    case Step.VISIT_RIGHT:
                        Stack.StackTop_Step = Step.RETURN_VAL;
                        if (leaf._Right != null && Check(leaf._Right._Value)) {
                            Stack.Push(leaf._Right, Step.VISIT_RIGHT);
                        }
                        break;
                    case Step.POP_CENTRE:
                        Stack.Pop();
                        break;
                }
            }
        }

        public void PostOrder(Action<T> Do) {
            Func<T, bool> Check = (v) => { return true; };
            Func<Node, bool> DoNode = (node) => { Do(node._Value); return true; };
            PostOrder(_Root, DoNode, Check);
        }

        private void PostOrder(Node SubRoot, Func<Node, bool> Do, Func<T, bool> Check) {
            Node leaf = SubRoot;
            bool ContinueOn = true;
            ProcessingStack<Node, Step> Stack = new ProcessingStack<Node, Step>();

            Stack.Push(leaf, Step.VISIT_LEFT);
            while (!Stack.Empty && ContinueOn) {
                leaf = Stack.StackTop_Item;
                switch (Stack.StackTop_Step) {
                    case Step.VISIT_LEFT:
                        Stack.StackTop_Step = Step.VISIT_RIGHT;
                        if (leaf._Left != null && Check(leaf._Left._Value)) {
                            Stack.Push(leaf._Left, Step.VISIT_LEFT);
                        }
                        break;
                    case Step.VISIT_RIGHT:
                        Stack.StackTop_Step = Step.RETURN_VAL;
                        if (leaf._Right != null && Check(leaf._Right._Value)) {
                            Stack.Push(leaf._Right, Step.VISIT_RIGHT);
                        }
                        break;
                    case Step.RETURN_VAL:
                        Stack.StackTop_Step = Step.POP_CENTRE;
                        ContinueOn = Do(Stack.StackTop_Item);
                        break;
                    case Step.POP_CENTRE:
                        Stack.Pop();
                        break;
                }
            }
        }

        private int Depth(Node node) {
            int count = 0;
            Node current = node;
            while (current != null && current != _Root) {
                count++;
                //current._Depth = count;
                current = current._Parent;
            }
            return count;
        }

        private void UpdateDepth(Node subtree) {
            Func<T, bool> check = (v) => { return true; };
            Func<Node, bool> Do = (node) =>
            {
                if (node._Left == null && node._Right == null) {
                    node._Depth = 0;
                    return true;
                }
                int d_left = node._Left != null ? node._Left._Depth : 0;
                int d_right = node._Right != null ? node._Right._Depth : 0;
                if (d_left > d_right) {
                    node._Depth = 1 + d_left;
                } else {
                    node._Depth = 1 + d_right;
                }
                return true;
            };
            PostOrder(subtree, Do, check);
        }

        public class NodeCheck {
            public int ParentId { get; private set; }
            public int ThisId { get; private set; }
            public int LeftId { get; private set; }
            public int RightId { get; private set; }
            public T Value { get; private set; }

            public Func<NodeCheck,bool> Verify {get; set;}
            public NodeCheck NextCheck { get; set; }

            public NodeCheck() {
                ParentId = 0;
                ThisId = 0;
                LeftId = 0;
                RightId = 0;
                Value = default(T);
                NextCheck = null;
            }

            internal void Update(Node node) {
                ParentId = node._Parent != null ? node._Parent.GetHashCode() : 0;
                ThisId = node.GetHashCode();
                LeftId = node._Left != null ? node._Left.GetHashCode() : 0;
                RightId = node._Right != null ? node._Right.GetHashCode() : 0;
                Value = node._Value;
            }

            public override string ToString() {
                string text = string.Format(
                    "NodeCheck: ID: {0:X} Value: {1}\r\n"
                    + "Parent ID: {2:X}  Left ID: {3:X}  Right ID: {4:X}",
                    ThisId, Value,
                    ParentId, LeftId, RightId
                    );
                return text;
            }
        }

        public void HierarchyCheck(NodeCheck Validator) {
            NodeCheck _Validator = Validator;
            Func<T, bool> check = (val) => { return true; };
            Func<Node, bool> Do = (node) =>
            {
                _Validator.Update(node);
                _Validator = _Validator.Verify(_Validator) ? _Validator.NextCheck : null;
                return _Validator != null;
            };
            PreOrder(_Root, Do, check);
        }

        public void Dump() {
            Func<T, bool> check = (val) => { return true; };
            Func<Node, bool> Do = (node) =>
            {
                Console.WriteLine(
                    string.Format("{0:X} [Left {1:X} Right {2:X} ] # {3}  depth: {4}",
                    node.GetHashCode(),
                    node._Left != null ? node._Left.GetHashCode() : 0,
                    node._Right != null ? node._Right.GetHashCode() : 0,
                    node._Value,
                    //Depth(node),
                    node._Depth
                    )
                );
                return true;
            };
            PreOrder(_Root, Do, check);
        }

        private void PreOrder(Node SubRoot, Func<Node, bool> Do, Func<T, bool> Check) {
            bool more = Do(SubRoot);
            if (more) {
                if (SubRoot._Left != null && Check(SubRoot._Left._Value) ) {
                    PreOrder(SubRoot._Left, Do, Check);
                }
                if (SubRoot._Right != null && Check(SubRoot._Right._Value)) {
                    PreOrder(SubRoot._Right, Do, Check);
                }
            }
        }
    }//-AvlTree

} //- namespace -
