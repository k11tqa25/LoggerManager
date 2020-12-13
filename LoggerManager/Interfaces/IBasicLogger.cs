using System.Runtime.CompilerServices;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// A logger that will handle log messages from a <see cref="IBasicLoggerFactory"/>
    /// </summary>
    public interface IBasicLogger
    {
        /// <summary>
        /// Handles the logged message being passed in
        /// </summary>
        /// <param name="message">The message being log</param>
        /// <param name="level">The level of the log message</param>
        void Log(string message, LogLevel level);
    }
}
