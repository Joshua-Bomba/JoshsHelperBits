using JoshsHelperBits.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelperBitsUT
{
    [TestFixture]
    public class DuplicatesDictionaryTest
    {

        [Test]
        public void BasicTest()
        {
            //Probably should do more testing
            DuplicatesDictionary<string, string> dd = new DuplicatesDictionary<string, string>();
            dd.Add("a", "b");
            dd.Add("a", "c");
            dd.Add("b", "d");

            Assert.IsTrue(dd["a"] == "b");

            ILookup<string, string> asLookup = dd;
            Assert.IsTrue(asLookup["a"].Count() == 2);


            dd.Remove("a");

            Assert.IsFalse(dd.ContainsKey("a"));

        }
    }
}
