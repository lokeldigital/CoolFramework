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

using NUnit.Framework;
using Lokel.CoolFramework;

namespace Lokel.CoolFramework.Test {

    [TestFixture]

    public class AvlTreeTest {

        [TestCase]
        public void AvlTree_Add_Test() {
            Func<int, int, bool> Smaller = (a, b) => { return a < b; };
            AvlTree<int> Tree = new AvlTree<int>(Smaller);
            AvlTree<int>.NodeCheck
                _2 = AvlTree<int>.NodeCheck.ConvirmValue(2),
                _1 = AvlTree<int>.NodeCheck.ConvirmValue(1),
                _3 = AvlTree<int>.NodeCheck.ConvirmValue(3),
                _4 = AvlTree<int>.NodeCheck.ConvirmValue(4);

            Tree.Add(3);
            Tree.Add(2);
            Tree.Add(1);
            Tree.Add(4);

            _2.NextCheck = _1;
            _1.NextCheck = _3;
            _3.NextCheck = _4;
            Tree.HierarchyCheck(_2);
            //Tree.Dump();
        }

        [TestCase]
        public void AvlTree_AddDoubleRotateLeft_Test() {
            Func<int, int, bool> Smaller = (a, b) => { return a < b; };
            AvlTree<int> Tree = new AvlTree<int>(Smaller);
            AvlTree<int>.NodeCheck
                _3 = AvlTree<int>.NodeCheck.ConvirmValue(3),
                _1 = AvlTree<int>.NodeCheck.ConvirmValue(1),
                _2 = AvlTree<int>.NodeCheck.ConvirmValue(2),
                _4 = AvlTree<int>.NodeCheck.ConvirmValue(4),
                _5 = AvlTree<int>.NodeCheck.ConvirmValue(5);
            _3.NextCheck = _1;
            _1.NextCheck = _2;
            _2.NextCheck = _4;
            _4.NextCheck = _5;

            Tree.Add(1);
            Tree.Add(4);
            Tree.Add(3);
            Tree.Add(5);
            Tree.Add(2);

            //Tree.Dump();
            Tree.HierarchyCheck(_3);
        }

        [TestCase]
        public void AvlTree_AddRotateLeft_Test() {
            Func<int, int, bool> Smaller = (a, b) => { return a < b; };
            AvlTree<int> Tree = new AvlTree<int>(Smaller);
            AvlTree<int>.NodeCheck
                _2 = AvlTree<int>.NodeCheck.ConvirmValue(2),
                _1 = AvlTree<int>.NodeCheck.ConvirmValue(1),
                _4 = AvlTree<int>.NodeCheck.ConvirmValue(4),
                _3 = AvlTree<int>.NodeCheck.ConvirmValue(3),
                _5 = AvlTree<int>.NodeCheck.ConvirmValue(5);
            _2.NextCheck = _1;
            _1.NextCheck = _4;
            _4.NextCheck = _3;
            _3.NextCheck = _5;

            Tree.Add(1);
            Tree.Add(2);
            Tree.Add(3);
            Tree.Add(4);
            Tree.Add(5);

            //Tree.Dump();
            Tree.HierarchyCheck(_2);
        }

        [TestCase]
        public void AvlTree_Balance_Test() {
            AvlTree<int> Tree = new AvlTree<int>((a, b) => { return a < b; });

            Tree.Add(5);
            Tree.Add(4);
            Tree.Add(3);
            Tree.Add(2);
            Tree.Add(1);
            Tree.Add(0);
            Tree.Dump();

            AvlTree<int>.NodeCheck
                RootCheck = new AvlTree<int>.NodeCheck(),
                Check_Left1 = new AvlTree<int>.NodeCheck(),
                Check_Right1 = new AvlTree<int>.NodeCheck(),
                Check_Left1_Left2 = new AvlTree<int>.NodeCheck(),
                Check_Right1_Left2 = new AvlTree<int>.NodeCheck(),
                Check_Right1_Right2 = new AvlTree<int>.NodeCheck()
                ;
            // This is an explicit structural test
            RootCheck.Verify = (nodeCheck) =>
            {
                Console.WriteLine(nodeCheck);
                Assert.AreEqual(2, nodeCheck.Value);

                Assert.AreEqual(0, nodeCheck.ParentId);
                Assert.AreNotEqual(0, nodeCheck.LeftId);
                Assert.AreNotEqual(0, nodeCheck.RightId);
                return true;
            };
            RootCheck.NextCheck = Check_Left1;

            Check_Left1.Verify = (nodeCheck) =>
            {
                Console.WriteLine(nodeCheck);
                Assert.AreEqual(1, nodeCheck.Value);

                Assert.AreEqual(RootCheck.ThisId, nodeCheck.ParentId);
                Assert.AreEqual(RootCheck.LeftId, nodeCheck.ThisId);
                Assert.Greater(RootCheck.Value, nodeCheck.Value);
                Assert.AreNotEqual(0, nodeCheck.LeftId); // should be left node
                Assert.AreEqual(0, nodeCheck.RightId); // but no right node.
                return true;
            };
            Check_Left1.NextCheck = Check_Left1_Left2;

            Check_Left1_Left2.Verify = (nodeCheck) =>
            {
                Console.WriteLine(nodeCheck);
                Assert.AreEqual(0, nodeCheck.Value);

                Assert.AreEqual(0, nodeCheck.LeftId);
                Assert.AreEqual(0, nodeCheck.RightId);
                Assert.AreEqual(Check_Left1.ThisId, nodeCheck.ParentId);
                Assert.Greater(Check_Left1.Value, nodeCheck.Value);
                return true;
            };
            Check_Left1_Left2.NextCheck = Check_Right1;
            
            Check_Right1.Verify = (nodeCheck) =>
            {
                Console.WriteLine(nodeCheck);
                Assert.AreEqual(4, nodeCheck.Value);

                Assert.AreEqual(RootCheck.ThisId, nodeCheck.ParentId);
                Assert.AreEqual(RootCheck.RightId, nodeCheck.ThisId);
                Assert.AreNotEqual(0, nodeCheck.LeftId);
                Assert.AreNotEqual(0, nodeCheck.RightId);
                Assert.Less(RootCheck.Value, nodeCheck.Value);
                return true;
            };
            Check_Right1.NextCheck = Check_Right1_Left2;

            Check_Right1_Left2.Verify = (nodeCheck) =>
            {
                Console.WriteLine(nodeCheck);
                Assert.AreEqual(3, nodeCheck.Value);

                Assert.AreEqual(Check_Right1.ThisId, nodeCheck.ParentId);
                Assert.AreEqual(Check_Right1.LeftId, nodeCheck.ThisId);
                Assert.AreEqual(0, nodeCheck.LeftId);
                Assert.AreEqual(0, nodeCheck.RightId);
                Assert.Greater(Check_Right1.Value, nodeCheck.Value);
                return true;
            };
            Check_Right1_Left2.NextCheck = Check_Right1_Right2;

            Check_Right1_Right2.Verify = (nodeCheck) =>
            {
                Console.WriteLine(nodeCheck);
                Assert.AreEqual(5, nodeCheck.Value);

                Assert.AreEqual(Check_Right1.ThisId, nodeCheck.ParentId);
                Assert.AreEqual(Check_Right1.RightId, nodeCheck.ThisId);
                Assert.AreEqual(0, nodeCheck.LeftId);
                Assert.AreEqual(0, nodeCheck.RightId);
                Assert.Less(Check_Right1.Value, nodeCheck.Value);
                return false; // end of tree
            };

            Tree.HierarchyCheck(RootCheck);
            Console.WriteLine("\r\n: Tree Looks Like:");
            Tree.InOrder((num) => { Console.WriteLine("> {0}", num); });
        }

        [TestCase]
        public void AvlTree_StringInsertionRebalance_Test() {
            Func<string, string, bool> Smaller = (text1, text2) =>
            {
                return string.Compare(text1,text2,StringComparison.OrdinalIgnoreCase) < 0;
            };
            AvlTree<string> StrTree = new AvlTree<string>(Smaller);

            StrTree.Add("ORY");
            StrTree.Add("JFK");
            StrTree.Add("BRU");
            StrTree.Add("DUS");
            StrTree.Add("ZRH");
            StrTree.Add("MEX");
            StrTree.Add("ORD");
            StrTree.Add("NRT");

            AvlTree<string>.NodeCheck
                _JFK = AvlTree<string>.NodeCheck.ConvirmValue("JFK"),
                _BRU = AvlTree<string>.NodeCheck.ConvirmValue("BRU"),
                _DUS = AvlTree<string>.NodeCheck.ConvirmValue("DUS"),
                _ORY = AvlTree<string>.NodeCheck.ConvirmValue("ORY"),
                _NRT = AvlTree<string>.NodeCheck.ConvirmValue("NRT"),
                _MEX = AvlTree<string>.NodeCheck.ConvirmValue("MEX"),
                _ORD = AvlTree<string>.NodeCheck.ConvirmValue("ORD"),
                _ZRH = AvlTree<string>.NodeCheck.ConvirmValue("ZRH");

            _JFK.NextCheck = _BRU;
            _BRU.NextCheck = _DUS;
            _DUS.NextCheck = _ORY;
            _ORY.NextCheck = _NRT;
            _NRT.NextCheck = _MEX;
            _MEX.NextCheck = _ORD;
            _ORD.NextCheck = _ZRH;

            StrTree.HierarchyCheck(_JFK);
        }

        [TestCase]
        public void AvlTree_String_ExtendedInsertionRebalance_Test() {
            Func<string, string, bool> Smaller = (text1, text2) =>
            {
                return string.Compare(text1, text2, StringComparison.OrdinalIgnoreCase) < 0;
            };
            AvlTree<string> StrTree = new AvlTree<string>(Smaller);

            StrTree.Add("ORY");
            StrTree.Add("JFK");
            StrTree.Add("BRU");
            StrTree.Add("DUS");
            StrTree.Add("ZRH");
            StrTree.Add("MEX");
            StrTree.Add("ORD");
            StrTree.Add("NRT");
            StrTree.Add("ARN");
            StrTree.Add("GLA");
            StrTree.Add("GCM");

            AvlTree<string>.NodeCheck
                _JFK = AvlTree<string>.NodeCheck.ConvirmValue("JFK"),
                _BRU = AvlTree<string>.NodeCheck.ConvirmValue("BRU"),
                _ARN = AvlTree<string>.NodeCheck.ConvirmValue("ARN"),
                _GCM = AvlTree<string>.NodeCheck.ConvirmValue("GCM"),
                _DUS = AvlTree<string>.NodeCheck.ConvirmValue("DUS"),
                _GLA = AvlTree<string>.NodeCheck.ConvirmValue("GLA"),
                _ORY = AvlTree<string>.NodeCheck.ConvirmValue("ORY"),
                _NRT = AvlTree<string>.NodeCheck.ConvirmValue("NRT"),
                _MEX = AvlTree<string>.NodeCheck.ConvirmValue("MEX"),
                _ORD = AvlTree<string>.NodeCheck.ConvirmValue("ORD"),
                _ZRH = AvlTree<string>.NodeCheck.ConvirmValue("ZRH");

            _JFK.NextCheck = _BRU;
            _BRU.NextCheck = _ARN;
            _ARN.NextCheck = _GCM;
            _GCM.NextCheck = _DUS;
            _DUS.NextCheck = _GLA;
            _GLA.NextCheck = _ORY;
            _ORY.NextCheck = _NRT;
            _NRT.NextCheck = _MEX;
            _MEX.NextCheck = _ORD;
            _ORD.NextCheck = _ZRH;

            StrTree.HierarchyCheck(_JFK);
        }

    } //-Test Class

} //-namespace

