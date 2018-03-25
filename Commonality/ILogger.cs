﻿using System;
using System.Threading.Tasks;

namespace Commonality
{
    /// <summary>
    /// Defines a platform-dependent service to log events and errors. Typically this is backed
    /// by a server-side instrumentation service, but it can also be used to capture all events
    /// locally.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Begin the logging session. Call the once when the app starts
        /// </summary>
        Task StartSession();

        /// <summary>
        /// Log an event immediately
        /// </summary>
        /// <param name="message">Descriptive message of what's goig on. Usually short</param>
        /// <param name="parameters">Additional parameters, usually 'key=value'</param>
        void LogEvent(string message, params string[] parameters);

        /// <summary>
        /// Log an event asynchronously
        /// </summary>
        /// <param name="message">Descriptive message of what's goig on. Usually short</param>
        /// <param name="parameters">Additional parameters, usually 'key=value'</param>
        /// <returns>Awaitable task</returns>
        Task LogEventAsync(string message, params string[] parameters);

        /// <summary>
        /// Report an exception
        /// </summary>
        /// <remarks>
        /// Expects a short tag in ex.Source, which some backend services will use for aggregation
        /// </remarks>
        /// <param name="ex">Exception to report</param>
        void LogError(Exception ex);

        /// <summary>
        /// Report an exception asynchronously
        /// </summary>
        /// <remarks>
        /// Expects a short tag in ex.Source, which some backend services will use for aggregation
        /// </remarks>
        /// <param name="ex">Exception to report</param>
        Task LogErrorAsync(Exception ex);

        /// <summary>
        /// Report an exception
        /// </summary>
        /// <remarks>
        /// DEPRECATED! Use LogError now, and put the key in ex.Source
        /// </remarks>
        /// <param name="key">Unique key to identify where in the app the exception was thrown</param>
        /// <param name="ex">Exception to report</param>
        void Error(string key, Exception ex);

        /// <summary>
        /// Log an informative message
        /// </summary>
        /// <remarks>
        /// Generally, we expect that these messages are not sent to server-side
        /// instrumentation, but are instead just stored in local logs.
        /// </remarks>
        /// <param name="message">Descriptive message of what's goig on. Usually detailed</param>
        void LogInfo(string message);

        /// <summary>
        /// Log an informative message asynchronously
        /// </summary>
        /// <remarks>
        /// Generally, we expect that these messages are not sent to server-side
        /// instrumentation, but are instead just stored in local logs.
        /// </remarks>
        /// <param name="message">Descriptive message of what's goig on. Usually detailed</param>
        Task LogInfoAsync(string message);
    }
}
