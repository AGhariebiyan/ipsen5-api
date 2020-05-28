

using GMAPI;
using NUnit.Framework;
using System;

namespace IpsenApiTesting
{
    public class Tests
    {
       
        /**
         * Unit test example.
         */
        [Test]
        public void TestSum()
        {
            var mathBl = new TestTheTesting();
            int result = mathBl.Sum(1, 1);
            Assert.True(result == 2);
        }
    }
}