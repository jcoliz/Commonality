using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MyFileSystem = System.IO;

namespace Commonality
{
    /// <summary>
    /// This is a simple logger which just logs to the local file system. No cloud analytics here.
    /// </summary>

    public class FileSystemLogger: ILogger
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="homedir">Directory where on the filesystem to store the logs</param>
        public FileSystemLogger(string homedir = null)
        {
#if IO_ABSTRACTRIONS
            MyFileSystem = new FileSystem();
#endif
            if (!string.IsNullOrEmpty(homedir))
                HomeDirectory = homedir;
        }

#if IO_ABSTRACTRIONS
        /// <summary>
        /// Constructor, with an IFileSystem for testing
        /// </summary>
        /// <param name="homedir">Directory where on the filesystem to store the logs</param>
        public FileSystemLogger(IFileSystem fileSystem)
        {
            MyFileSystem = fileSystem;
            try
            {
                HomeDirectory = MyFileSystem.Directory.GetCurrentDirectory();
            }
            catch
            {
                // Ignore filesystem errors
            }
        }
#endif

        /// <summary>
        /// Report an exception
        /// </summary>
        /// <param name="key">Unique key to identify where in the app the exception was thrown</param>
        /// <param name="ex">Exception to report</param>
        public void Error(string key, Exception ex)
        {
            ExternalSemaphore.Wait();
            var ignore = ErrorAsync(key,ex);
        }

        public async Task ErrorAsync(string key, Exception ex)
        {
            var list = new List<string>();

            list.Add($"Error: {key}/{ex.GetType().ToString()}");
            if ( ! string.IsNullOrEmpty( ex.StackTrace ) )
                list.Add($", Stack = {ex.StackTrace}");
            if ( ! string.IsNullOrEmpty(ex.Source) )
                list.Add($", Source = {ex.Source}");
            Exception e = ex;
            while (e != null)
            {
                list.Add($", Message = {e.GetType().ToString()} {e.Message}");
                e = e.InnerException;
            }
            await Log(list);

            if (ExternalSemaphore.CurrentCount == 0)
                ExternalSemaphore.Release();
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
            ExternalSemaphore.Wait();
            var ignore = LogErrorAsync(ex);
        }

        /// <summary>
        /// Report an exception asynchronously
        /// </summary>
        /// <remarks>
        /// Expects a short tag in ex.Source, which some backend services will use for aggregation
        /// </remarks>
        /// <param name="ex">Exception to report</param>
        public async Task LogErrorAsync(Exception ex)
        {
            var list = new List<string>();

            list.Add($"Error: {ex.Source}/{ex.GetType().ToString()}");
            if (!string.IsNullOrEmpty(ex.StackTrace))
                list.Add($", Stack = {ex.StackTrace}");
            Exception e = ex;
            while (e != null)
            {
                list.Add($", Message = {e.GetType().ToString()} {e.Message}");
                e = e.InnerException;
            }
            await Log(list);

            if (ExternalSemaphore.CurrentCount == 0)
                ExternalSemaphore.Release();
        }

        /// <summary>
        /// Log an event immediately
        /// </summary>
        /// <param name="message">Descriptive message of what's goig on. Usually short</param>
        /// <param name="parameters">Additional parameters, usually 'key=value'</param>
        public void LogEvent(string message, params string[] parameters)
        {
            ExternalSemaphore.Wait();
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
            var list = new List<string>(parameters.Select(x=>$", {x}"));
            list.Insert(0, $"Event: {message}");
            await Log(list);

            if (ExternalSemaphore.CurrentCount == 0)
                ExternalSemaphore.Release();
        }

        /// <summary>
        /// Log an informative message
        /// </summary>
        /// <param name="message">Descriptive message of what's goig on. Usually detailed</param>
        public void LogInfo(string message)
        {
            ExternalSemaphore.Wait();
            var ignore = LogInfoAsync(message);
        }

        public async Task LogInfoAsync(string message)
        {
            await Log(new[] { $"FYI: {message}" });

            if (ExternalSemaphore.CurrentCount == 0)
                ExternalSemaphore.Release();
        }

        /// <summary>
        /// Begin the logging session. Call the once when the app starts
        /// </summary>
        public async Task StartSession()
        {
            await Log(new[] { "Started" });
        }

        /// <summary>
        /// Await until the current log has written
        /// </summary>
        /// <remarks>
        /// Useful for unit testing the non-async methods
        /// </remarks>
        /// <returns></returns>
        public async Task Wait()
        {
            await ExternalSemaphore.WaitAsync();
            ExternalSemaphore.Release();
        }

#pragma warning disable 1998
        /// <summary>
        /// Retrieve listing of all the log files
        /// </summary>
        /// <returns>List of all the log files</returns>
        public static async Task<IEnumerable<string>> GetLogs()
        {
            string[] files;
            files = MyFileSystem.Directory.GetFiles(HomeDirectory + "Logs");

            var select = files.Select(x => MyFileSystem.Path.GetFileName(x));
            return select;
        }

        /// <summary>
        /// Get a single log for reading
        /// </summary>
        /// <remarks>
        /// It's cumpersonme and suboptimal to take a datetime here but return a filename in
        /// GetLogs. This should be improved. See example below for how to convert.
        /// </remarks>
        /// <param name="dt">Datetime which corresponds to a filename returned from GetLogs</param>
        /// <example>
        /// var logs = FileSystemLogger.GetLogs();
        /// var log = logs[0];
        /// var text = log.Split('.')[0];
        /// long binary = Convert.ToInt64(text, 16);
        /// DateTime dt = DateTime.FromBinary(binary);
        /// var stream = FileSystemLogger.OpenLogForRead(dt);
        /// </example>
        /// <returns>Stream to read a single log file</returns>
        public static async Task<Stream> OpenLogForRead(DateTime dt)
        {
            var path = HomeDirectory + "Logs/" + dt.ToBinary().ToString("x") + ".txt";

            // QUIRKS
            path = path.Replace('/', '\\');
            
            return MyFileSystem.File.OpenRead(path);
        }
#pragma warning restore 1998

        /// <summary>
        /// Filename of the session log file
        /// </summary>
        private string SessionFilename;

        /// <summary>
        /// Semaphore used to control access to the logs. Only one thread can write to logs at once!
        /// </summary>
        private SemaphoreSlim Semaphore = new SemaphoreSlim(1);

        /// <summary>
        /// Semaphore used to control access to syncrhonous loggers. In case you want to 'await'
        /// a synchronous logging method
        /// </summary>
        private SemaphoreSlim ExternalSemaphore = new SemaphoreSlim(1);

        /// <summary>
        /// Internal method to actually write to the logs
        /// </summary>
        /// <param name="lines">Text lines to be written</param>
        /// <returns>Awaitable task</returns>
        private async Task Log(IEnumerable<string> lines)
        {
            await Semaphore.WaitAsync();

            if (SessionFilename == null)
            {
                SessionFilename = "Logs/" + Time.ToBinary().ToString("x") + ".txt";

                try
                {
                    var path = HomeDirectory + SessionFilename;
                    var dir = MyFileSystem.Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(dir))
                        MyFileSystem.Directory.CreateDirectory(dir);

                    // QUIRKS
                    path = path.Replace('/', '\\');

                    using (var stream = MyFileSystem.File.Create(path))
                    {
                        var sw = new StreamWriter(stream);
                        await sw.WriteLineAsync(Time.ToString("u") + " Created");
                        await sw.FlushAsync();
                    }

                }
                catch
                {
                    // We're too deep here to do anything good with exceptions now
                }

            }

            try
            {
                var path = HomeDirectory + SessionFilename;
                var dir = MyFileSystem.Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir))
                    MyFileSystem.Directory.CreateDirectory(dir);

                // QUIRKS
                path = path.Replace('/', '\\');

                using (var sw = MyFileSystem.File.AppendText(path))
                {
                    foreach (var line in lines)
                        await sw.WriteLineAsync(FormattedLine(line));
                    await sw.FlushAsync();
                }

            }
            catch (Exception)
            {
                // All we can do is swallow exceptions here. We are deep INSIDE our exception
                // logging mechanism!
            }
            finally
            {
                Semaphore.Release();
            }
        }

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
        protected DateTime Time => Service.TryGet<IClock>()?.Now ?? DateTime.Now;

        /// <summary>
        /// THe location on the filesystem where logs are to be written
        /// </summary>
        private static string HomeDirectory = string.Empty;

#if IO_ABSTRACTRIONS
        /// <summary>
        /// Which filesystem we are using. This can be overriden for testing
        /// </summary>
        private static IFileSystem MyFileSystem;
#endif

    }
}
