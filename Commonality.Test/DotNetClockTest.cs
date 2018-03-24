using Commonality.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
            Service.Set<ISettings>(new TestSettingsManager());
            SystemClock = new TestSystemClock();
            ThisClock = new Clock(SystemClock);
        }

        [TestMethod]
        public void Empty()
        {
            Assert.IsNotNull(ThisClock);
        }
        [TestMethod]
        public void SimpleTime()
        {
            var expected = new DateTime(2018, 03, 24, 12, 56, 30);
            SystemClock.Now = expected;

            var actual = ThisClock.Now;

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void SimpleTimeNoSettings()
        {
            Service.Clear();

            var expected = new DateTime(2018, 03, 24, 12, 56, 30);
            SystemClock.Now = expected;

            var actual = ThisClock.Now;

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void SetTime()
        {
            SystemClock.Now = new DateTime(2017, 04, 01, 19, 16, 3);
            var expected = new DateTime(2018, 03, 24, 12, 56, 30);
            ThisClock.Now = expected;

            var actual = ThisClock.Now;

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void SetTimeNoSettings()
        {
            Service.Clear();

            SystemClock.Now = new DateTime(2017, 04, 01, 19, 16, 3);
            var expected = new DateTime(2018, 03, 24, 12, 56, 30);
            ThisClock.Now = expected;

            var actual = ThisClock.Now;

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void SetTimeCheckLater()
        {
            SystemClock.Now = new DateTime(2017, 04, 01, 19, 16, 3);
            var expected = new DateTime(2018, 03, 24, 12, 56, 30);
            var offset = TimeSpan.FromHours(3.75);
            ThisClock.Now = expected - offset;

            SystemClock.Now += offset;

            var actual = ThisClock.Now;

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void SetTimeCheckLaterNextRun()
        {
            SystemClock.Now = new DateTime(2017, 04, 01, 19, 16, 3);
            var expected = new DateTime(2018, 03, 24, 12, 56, 30);
            var offset = TimeSpan.FromHours(3.75);
            ThisClock.Now = expected - offset;

            SystemClock.Now += offset;

            ThisClock = new Clock(SystemClock);

            var actual = ThisClock.Now;

            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public async Task CheckNativeMethods()
        {
            // Just make sure they work
            var NativeClock = new Clock();
            var now = NativeClock.Now;
            await NativeClock.Delay(TimeSpan.FromMilliseconds(1));
            var later = NativeClock.Now;

            Assert.IsTrue(later > now);
        }
    }

    class TestSystemClock: ISystemClock
    {
        public DateTime Now { get; set; }
    }
}
