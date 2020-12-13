using System;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// Logs to the debug file for this library
    /// </summary>
    public class InternalFileLogger : IBasicLogger
    {
        #region Private Properties

        private InternalFileManager debugFileManager;

        #endregion

        #region Public Properties

        /// <summary>
        /// The path to write the log file to
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// If true, logs the current time with each message
        /// </summary>
        public bool LogTime { get; set; } = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="filePath">The path to log to</param>
        public InternalFileLogger()
        {
            debugFileManager = new InternalFileManager();
        }

        #endregion

        #region Logger Methods

        /// <summary>
        /// Log it.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="level">The level of the message. <see cref="LogLevel"/></param>
        public void Log(string message, LogLevel level)
        {
            // Get current time
            var currentTime = DateTimeOffset.Now.ToString("yyyy-MM-dd hh:mm:ss");

            // Prepend the time to the log if desired
            var timeLogString = LogTime ? $"[{ currentTime}] " : "";

            // Write the message
            debugFileManager.WriteTextToFileAsync($"{timeLogString}{message}{Environment.NewLine}");
        }

        #endregion
    }
}
