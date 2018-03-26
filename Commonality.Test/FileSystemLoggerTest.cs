#define IO_ABSTRACTIONS

using Commonality.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// :( Cannot test filesystemlogger because can't use system.io.abstractions, because doesn't work on UWP :(:(

#if IO_ABSTRACTIONS
namespace Commonality.Test
{
    [TestClass]
    public class FileSystemLoggerTest
    {
        private FileSystemLogger Logger;
        private TestFileSystem FileSystem;
        private TestClock Clock; 

        private async Task<List<string>> ReadLog()
        {
            var lines = new List<string>();

            using (var stream = FileSystemLogger.OpenLogForRead(Clock.Now))
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
            FileSystem = new TestFileSystem();
            Logger = new FileSystemLogger( FileSystem );
        }

        [TestMethod]
        public void Empty()
        {
            Assert.IsNotNull(Logger);
        }

        [TestMethod]
        public void EmptyWithFilesystem()
        {
            // Just make sure we can construct one without the abstracted file system
            Logger = new FileSystemLogger("Home");
            Assert.IsNotNull(Logger);
        }

        [TestMethod]
        public async Task StartSession()
        {
            await Logger.StartSession();

            var result = FileSystemLogger.GetLogs();

            var actual = result.Single();

            var expected = Clock.Now;

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

        //[TestMethod]
        public async Task StartSessionAndReadNoFilesystem()
        {
            // The purpose of this is to inject filesystem exceptions, and make sure we 
            // recover cleanly and don't propagate exceptions out

           // Logger = new FileSystemLogger(new BogusFileSystem());
           // await Logger.StartSession();
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
        public async Task LogEventAndReadNoClock()
        {
            await Logger.StartSession();
            Service.Clear();
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
        public async Task LogErrorWithMoreParametersAndRead()
        {
            await Logger.StartSession();

            Exception expected = null;
            try
            {
                throw new Exception("FAILED") { Source = "TEST" };
            }
            catch(Exception ex)
            {
                expected = ex;
            }

            await Logger.ErrorAsync("ABC", expected);

            var lines = await ReadLog();

            Assert.AreEqual(6, lines.Count);
            Assert.IsTrue(lines[2].Contains("ABC"));
            Assert.IsTrue(lines[2].Contains("System.Exception"));
            Assert.IsTrue(lines[3].Contains("LogErrorWithMoreParametersAndRead"));
            Assert.IsTrue(lines[4].Contains("TEST"));
            Assert.IsTrue(lines[5].Contains("FAILED"));
        }

        [TestMethod]
        public async Task LogErrorWithMoreParametersAndReadNew()
        {
            await Logger.StartSession();

            Exception expected = null;
            try
            {
                throw new Exception("FAILED") { Source = "TEST" };
            }
            catch (Exception ex)
            {
                expected = ex;
            }

            await Logger.LogErrorAsync(expected);

            var lines = await ReadLog();

            Assert.AreEqual(5, lines.Count);
            Assert.IsTrue(lines[2].Contains("TEST"));
            Assert.IsTrue(lines[2].Contains("System.Exception"));
            Assert.IsTrue(lines[3].Contains("LogErrorWithMoreParametersAndRead"));
            Assert.IsTrue(lines[4].Contains("FAILED"));
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

        [TestMethod]
        public async Task LogInfoSynchronous()
        {
            // If this starts failing, check the external semaphore timing.
            await Logger.StartSession();
            Logger.LogInfo("Hello");
            await Logger.Wait();

            var lines = await ReadLog();

            Assert.AreEqual(3, lines.Count);
            Assert.IsTrue(lines[2].Contains("Hello"));
            Assert.IsTrue(lines[2].Contains("FYI"));
        }
        [TestMethod]
        public async Task LogErrorWithParametersAndReadSynchronous()
        {
            await Logger.StartSession();
            Logger.Error("ABC", new Exception("FAILED"));
            await Logger.Wait();

            var lines = await ReadLog();

            Assert.AreEqual(4, lines.Count);
            Assert.IsTrue(lines[2].Contains("ABC"));
            Assert.IsTrue(lines[2].Contains("System.Exception"));
            Assert.IsTrue(lines[3].Contains("FAILED"));
        }
        [TestMethod]
        public async Task LogErrorWithParametersAndReadSynchronousNew()
        {
            await Logger.StartSession();
            Logger.LogError(new Exception("FAILED") { Source = "ABC" });
            await Logger.Wait();

            var lines = await ReadLog();

            Assert.AreEqual(4, lines.Count);
            Assert.IsTrue(lines[2].Contains("ABC"));
            Assert.IsTrue(lines[2].Contains("System.Exception"));
            Assert.IsTrue(lines[3].Contains("FAILED"));
        }
        [TestMethod]
        public async Task LogEventAndReadSynchronous()
        {
            await Logger.StartSession();
            Logger.LogEvent("Hello");
            await Logger.Wait();

            var lines = await ReadLog();

            Assert.AreEqual(3, lines.Count);
            Assert.IsTrue(lines[2].Contains("Hello"));
        }

    }

    class TestClock : IClock
    {
        public DateTime Now { get; set; }

        public async Task Delay(TimeSpan t)
        {
        }
    }

    class TestFileSystem : ILoggerFileSystem
    {
        Dictionary<DateTime, List<string>> LogEntries = new Dictionary<DateTime, List<string>>();

        public async Task Append(DateTime dt, IEnumerable<string> lines)
        {
            LogEntries[dt].AddRange(lines);
        }

        public async Task Create(DateTime dt, string line)
        {
            LogEntries[dt] = new List<string>();
            LogEntries[dt].Add(line);
        }

        public IEnumerable<DateTime> Directory()
        {
            return LogEntries.Keys;
        }

        public Stream OpenForRead(DateTime dt)
        {
            var builder = new StringBuilder();
            foreach (var line in LogEntries[dt])
                builder.AppendLine(line);

            var file = Encoding.ASCII.GetBytes(builder.ToString());

            return new MemoryStream(file);
        }
    }
}
#endif