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
        private TestSystemClock SystemClock;

        [TestInitialize]
        public void SetUp()
        {
            SystemClock = new TestSystemClock();
            ThisClock = new Clock(SystemClock);
        }

        [TestMethod]
        public void Empty()
        {
            Assert.IsNotNull(ThisClock);
        }
    }

    class TestSystemClock: ISystemClock
    {
        public DateTime Now { get; set; }
    }
}
