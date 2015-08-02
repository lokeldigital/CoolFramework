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

using Lokel.CoolFramework;
using NUnit.Framework;
using NUnit.Core;



namespace Lokel.CoolFramework.Test {

    public enum StateInfo {
        ERROR = 0,

        PUSHED,
        PROCESSED,
        READY_TO_POP
    }


    [TestFixture]
    public class Stack_Test {
        ProcessingStack<string, StateInfo> StackUnderTest;

        [SetUp]
        public void Setup() {
            StackUnderTest = new ProcessingStack<string, StateInfo>();
            StackUnderTest.Push("good bye", StateInfo.PUSHED);
            StackUnderTest.Push("middle", StateInfo.PUSHED);
            StackUnderTest.Push("hello", StateInfo.PUSHED);
        }

        [TestCase]
        public void ProcessingStack_ReadAndWriteState_Test() {
            string val = null;
            StateInfo Step;

            Assert.AreEqual(StateInfo.PUSHED, StackUnderTest.StackTop_Step);
            Assert.AreEqual("hello", StackUnderTest.StackTop_Item);

            StackUnderTest.StackTop_Step = StateInfo.PROCESSED;
            Assert.AreEqual(StateInfo.PROCESSED, StackUnderTest.StackTop_Step);

            Step = StackUnderTest.Pop(ref val);
            Assert.AreEqual(StateInfo.PROCESSED, Step);
            Assert.AreEqual("hello", val);

            Assert.AreEqual(StateInfo.PUSHED, StackUnderTest.StackTop_Step);
            StackUnderTest.StackTop_Step = StateInfo.READY_TO_POP;
            Assert.AreEqual(StateInfo.READY_TO_POP, StackUnderTest.StackTop_Step);

            StackUnderTest.Pop();
            Assert.AreEqual(StateInfo.PUSHED, StackUnderTest.StackTop_Step);
            Assert.AreEqual("good bye", StackUnderTest.StackTop_Item);

            StackUnderTest.Pop();
            Assert.AreEqual(StateInfo.ERROR, StackUnderTest.StackTop_Step);
        }

        [TestCase]
        public void ProcessingStack_ExceedInitialStackCapacity_Test() {
            int testVolume = ProcessingStack<string, StateInfo>.DEFAULT_SIZE * 2;

            while (StackUnderTest.StackDepth < testVolume) {
                StackUnderTest.Push(string.Format("Padding {0}", StackUnderTest.StackDepth), StateInfo.PUSHED);
            }

            for (int i = testVolume; i > 0; i--) {
                Assert.AreNotEqual(StateInfo.ERROR, StackUnderTest.StackTop_Step);
                Console.WriteLine("Stack Value: " + StackUnderTest.StackTop_Item);
                StackUnderTest.Pop();
            }

            Assert.AreEqual(StateInfo.ERROR, StackUnderTest.StackTop_Step);
            Assert.IsTrue(StackUnderTest.Empty);
        }
    }
}

