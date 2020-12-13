using System;
using System.Linq;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// A basic logger that allows you to pass in a customized logging method and run it asynchonously.
    /// <br></br><br></br>
    /// <strong>NOTE: Handle the UI-related function carefully. You'll need to jump back to UI thread if any.</strong>
    /// </summary>
    public class CustomFunctionAsyncLogger : IBasicLogger
    {
        #region Private Properties

        /// <summary>
        /// Store the action.
        /// </summary>
        private Action<string> LogAction = (s) => { };

        #endregion

        #region Constructor

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="logAction">The action to log. It takes a string as an parameter.</param>
        public CustomFunctionAsyncLogger(Action<string> logAction)
        {
            LogAction = logAction;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Log function
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="level">The level of the message.</param>
        public void Log(string message, LogLevel level)
        {
            // Log the full message (with message origin) for error
            if (level == LogLevel.Error)
                LogAsync(message);
            else
            {
                // Else, extract the message origin 
                var msg = message.Split('[').ToList();
                if (msg.Count > 1) msg.RemoveAt(msg.Count - 1);
                LogAsync(string.Join("", msg));
            }            
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Run the action asynchronously.
        /// </summary>
        /// <param name="msg"></param>
        private async void LogAsync(string msg)
        {
            // Lock the task
            await AsyncAwaiter.AwaitAsync(nameof(CustomFunctionAsyncLogger), async () =>
            {
                // Run the synchronous task
                await IoC.Task.Run(() =>
                {
                    // Run the action
                    LogAction(msg);
                });
            });
        }

        #endregion
    }
}
