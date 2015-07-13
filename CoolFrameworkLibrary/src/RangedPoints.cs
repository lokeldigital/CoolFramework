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

    internal class PointNode {
        private PointNode _Parent;
        private PointNode _NearBound1, _NearBound2;
        private Point_rf _Point;

        internal PointNode Parent { get { return _Parent; } }
        internal Point_rf Point { get { return _Point; } }

        internal PointNode(PointNode parent, Point_rf pt, PointNode near1, PointNode near2) {
            _Parent = parent;
            _Point = pt;
            _NearBound1 = near1;
            _NearBound2 = near2;
        }

        internal PointNode(PointNode parent, Point_rf pt)
            : this(parent, pt, null, null) { }

        internal PointNode(Point_rf pt)
            : this(null, pt, null, null) { }

        enum Step {
            PUSH_LEFT,
            RETURN_VAL,
            VISIT_RIGHT,
            POP_CENTRE
        }

        private class ClassNode {
            internal PointNode _Node;
            internal Step _Step;
        }
        private ClassNode[] _Stack;
        private int _StackPos = 0;

        private void Push(PointNode pn, Step nextStep) {
            if (_StackPos == _Stack.Length) {
                Array.Resize( ref _Stack, _Stack.Length * 2);
            }
            _Stack[_StackPos] = new ClassNode()
            {
                _Node = pn,
                _Step = nextStep
            };
            _StackPos++;
        }

        private ClassNode Pop() {
            ClassNode n = null;
            if (_StackPos > 0) {
                _StackPos--;
                n = _Stack[_StackPos];
            }
            return n;
        }

        private ClassNode Top() {
            ClassNode n = null;
            if (_StackPos > 0) {
                n = _Stack[_StackPos - 1];
            }
            return n;
        }

        IEnumerable<Point_rf> InOrder() {
            PointNode leaf = this;
            _Stack = new ClassNode[32];

            Push(this, Step.PUSH_LEFT);
            while (_StackPos > 0) {
                leaf = Top()._Node;
                switch (Top()._Step) {
                    case Step.PUSH_LEFT:
                        Top()._Step = Step.RETURN_VAL;
                        if (leaf._NearBound1 != null) {
                            Push(leaf._NearBound1, Step.PUSH_LEFT);
                        }
                        break;
                    case Step.RETURN_VAL:
                        Top()._Step = Step.VISIT_RIGHT;
                        yield return Top()._Node._Point;
                        break;
                    case Step.VISIT_RIGHT:
                        Top()._Step = Step.POP_CENTRE;
                        leaf = Top()._Node;
                        if (leaf._NearBound2 != null) {
                            Push(leaf._NearBound2, Step.PUSH_LEFT);
                        }
                        break;
                    case Step.POP_CENTRE:
                        Pop();
                        break;
                }
            }
            _Stack = null;
            yield break;
        }
    } // -- PointNode

    public class RangedPoints_f {
        private Point_rf _Bounds1, _Bounds2;

        public RangedPoints_f(Bunch<Point_rf> Points) {
            _Bounds1 = _Bounds2 = null;
            ArrangePoints(Points);
        }

        private void ArrangePoints(Bunch<Point_rf> Points) {
            Action<Point_rf> Plan;
            Action<Point_rf> Plan2 = (point)=>{
                // Either point is outside of b1/b2 or inside.
                Point_rf mid = new Point_rf((_Bounds1.x + _Bounds2.x) / 2, (_Bounds1.y + _Bounds2.y) / 2);
                float b1 = Point_rf.DistanceSqared(mid, _Bounds1);
                float b2 = Point_rf.DistanceSqared(mid, _Bounds2);
                float p = Point_rf.DistanceSqared(mid, point);

                if (p > b1) {
                    if (b1 > b2)
                        _Bounds2 = point;
                    else
                        _Bounds1 = point;
                } else if (p > b2) {
                    _Bounds2 = point;
                }
            };

            Plan = (point) =>
            {
                if (_Bounds1 == null) {
                    _Bounds1 = point;
                } else if (_Bounds2 == null) {
                    if (point.y >= _Bounds1.y) {
                        _Bounds2 = point;
                    } else {
                        _Bounds2 = _Bounds1;
                        _Bounds1 = point;
                    }
                    Plan = Plan2;
                }
            };

            Points.EachDo(Plan);
        }
    }

}
