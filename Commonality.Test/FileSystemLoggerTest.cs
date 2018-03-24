using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
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

        private async Task<List<string>> ReadLog()
        {
            var lines = new List<string>();

            using (var stream = await FileSystemLogger.OpenLogForRead(Clock.Now))
            {
                var sw = new StreamReader(stream);
                while (!sw.EndOfStream)
                {
                    var line = await sw.ReadLineAsync();
                    lines.Add(line);
                }
            }

            return lines;
        }

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

        [TestMethod]
        public async Task StartSession()
        {
            await Logger.StartSession();

            var result = await FileSystemLogger.GetLogs();

            var actual = result.Single();

            var expected = Clock.Now.ToBinary().ToString("x") + ".txt";

            Assert.AreEqual(expected,actual);
        }

        [TestMethod]
        public async Task StartSessionAndRead()
        {
            await Logger.StartSession();

            var lines = await ReadLog();

            Assert.AreEqual(2, lines.Count);
            Assert.IsTrue(lines[0].Contains("Created"));
            Assert.IsTrue(lines[1].Contains("Started"));
        }


        [TestMethod]
        public async Task LogEventAndRead()
        {
            await Logger.StartSession();
            await Logger.LogEventAsync("Hello");

            var lines = await ReadLog();

            Assert.AreEqual(3, lines.Count);
            Assert.IsTrue(lines[2].Contains("Hello"));
        }
        
        [TestMethod]
        public async Task LogEventWithParametersAndRead()
        {
            await Logger.StartSession();
            await Logger.LogEventAsync("Hello", new[] { "AAA=BBB","CCC=DDD" });

            var lines = await ReadLog();

            Assert.AreEqual(5, lines.Count);
            Assert.IsTrue(lines[3].Contains("AAA"));
            Assert.IsTrue(lines[3].Contains("BBB"));
            Assert.IsTrue(lines[4].Contains("CCC"));
            Assert.IsTrue(lines[4].Contains("DDD"));
        }

        [TestMethod]
        public async Task LogErrorWithParametersAndRead()
        {
            await Logger.StartSession();
            await Logger.ErrorAsync("ABC", new Exception("FAILED"));

            var lines = await ReadLog();

            Assert.AreEqual(4, lines.Count);
            Assert.IsTrue(lines[2].Contains("ABC"));
            Assert.IsTrue(lines[2].Contains("System.Exception"));
            Assert.IsTrue(lines[3].Contains("FAILED"));
        }

        [TestMethod]
        public async Task LogInfoAndRead()
        {
            await Logger.StartSession();
            await Logger.LogInfoAsync("Hello");

            var lines = await ReadLog();

            Assert.AreEqual(3, lines.Count);
            Assert.IsTrue(lines[2].Contains("Hello"));
            Assert.IsTrue(lines[2].Contains("FYI"));
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