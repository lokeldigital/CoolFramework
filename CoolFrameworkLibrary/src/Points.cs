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
    public class Point_rf {
        private float _x;
        private float _y;

        public float x { get { return _x; } }
        public float y { get { return _y; } }

        public Point_rf(float i, float j) {
            _x = i;
            _y = j;
        }

        public Point_rf(Point_rf p) : this(p._x, p._y) { }

        public Point_rf() : this(0f, 0f) { }

        public override string ToString() {
            return string.Format("Point[ref, float]( {0}, {1})", _x, _y);
        }

        public static float DistanceSqared(Point_rf A, Point_rf B) {
            float hori = (A._x - B._x);
            float vert = (A._y - B._y);
            return (hori * hori) + (vert * vert);
        }
    } //-- Point_rf --

    public struct Point_vf {
        private float _x;
        private float _y;

        public float x { get { return _x; } }
        public float y { get { return _y; } }

        public Point_vf(float i, float j) {
            _x = i;
            _y = j;
        }

        public Point_vf(Point_vf p) : this(p._x, p._y) { }

        public Point_vf(Point_rf rp) : this(rp.x, rp.y) { }

        public override string ToString() {
            return string.Format("Point[val, float]( {0}, {1})", _x, _y);
        }

        public static float DistanceSqared(Point_vf A, Point_vf B) {
            float hori = (A._x - B._x);
            float vert = (A._y - B._y);
            return (hori * hori) + (vert * vert);
        }
    } //-- Point_vf --

} //-namespace
