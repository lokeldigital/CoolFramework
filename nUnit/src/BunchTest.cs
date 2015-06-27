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
using NUnit.Framework;

using Lokel.CoolFramework;

namespace Lokel.CoolFramework.Test {

    [TestFixture]
    class BunchTest {

        static int[] Vals = { -1, 10, 20, 15000,
                                3, 9, 8, 21, 
                                65, 45, 32, 65530,
                                -21034, 10, 16, 8
                            };
        private Bunch<int> B = null;

        private void Init(int num_vals) {
            B = new Bunch<int>();
            int i = 0;
            for (i = 0; i < num_vals; i++) {
                B.Add(Vals[i & 0x0F]);
            }
        }

        [TestCase(0)]
        [TestCase(1)]
        public void EachDoTest(int num_vals) {
            Init(num_vals);
            int count = 0;
            B.EachDo((int v) =>
            {
                count++;
            });
            Assert.AreEqual(num_vals, count);
        }
    }
}
