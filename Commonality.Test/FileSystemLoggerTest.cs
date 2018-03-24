using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commonality.Test
{
    [TestClass]
    public class FileSystemLoggerTest
    {
        private FileSystemLogger Logger;
        private MockFileSystem FileSystem;
        private TestClock Clock; 

        [TestInitialize]
        public void SetUp()
        {
            Clock = new TestClock() { Now = new DateTime(2018, 03, 24, 13, 39, 50) };
            Service.Set<IClock>(Clock);
            FileSystem = new MockFileSystem();
            Logger = new FileSystemLogger( FileSystem );
        }

        [TestMethod]
        public void Empty()
        {
            Assert.IsNotNull(Logger);
        }

        // FAILS. Needs more work to mock out a filesystem for FileSystemLogger
        [TestMethod]
        public async Task StartSession()
        {
            await Logger.StartSession();

            var result = await FileSystemLogger.GetLogs();

            var actual = result.Single();

            var expected = Clock.Now.ToBinary().ToString("x") + ".txt";

            Assert.AreEqual(expected,actual);
        }

    }

    class TestClock : IClock
    {
        public DateTime Now { get; set; }

        public async Task Delay(TimeSpan t)
        {
        }
    }
}