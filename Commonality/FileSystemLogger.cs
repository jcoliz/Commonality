using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Commonality
{
    /// <summary>
    /// This is a simple logger which just logs to the local file system. No cloud analytics here.
    /// </summary>
    /// <remarks>
    /// Service Dependencies:
    ///     * IClock
    /// </remarks>

    public class FileSystemLogger: ILogger
    {
        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="homedir">Directory where on the filesystem to store the logs</param>
        public FileSystemLogger(string homedir = ""):
            this(new LoggerFileSystem(homedir))
        {
        }

        /// <summary>
        /// Constructor, with an IFileSystem for testing
        /// </summary>
        public FileSystemLogger(ILoggerFileSystem fileSystem)
        {
            FileSystem = fileSystem;
        }
        #endregion

        #region Public Interface (ILogger)
        /// <summary>
        /// Begin the logging session. Call the once when the app starts
        /// </summary>
        public async Task StartSession()
        {
            await Log(new[] { "Started" });
        }

        /// <summary>
        /// Report an exception
        /// </summary>
        /// <param name="key">Unique key to identify where in the app the exception was thrown</param>
        /// <param name="ex">Exception to report</param>
        [Obsolete("Error is deprecated, please use LogError instead, with the code in ex.Source")]
        public void Error(string key, Exception ex)
        {
            var ignore = ErrorAsync(key,ex);
        }

        /// <summary>
        /// Report an exception asynchronously
        /// </summary>
        /// <param name="key">Unique key to identify where in the app the exception was thrown</param>
        /// <param name="ex">Exception to report</param>
        [Obsolete("ErrorAsync is deprecated, please use LogErrorAsync instead, with the code in ex.Source")]
        public async Task ErrorAsync(string key, Exception ex) => await ErrorAsyncInternal(key, ex);

        private async Task ErrorAsyncInternal(string key, Exception ex)
        {
            var list = new List<string>();

            string showkey = key ?? ex.Source;

            list.Add($"Error: {showkey}/{ex.GetType().ToString()}");
            if ( ! string.IsNullOrEmpty( ex.StackTrace ) )
                list.Add($", Stack = {ex.StackTrace}");
            if ( ! string.IsNullOrEmpty(ex.Source) && !string.IsNullOrEmpty(key))
                list.Add($", Source = {ex.Source}");
            Exception e = ex;
            while (e != null)
            {
                list.Add($", Message = {e.GetType().ToString()} {e.Message}");
                e = e.InnerException;
            }
            await Log(list);
        }

        /// <summary>
        /// Report an exception
        /// </summary>
        /// <remarks>
        /// Expects a short tag in ex.Source, which some backend services will use for aggregation
        /// </remarks>
        /// <param name="ex">Exception to report</param>
        public void LogError(Exception ex)
        {
            var ignore = LogErrorAsync(ex);
        }

        /// <summary>
        /// Report an exception asynchronously
        /// </summary>
        /// <remarks>
        /// Expects a short tag in ex.Source, which some backend services will use for aggregation
        /// </remarks>
        /// <param name="ex">Exception to report</param>
        public async Task LogErrorAsync(Exception ex) => await ErrorAsyncInternal(null, ex);

        /// <summary>
        /// Log an event immediately
        /// </summary>
        /// <param name="message">Descriptive message of what's goig on. Usually short</param>
        /// <param name="parameters">Additional parameters, usually 'key=value'</param>
        public void LogEvent(string message, params string[] parameters)
        {
            Debug.WriteLine($"LogEvent {message}");
            var ignore = LogEventAsync(message,parameters);
        }

        /// <summary>
        /// Log an event asynchronously
        /// </summary>
        /// <param name="message">Descriptive message of what's goig on. Usually short</param>
        /// <param name="parameters">Additional parameters, usually 'key=value'</param>
        /// <returns>Awaitable task</returns>
        public async Task LogEventAsync(string message, params string[] parameters)
        {
            Debug.WriteLine($"LogEventAsync {message}");
            var list = new List<string>(parameters.Select(x=>$", {x}"));
            list.Insert(0, $"Event: {message}");
            await Log(list);
        }

        /// <summary>
        /// Log an informative message
        /// </summary>
        /// <param name="message">Descriptive message of what's goig on. Usually detailed</param>
        public void LogInfo(string message)
        {
            var ignore = LogInfoAsync(message);
        }

        /// <summary>
        /// Log an informative message, asynchronously
        /// </summary>
        /// <param name="message">Descriptive message of what's goig on. Usually detailed</param>
        public async Task LogInfoAsync(string message)
        {
            await Log(new[] { $"FYI: {message}" });
        }

        #endregion

        #region Additional public interface

        /// <summary>
        /// Await until the current log has written
        /// </summary>
        /// <remarks>
        /// Useful for unit testing the non-async methods
        /// </remarks>
        /// <returns></returns>

        /// <summary>
        /// Retrieve listing of all the log files
        /// </summary>
        /// <returns>List of all the log files</returns>
        public static IEnumerable<DateTime> GetLogs()
        {
            return FileSystem.Directory();
        }

        /// <summary>
        /// Get a single log for reading
        /// </summary>
        /// <param name="dt">Datetime which corresponds to a filename returned from GetLogs</param>
        /// <returns>Stream to read a single log file</returns>
        public static async Task<IEnumerable<string>> ReadContents(DateTime dt)
        {
            IEnumerable<string> result = null;

            try
            {
                await Semaphore.WaitAsync();
                result = await FileSystem.ReadContents(dt);
            }
            finally
            {
                Semaphore.Release();
            }

            return result;
        }

        #endregion

        #region Internal Methods
        /// <summary>
        /// Internal method to actually write to the logs
        /// </summary>
        /// <param name="lines">Text lines to be written</param>
        /// <returns>Awaitable task</returns>
        private async Task Log(IEnumerable<string> lines)
        {
            try
            {
                await Semaphore.WaitAsync();

                if (!SessionId.HasValue)
                {
                    SessionId = Time;
                    await FileSystem.Create(SessionId.Value, FormattedLine("Created"));
                }

                await FileSystem.Append(SessionId.Value, lines.Select(x => FormattedLine(x)));
            }
            finally
            {
                Semaphore.Release();
            }
        }
        #endregion

        #region Internal Properties

        /// <summary>
        /// Identifier for which logging session we are currently logging into
        /// </summary>
        private DateTime? SessionId;

        /// <summary>
        /// Semaphore used to control access to the logs. Only one thread can write to logs at once!
        /// </summary>
        private static SemaphoreSlim Semaphore = new SemaphoreSlim(1);

        /// <summary>
        /// Which filesystem we are using. This can be overriden for testing
        /// </summary>
        private static ILoggerFileSystem FileSystem;

        /// <summary>
        /// Format the given line into what it should look like when it actually goes into
        /// the log.
        /// </summary>
        /// <remarks>
        /// This can be overridden by derived class to do something special with the formatting
        /// </remarks>
        /// <param name="originalline">Unformatted raw line</param>
        /// <returns>Formatted line ready to log</returns>
        protected virtual string FormattedLine(string originalline) => Time.ToString("u") + " " + originalline;

        /// <summary>
        /// If there IS a platform clock use that for time, else just pick up regular
        /// system time.
        /// </summary>
        protected DateTime Time => Clock?.Now ?? DateTime.Now;
        #endregion

        private IClock Clock => Service.TryGet<IClock>();
    }

    /// <summary>
    /// Describes the encapsulated filesystem interaction used by the FileSystemLogger
    /// </summary>
    public interface ILoggerFileSystem
    {
        Task Create(DateTime dt, string line);
        Task Append(DateTime dt, IEnumerable<string> lines);
        Task<IEnumerable<string>> ReadContents(DateTime dt);
        IEnumerable<DateTime> Directory();
    }

    /// <summary>
    /// Implements the minimum needed filesystem interaction used by the FileSystem Logger
    /// </summary>
    /// <remarks>
    /// Encapsulating all the filesystem access makes it easier to test. Also seems to clean
    /// up the logger itself
    /// </remarks>
    //[ExcludeFromCodeCoverage]
    class LoggerFileSystem : ILoggerFileSystem
    {
        private string HomeDirectory;

        public LoggerFileSystem(string _HomeDirectory = "")
        {
            HomeDirectory = _HomeDirectory;
        }

        public async Task Append(DateTime dt, IEnumerable<string> lines)
        {
            try
            {
                var SessionFilename = "Logs/" + dt.ToBinary().ToString("x") + ".txt";

                var path = HomeDirectory + SessionFilename;
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir))
                    System.IO.Directory.CreateDirectory(dir);

                using (var sw = File.AppendText(path))
                {
                    foreach (var line in lines)
                        await sw.WriteLineAsync(line);
                    await sw.FlushAsync();
                }

            }
            catch (Exception)
            {
                // All we can do is swallow exceptions here. We are deep INSIDE our exception
                // logging mechanism!
            }
        }

        /// <summary>
        /// Create a new logging session, and add this line
        /// </summary>
        /// <param name="dt">Session identifier</param>
        /// <param name="line">Logging text line</param>
        /// <returns></returns>
        public async Task Create(DateTime dt, string line)
        {
            var SessionFilename = "Logs/" + dt.ToBinary().ToString("x") + ".txt";

            try
            {
                var path = HomeDirectory + SessionFilename;
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir))
                    System.IO.Directory.CreateDirectory(dir);

                using (var stream = File.Create(path))
                {
                    var sw = new StreamWriter(stream);
                    await sw.WriteLineAsync(line);
                    await sw.FlushAsync();
                }

            }
            catch
            {
                // We're too deep here to do anything good with exceptions now
            }
        }

        /// <summary>
        /// Enumerate all the available logging sessions
        /// </summary>
        /// <returns>List of all logging sessions</returns>
        public IEnumerable<DateTime> Directory()
        {
            string[] files;
            files = System.IO.Directory.GetFiles(HomeDirectory + "Logs");

            var select = files.Select(TransformFileName);

            return select;
        }

        /// <summary>
        /// Turn a filename into a session id
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>

        private DateTime TransformFileName(string arg)
        {
            var log = Path.GetFileName(arg);
            var text = log.Split('.')[0];
            long binary = Convert.ToInt64(text, 16);
            DateTime dt = DateTime.FromBinary(binary);

            return dt;
        }

        /// <summary>
        /// Read the entire contents of a logging session
        /// </summary>
        /// <param name="dt">Session id</param>
        /// <returns>All logging lines in that session</returns>
        public async Task<IEnumerable<string>> ReadContents(DateTime dt)
        {
            List<string> result = new List<string>();

            var path = HomeDirectory + "Logs/" + dt.ToBinary().ToString("x") + ".txt";
            using (var stream = File.OpenRead(path))
            {
                var reader = new StreamReader(stream);
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    result.Add(line);
                }
            }

            return result;
        }
    }
}
