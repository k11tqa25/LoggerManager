using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The factory that log all the basic informations, warnings, errors.
    /// </summary>
    public class BasicLoggerFactory : BaseBasicLoggerFactory
    {
        #region Protected Methods

        /// <summary>
        /// The list of loggers in this factory
        /// </summary>
        protected List<IBasicLogger> mLoggers = new List<IBasicLogger>();

        /// <summary>
        /// A lock for the logger list to keep it thread-safe
        /// </summary>
        protected object mLoggersLock = new object();

        #endregion

        #region Constructor

        /// <summary>
        /// The constructor
        /// </summary>
        public BasicLoggerFactory()
        {
            FactoryName = nameof(BasicLoggerFactory);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Build thils factory.
        /// </summary>
        public override void Build()
        {
            // Add debug logger
            AddLogger(new DebugLogger());
        }

        /// <summary>
        /// Add a console logger to the logger list.
        /// </summary>
        /// <returns></returns>
        public BasicLoggerFactory UseConsoleLogger()
        {
            // Add a console logger for user
            AddLogger(new ConsoleLogger());

            // Chain the method
            return this;

        }
        
        /// <summary>
        /// Add a file logger to the logger list.
        /// </summary>
        /// <param name="filename">The filename to save the file.</param>
        /// <returns></returns>
        public BasicLoggerFactory UseFileLogger(string filename)
        {
            // Add a file logger for user
            AddLogger(new FileLogger(filename));

            // Chain the method
            return this;
        }

        /// <summary>
        /// This method can only be used in this library. This is specifically used for saving the debug logger file for this library.
        /// </summary>
        /// <returns></returns>
        internal BasicLoggerFactory UseDebugLogger()
        {
            // Add a file logger for user
            AddLogger(new InternalFileLogger());

            // Chain the method
            return this;
        }

        /// <summary>
        /// Add a custom function async logger to the logger list. Use this to handle any customized function to log your message.
        /// <br></br><br></br>
        /// <strong>NOTE: Handle the UI-related function carefully. You'll need to jump back to UI thread if any.</strong>
        /// </summary>
        /// <param name="action">The custom action.</param>
        /// <returns></returns>
        public BasicLoggerFactory UseCustomFunctionAsyncLogger(Action<string> action)
        {
            // Add a custom logger for user
            AddLogger(new CustomFunctionAsyncLogger(action));

            // Chain the method
            return this;
        }

        /// <summary>
        /// Logs the specific message to all loggers in this factory
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The level of the message being logged</param>
        /// <param name="origin">The method/function this message was logged in</param>
        /// <param name="filePath">The code filename that this message was logged from</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from</param>
        public override void Log(
            string message,
            LogLevel level = LogLevel.Informative,
            [CallerMemberName] string origin = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            // If we should not log the message as the level is too low...
            if ((int)level < (int)LogOutputLevel)
                return;

            // Append the information of where the log originated from...
           message = $"{message} [{Path.GetFileName(filePath)} > {origin}() > Line {lineNumber}]";

            // Log the list so it is thread-safe
            lock (mLoggersLock)
            {
                // Log to all loggers
                mLoggers.ForEach(logger => logger.Log(message, level));
            }

            // Inform listeners
            NewLog?.Invoke((message, level));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// A helper function to handle all the exceptions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        private void HandleExceptions(object sender, Exception ex)
        {
            IoC.Logger.Log($"Error sent from{sender}:\r\n{ex.Message}");
            ErrorOccurs.Invoke((sender, ex));
        }

        /// <summary>
        /// Adds the specific logger to this factory
        /// </summary>
        /// <param name="logger">The logger</param>
        private void AddLogger(IBasicLogger logger)
        {
            try
            {
                // Log the list so it is thread-safe
                lock (mLoggersLock)
                {
                    // If the logger is not already in the list...
                    if (!mLoggers.Contains(logger))
                        // Add the logger to the list
                        mLoggers.Add(logger);
                }
            }
            catch(Exception ex)
            {
                HandleExceptions(this, ex);
            }
        }

        /// <summary>
        /// Removes the specified logger from this factory
        /// </summary>
        /// <param name="logger">The logger</param>
        private void RemoveLogger(IBasicLogger logger)
        {
            // Log the list so it is thread-safe
            lock (mLoggersLock)
            {
                // If the logger is in the list...
                if (mLoggers.Contains(logger))
                    // Remove the logger from the list
                    mLoggers.Remove(logger);
            }
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Fires whenever a new log arrives
        /// </summary>
        public override event Action<(string Message, LogLevel Level)> NewLog = (details) => { };

        /// <summary>
        /// Fires whenever an error occurs
        /// </summary>
        public override event Action<(object sender, Exception exception)> ErrorOccurs = (details) => { };

        #endregion
    }
}
