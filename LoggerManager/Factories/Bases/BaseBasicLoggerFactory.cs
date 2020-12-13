using System;
using System.Runtime.CompilerServices;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The base of the <see cref="BasicLoggerFactory"/>
    /// </summary>
    public abstract class BaseBasicLoggerFactory : AbstractLoggerFactory, IBasicLoggerFactory
    {
        #region Properties

        /// <summary>
        /// The level of logging to output
        /// </summary>
        public LogOutputLevel LogOutputLevel { get; set; }

        #endregion

        /// <summary>
        /// Fires when a new log is added
        /// </summary>
        public virtual event Action<(string Message, LogLevel Level)> NewLog = (details) => { };

        /// <summary>
        /// Fires when an error occurs
        /// </summary>
        public virtual event Action<(object sender, Exception exception)> ErrorOccurs = (details) => { };

        /// <summary>
        /// Build this logger
        /// </summary>
        public virtual void Build()
        {
        }

        /// <summary>
        /// Log it.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="level">The level of the message. <see cref="LogLevel"/></param>
        /// <param name="origin">The method/function this message was logged in</param>
        /// <param name="filePath">The code filename that this message was logged from</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from</param>
        public virtual void Log(string message, LogLevel level = LogLevel.Informative, [CallerMemberName] string origin = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
        }
    }
}
