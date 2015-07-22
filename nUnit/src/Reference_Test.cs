
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
    class Has_Test {

        [TestCase]
        public void Reference_Simple_Test() {
            string myString = "hi there";
            string val = null;
            bool mustBeTrue = false;

            Console.WriteLine("-Started-");
            Reference.NotNull(myString).AndNotNull(val).IfTrue(() => { Assert.Fail(); });
            val = "yo";
            Reference.NotNull(myString).AndNotNull(val).IfTrue(() => {
                mustBeTrue = true;
            })
            .IfTrue(() => { Console.WriteLine("Values are myString = {0} / val = {1}", myString, val); });
            Assert.IsTrue(mustBeTrue);
        }

        [TestCase(1000)]
        public void Reference_Repeated_Test(int numRepeats) {
            string value = "Count:";
            Console.Write(".");
            while (numRepeats > 0) {
                value += string.Format(" {0}", numRepeats);
                Reference.NotNull(value).IfTrue(() => { Console.Write("."); }).IfFalse(() => { Assert.Fail(); });
                numRepeats--;
            }
        }

    }

}
