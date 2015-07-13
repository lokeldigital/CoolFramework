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

            Tree.Add(3);
            Tree.Add(2);
            Tree.Add(1);

            Tree.InOrder((i) => { Console.WriteLine("> " + i); });
            Tree.Dump();

            Tree.Add(4);
            Tree.Dump();
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
                RootCheck,
                Check_Left_Level1,
                Check_Right_Level1,
                Check_Left_Level2
                ;

            RootCheck = new AvlTree<int>.NodeCheck()
            {
                Verify = (nodeCheck) =>
                {
                    Console.WriteLine(nodeCheck);
                    Assert.AreEqual(2, nodeCheck.Value);
                    return 2 == nodeCheck.Value;
                }
            };

            Check_Left_Level1 = new AvlTree<int>.NodeCheck()
            {
                Verify = (nodeCheck) =>
                {
                    Console.WriteLine(nodeCheck);
                    Assert.AreEqual(RootCheck.ThisId, nodeCheck.ParentId);
                    Assert.AreEqual(RootCheck.LeftId, nodeCheck.ThisId);
                    return 1 == nodeCheck.Value;
                }
            };
            RootCheck.NextCheck = Check_Left_Level1;
            Check_Left_Level2 = new AvlTree<int>.NodeCheck()
            {
                Verify = (nodeCheck) =>
                {
                    Console.WriteLine(nodeCheck);
                    Assert.AreEqual(0, nodeCheck.Value);
                    return 0 == nodeCheck.Value;
                }
            };
            Check_Left_Level1.NextCheck = Check_Left_Level2;

            Check_Right_Level1 = new AvlTree<int>.NodeCheck()
            {
                Verify = (nodeCheck) =>
                {
                    Console.WriteLine(nodeCheck);
                    Assert.AreEqual(RootCheck.ThisId, nodeCheck.ParentId);
                    Assert.AreEqual(RootCheck.RightId, nodeCheck.ThisId);
                    return 4 == nodeCheck.Value;
                }
            };
            Check_Left_Level2.NextCheck = Check_Right_Level1;
            Tree.HierarchyCheck(RootCheck);
        }

        //[TestCase]
        //public void AvlTree_PostOrder_Test() {

        //}
    }

} //-namespace
