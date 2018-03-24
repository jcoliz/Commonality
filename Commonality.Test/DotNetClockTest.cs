using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Commonality.Test
{
    [TestClass]
    public class DotNetClockTest
    {
        private Clock ThisClock;

        [TestInitialize]
        public void SetUp()
        {
            ThisClock = new Clock();
        }

        [TestMethod]
        public void Empty()
        {
            Assert.IsNotNull(ThisClock);
        }
    }
}
