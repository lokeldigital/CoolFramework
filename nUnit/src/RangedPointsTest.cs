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

using NUnit.Core;
using NUnit.Framework;

namespace Lokel.CoolFramework.Test {

    [TestFixture]
    public class RangedPointsTest {
        private Bunch<Point_rf> Points = null;
        [SetUp]
        public void Setup() {
            Points = new Bunch<Point_rf>();
            Points.Add(new Point_rf(10, 20));
            Points.Add(new Point_rf(-15, 2));
            Points.Add(new Point_rf(12, -20));
            Points.Add(new Point_rf(15, 8));
            Points.Add(new Point_rf( 6, 5));
            Points.Add(new Point_rf(-1, -4));
        }

        [TestCase]
        public void Test() {

        }
    }
}
