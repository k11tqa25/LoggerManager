using System;
using System.Runtime.CompilerServices;

namespace LoggerManagerLibrary
{

    /// <summary>
    /// Holds a bunch of loggers to log messages for the user
    /// </summary>
    public interface IBasicLoggerFactory
    {
        #region Events

        /// <summary>
        /// Fires whenever a new log arrives
        /// </summary>
        event Action<(string Message, LogLevel Level)> NewLog;

        /// <summary>
        /// Fires whenever a error occurs
        /// </summary>
        event Action<(object sender, Exception exception)> ErrorOccurs;

        #endregion

        #region Methods

        /// <summary>
        /// Build the factory. This could function as a constructor.
        /// </summary>
        void Build();

        /// <summary>
        /// Logs the specific message to all loggers in this factory
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The level of the message being logged</param>
        /// <param name="origin">The method/function this message was logged in</param>
        /// <param name="filePath">The code filename that this message was logged from</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from</param>
        void Log(string message, LogLevel level = LogLevel.Informative, [CallerMemberName] string origin = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0);

        #endregion
    }
}
