using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest_Figure
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ZeroException(int par)
        {   
            Assert.IsTrue(par > 0);
        }
    }
}
