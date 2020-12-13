using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LoggerManager
{
    /// <summary>
    /// The basic logger of this logger manager.
    /// Use this logger to log the basic informations such as warings, exceptions or other details in the console, a file, or the output in debug mode.
    /// For example:
    /// <example>
    /// <code>
    ///     BasicLogger.Construct().UseFileLogger("yourFilename.txt").Build();  <br></br>
    ///     or <br></br>
    ///     BasicLogger.Construct&lt;BasicLoggerFactory&gt;().UseFileLogger("yourFilename.txt").Build(); 
    /// </code>
    /// </example>
    /// </summary>
    public static class BasicLogger
    {
        #region Public Properties

        public static BasicLoggerFactory Factory { get; private set; }

        #endregion

        #region Public Methods       

        /// <summary>
        /// A short cut to call the logging function in the factory
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The level of the message being logged</param>
        /// <param name="origin">The method/function this message was logged in</param>
        /// <param name="filePath">The code filename that this message was logged from</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from</param>
        public static void Log(
            string message,
            LogLevel level = LogLevel.Informative,
            [CallerMemberName] string origin = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            // Log
            Factory.Log(message, level, origin, filePath, lineNumber);
            IoC.Logger.Log($"Message logged: {message}");
        }

        #endregion

        #region Extension Methods

        /// <summary>
        /// A defaul construction method that construct a <see cref="BasicLoggerFactory"/>
        /// NOTE: This should be set by the consuming application at the very start of the program. <br></br>
        /// </summary>
        /// <returns></returns>
        public static BasicLoggerFactory Construct()
        {
            Factory = new BasicLoggerFactory();

            return Factory;
        }

        /// <summary>
        /// The initial call to setting up and using the BasicLogger.
        /// NOTE: This should be set by the consuming application at the very start of the program. <br></br>
        /// For example:
        /// <example>
        /// <code>
        ///     BasicLogger.Construct().UseFileLogger("yourFilename.txt").Build();  <br></br>
        ///     or <br></br>
        ///     BasicLogger.Construct&lt;BasicLoggerFactory&gt;().UseFileLogger("yourFilename.txt").Build(); 
        /// </code>
        /// </example>
        /// </summary>
        public static BasicLoggerFactory Construct<T>()
            where T : BasicLoggerFactory, new()
        {
            Factory = new T();

            return Factory;
        }

        /// <summary>
        /// An extension method of BasicLoggerFactory. Use this function to build the factory after constructing all the loggers that are going to be used. <br></br>
        /// It comes with a <see cref="DebugLogger"/> by default. Current possible loggers are: <br></br>
        /// <see cref="DebugLogger"/> : Log in the output when build the application in debug mode.
        /// <see cref="ConsoleLogger"/> : Log in the console.
        /// <see cref="FileLogger"/> : Log in a file.
        /// </summary>
        /// <param name="factory"></param>
        public static void Build(this BasicLoggerFactory factory)
        {
            // Build the factory
            factory.Build();
        }

        #endregion
    }

    /// <summary>
    /// The result logger of this logger manager. Use this logger to log the result of a task. 
    /// For example:<br></br>
    /// <example>
    /// <code>
    ///     ResultLogger&lt;YourResultDetailClass&gt;.Construct().UseJsonResultLogger("yourResultFilename").Build();  <br></br>
    /// </code>
    /// </example>
    /// </summary>
    public static class ResultLogger<TClass>
        where TClass : class, new()
    {
        #region Public Properties

        /// <summary>
        /// The factory that handles all the work
        /// </summary>
        public static ResultLoggerFactory<TClass> ResultFactory { get; private set; }

        /// <summary>
        ///  A shortcut property to access the result class instance in the result factory.
        /// </summary>
        public static TClass Instance
        {
            get => ResultFactory.ResultClassInstance;
            set
            {
                Instance = value;
                ResultFactory.ResultClassInstance = value;
            }
        }

        #endregion

        #region Public Events

        /// <summary>
        ///  A shortcut that catch the errors sent by the factory.
        /// </summary>
        public static Action<(object sender, Exception exception)> ErrorOccurs = (detail) => { };

        private static void ResultFactory_ErrorOccurs((object sender, Exception exception) obj)
        {
            ErrorOccurs.Invoke(obj);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// A shortcut to call the save function in the ResultFactory
        /// </summary>
        public static void Save()
        {
            ResultFactory.Save();
        }

        /// <summary>
        /// A shortcut to call the GetFilenames method in the ResultFactory
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFilenames()
        {
            return ResultFactory.GetFilenames();
        }

        #endregion

        #region Extention Methods

        /// <summary>
        /// To construct the factory. 
        /// NOTE: This should be set by the consuming application at the very start of the program. <br></br>
        /// </summary>
        /// <param name="filename">The filename of the result file. (path included) <br></br>
        /// NOTE:  Do not include the extension filename.</param>
        /// <returns></returns>
        public static ResultLoggerFactory<TClass> Construct(string filename)
        {
            // New up a factory
            ResultFactory = new ResultLoggerFactory<TClass>(new TClass(), filename);

            // Register the events to provide a shortcut
            ResultFactory.ErrorOccurs += ResultFactory_ErrorOccurs;

            // Chain the method
            return ResultFactory;
        }

        #endregion
    }
}
