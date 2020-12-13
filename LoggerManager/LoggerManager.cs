using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The configuration of this <see cref="LoggerManagerLibrary"/>
    /// </summary>
    public static class LoggerManagerConfiguration
    {
        /// <summary>
        ///  Save the debug file for this <see cref="LoggerManagerLibrary"/>. This library will save a "LoggerManager.log" in the application path by default.<br></br>
        ///  Set to false if you don't want to save the debug file.
        /// </summary>
        public static bool SaveDebugFile { get; set; } = true;

        /// <summary>
        /// Override the existing debug file ("LoggerManager.log"). <br></br>
        /// It'll delete and create a new one if this value is set to true, or the debug log will append to the existing one if set to false.<br></br><br></br>
        /// The default is false.
        /// </summary>
        public static bool OverrideDebugFile { get; set; } = false;
    }

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

        /// <summary>
        /// The Factory for the <see cref="BasicLoggerFactory"/>
        /// </summary>
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
            try
            {
                // Log
                Factory.Log(message, level, origin, filePath, lineNumber);
                IoC.Logger.Log($"Message logged: {message}");
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(BasicLogger.Log), ex);
            }
        }

        #endregion

        #region Public Events

        /// <summary>
        ///  A shortcut that catch the errors sent by the factory.
        /// </summary>
        public static event Action<(object sender, Exception exception)> ErrorOccurs = (detail) => { };

        private static void Factory_ErrorOccrus((object sender, Exception exception) obj)
        {
            HandleExceptions(obj.sender, obj.exception);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// A helper function to handle all the exceptions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        private static void HandleExceptions(object sender, Exception ex)
        {
            IoC.Logger.Log($"Error sent from{sender}:\r\n{ex.Message}");
            ErrorOccurs.Invoke((sender, ex));
        }

        #endregion

        #region Builder Methods

        /// <summary>
        /// A defaul construction method that construct a <see cref="BasicLoggerFactory"/>
        /// NOTE: This should be set by the consuming application at the very start of the program. <br></br>
        /// </summary>
        /// <returns></returns>
        public static BasicLoggerFactory Construct()
        {
            try
            {
                // In case some use this function to recreate a factory. Need to avoid memory leak.
                if (Factory != null)
                    Factory.ErrorOccurs -= Factory_ErrorOccrus;

                Factory = new BasicLoggerFactory();

                Factory.ErrorOccurs += Factory_ErrorOccrus;

                return Factory;
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(BasicLogger.Construct), ex);
                return Factory;
            }
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
            try
            {
                // In case some use this function to recreate a factory. Need to avoid memory leak.
                if (Factory != null)
                    Factory.ErrorOccurs -= Factory_ErrorOccrus;

                Factory = new T();

                Factory.ErrorOccurs += Factory_ErrorOccrus;

                return Factory;
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(BasicLogger.Construct), ex);
                return Factory;
            }
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
            try
            {
                // Build the factory
                factory.Build();
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(BasicLogger.Build), ex);
            }
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
    /// <typeparam name="TClass">The class object that contains the structure of the result.</typeparam>
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
            get
            {
                if (ResultFactory != null) return ResultFactory.ResultClassInstance;
                else
                {
                    HandleExceptions("Get " + nameof(Instance),
                        new Exception($"The result factory of class type {typeof(TClass).Name} haven't been initialized yet. Check if you used the right class."));
                    return null;
                }
            }
            set
            {
                if (ResultFactory != null) ResultFactory.ResultClassInstance = value;
                else HandleExceptions("Set " + nameof(Instance),
                    new Exception($"The result factory of class type {typeof(TClass).Name} haven't been initialized yet. Check if you used the right class."));
            }
        }

        #endregion

        #region Public Events

        /// <summary>
        ///  A shortcut that catch the errors sent by the factory.
        /// </summary>
        public static event Action<(object sender, Exception exception)> ErrorOccurs = (detail) => { };

        private static void ResultFactory_ErrorOccurs((object sender, Exception exception) obj)
        {
            HandleExceptions(obj.sender, obj.exception);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// A shortcut to call the save function in the ResultFactory
        /// </summary>
        public static bool Save()
        {
            try
            {
                return ResultFactory.Save();
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(ResultLogger<TClass>.Save), ex);
                return false;
            }
        }

        /// <summary>
        /// A shortcut to call the GetFilenames method in the ResultFactory
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFilenames()
        {
            try
            {
                return ResultFactory.GetFilenames();
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(ResultLogger<TClass>.GetFilenames), ex);
                return null;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// A helper function to handle all the exceptions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        private static void HandleExceptions(object sender, Exception ex)
        {
            IoC.Logger.Log($"Error sent from{sender}:\r\n{ex.Message}");
            ErrorOccurs.Invoke((sender, ex));
        }

        #endregion

        #region Builder Methods

        /// <summary>
        /// To construct the factory. 
        /// NOTE: This should be set by the consuming application at the very start of the program. <br></br>
        /// </summary>
        /// <param name="filename">The filename of the result file. (path included) <br></br>
        /// NOTE:  Do not include the extension filename.</param>
        /// <returns></returns>
        public static ResultLoggerFactory<TClass> Construct(string filename)
        {
            try
            {
                // In case some use this function to recreate a factory. Need to avoid memory leak.
                if (ResultFactory != null)
                    ResultFactory.ErrorOccurs -= ResultFactory_ErrorOccurs;

                // New up a factory
                ResultFactory = new ResultLoggerFactory<TClass>(filename);

                // Register the events to provide a shortcut
                ResultFactory.ErrorOccurs += ResultFactory_ErrorOccurs;

                // Chain the method
                return ResultFactory;
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(ResultLogger<TClass>.Construct), ex);
                return ResultFactory;
            }
        }

        #endregion
    }

    /// <summary>
    /// The defaut settings logger of this logger manager. Use this logger to setup your settings.
    /// A noticable differenct between this logger and the other two is that <br></br>
    /// in the DefualtSettingsLoggerFactory, you can only use ONE format to read / write the settings file.
    /// </summary>
    /// <typeparam name="TClass">The class object that contains the details of the settings.</typeparam>
    public static class SettingsLogger<TClass>
        where TClass : class, new()
    {
        #region Public Properties

        /// <summary>
        ///  The factory of that handles all the work
        /// </summary>
        public static SettingsLoggerFactory<TClass> SettingsLoggerFactory { get; private set; }

        /// <summary>
        ///  A shortcut property to access the default settings class instance in the settings factory.
        /// </summary>
        public static TClass Instance
        {
            get
            {
                if (SettingsLoggerFactory != null) return SettingsLoggerFactory.SettingsClassInstance;
                else
                {
                    HandleExceptions("Get " + nameof(Instance),
                        new Exception($"The settings factory of class type {typeof(TClass).Name} haven't been initialized yet. Check if you used the right class."));
                    return null;
                }
            }
            set
            {
                if (SettingsLoggerFactory != null) SettingsLoggerFactory.SettingsClassInstance = value;
                else HandleExceptions("Set " + nameof(Instance),
                    new Exception($"The settings factory of class type {typeof(TClass).Name} haven't been initialized yet. Check if you used the right class."));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///  A shortcut to save the settings
        /// </summary>
        /// <returns></returns>
        public static bool Save()
        {
            try
            {
                return SettingsLoggerFactory.Save();
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(SettingsLogger<TClass>.Save), ex);
                return false;
            }
        }

        /// <summary>
        /// A shortcut to read the settings
        /// </summary>
        /// <returns></returns>
        public static bool Read()
        {
            try
            {
                return SettingsLoggerFactory.Read();
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(SettingsLogger<TClass>.Read), ex);
                return false;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// A helper function to handle all the exceptions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        private static void HandleExceptions(object sender, Exception ex)
        {
            IoC.Logger.Log($"Error sent from{sender}:\r\n{ex.Message}");
            ErrorOccurs.Invoke((sender, ex));
        }

        #endregion

        #region Public Events

        /// <summary>
        ///  A shortcut that catch the errors sent by the factory.
        /// </summary>
        public static event Action<(object sender, Exception exception)> ErrorOccurs = (detail) => { };

        private static void SettingsLoggerFactory_ErrorOccurs((object sender, Exception exception) obj)
        {
            HandleExceptions(obj.sender, obj.exception);
        }

        #endregion

        #region Builder Methods

        /// <summary>
        /// To construct the factory. 
        /// NOTE: This should be set by the consuming application at the very start of the program. <br></br>
        /// </summary>
        /// <param name="filename">The filename of the settings file. (path included) <br></br>
        /// NOTE:  Do not include the extension filename.</param>
        /// <returns></returns>
        public static SettingsLoggerFactory<TClass> Construct(string filename)
        {
            try
            {
                // In case some use this function to recreate a factory. Need to avoid memory leak.
                if(SettingsLoggerFactory != null)
                    SettingsLoggerFactory.ErrorOccurs -= SettingsLoggerFactory_ErrorOccurs;

                SettingsLoggerFactory = new SettingsLoggerFactory<TClass>(filename);

                SettingsLoggerFactory.ErrorOccurs += SettingsLoggerFactory_ErrorOccurs;

                // Chain the method
                return SettingsLoggerFactory;
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(SettingsLogger<TClass>.Construct), ex);
                return SettingsLoggerFactory;
            }
        }

        #endregion
    }

    /// <summary>
    /// Use this class if you need to have multiple basic loggers for different scenarios.
    /// </summary>
    public static class MultiBasicLoggers
    {
        #region Public Properties

        /// <summary>
        /// A dictionary of <see cref="BasicLoggerFactory"/>. The key is the name of the factory.
        /// </summary>
        public static Dictionary<string, BasicLoggerFactory> Factories { get; private set; }

        #endregion

        #region Public Methods       

        /// <summary>
        /// A short cut to call the logging function for a specific logger
        /// </summary>
        /// <param name="factoryName"></param>
        /// <param name="message">The message to log</param>
        /// <param name="level">The level of the message being logged</param>
        /// <param name="origin">The method/function this message was logged in</param>
        /// <param name="filePath">The code filename that this message was logged from</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from</param>
        public static void Log(
            string factoryName,
            string message,
            LogLevel level = LogLevel.Informative,
            [CallerMemberName] string origin = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            try
            {
                // Log
                Factories[factoryName].Log(message, level, origin, filePath, lineNumber);
                IoC.Logger.Log($"Message logged: {message}");
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(BasicLogger.Log), ex);
            }
        }

        /// <summary>
        /// Add and build a factory dynamically.         
        /// This is a helpful function for some scenarios where you need to add another <see cref="BasicLoggerFactory"/> in the run time. <br></br>
        /// <br></br>
        /// <strong> NOTE: Please be sure that you have already initiated the Factories by constructing one.  Or it'll throw a <see cref="NullReferenceException"/></strong>
        /// </summary>
        /// <param name="factoryName">The name of the factory</param>
        /// <param name="factory">A <see cref="BasicLoggerFactory"/></param>
        public static void AddFactory(string factoryName, BasicLoggerFactory factory)
        {
            if (Factories == null) throw new NullReferenceException("The Factories dictionary haven't been constructed yet. Make sure you have call the construct function first.");
            try
            {
                Factories[factoryName] = factory;
                factory.FactoryName = factoryName;

                IoC.Logger.Log($"Add factory {factory.FactoryName} to Factories dynamically.");
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(MultiBasicLoggers.AddFactory), ex);
            }
        }

        /// <summary>
        /// Remove a factory dynamically. 
        /// This is a helpful function for some scenarios where you need to remove a <see cref="BasicLoggerFactory"/> in the run time. <br></br>
        /// <br></br>
        /// <strong> NOTE: Please be sure that you have already initiated the Factories by constructing one.  Or it'll throw a <see cref="NullReferenceException"/></strong>
        /// </summary>
        /// <param name="factoryName">The name of the factory. It'll be the class you passed in to <see cref="BasicLoggerFactory"/> by default.</param>
        public static void RemoveFactory(string factoryName)
        {
            if (Factories == null) throw new NullReferenceException("The Factories dictionary haven't been constructed yet. Make sure you have call the construct function first.");
            try
            {
                if (Factories.ContainsKey(factoryName))
                {
                    Factories.Remove(factoryName);
                    IoC.Logger.Log($"Remove factory {factoryName} to Factories dynamically.");
                }
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(MultiBasicLoggers.RemoveFactory), ex);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// A helper function to handle all the exceptions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        private static void HandleExceptions(object sender, Exception ex)
        {
            IoC.Logger.Log($"Error sent from {sender}:\r\n{ex.Message}");
            ErrorOccurs.Invoke((sender, ex));
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Fires when an error occurs.
        /// </summary>
        public static event Action<(object sender, Exception exception)> ErrorOccurs = (detail) => { };

        private static void MultiBasicLoggers_ErrorOccurs((object sender, Exception exception) obj)
        {
            HandleExceptions(obj.sender, obj.exception);
        }

        #endregion

        #region Builder Methods

        /// <summary>
        ///  Construct the Factories.<br></br>
        ///  <br></br>
        /// <strong>NOTE: This needs to be called first before adding any factory.</strong>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, BasicLoggerFactory> Construct()
        {
            // In case some use this function to recreate a factory. Need to avoid memory leak.
            if (Factories != null) foreach (var f in Factories.Keys) Factories[f].ErrorOccurs -= MultiBasicLoggers_ErrorOccurs;

            Factories = new Dictionary<string, BasicLoggerFactory>();

            return Factories;
        }

        /// <summary>
        /// Add a result factory to the dictionary after constructing the Factories.
        /// New up a <see cref="ResultLoggerFactory{TClass}"/>, use the format you need, but do not need to build.
        /// </summary>
        /// <param name="dictionary">The dictionary to add (Extention). </param>
        /// <param name="name">Give a name for the factory.</param>
        /// <param name="factory">The factory to add.</param>
        /// <returns></returns>
        public static Dictionary<string, BasicLoggerFactory> AddFactory(this Dictionary<string, BasicLoggerFactory> dictionary, string name, BasicLoggerFactory factory)
        {
            try
            {
                // Need to setup the factory name for diffenet basic factories.
                factory.FactoryName = name;
                IoC.Logger.Log($"Add factory {factory.FactoryName} to Factories.");
                dictionary.Add(factory.FactoryName, factory);

                return dictionary;
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(MultiBasicLoggers.AddFactory), ex);
                return dictionary;
            }
        }

        /// <summary>
        /// Build all the factorys in the Factories
        /// </summary>
        /// <param name="factory"></param>
        public static void Build(this Dictionary<string, BasicLoggerFactory> factory)
        {
            foreach (var name in factory.Keys)
            {
                try
                {
                    // Build the factory
                    factory[name].Build();

                    // Hook up the event
                    factory[name].ErrorOccurs += MultiBasicLoggers_ErrorOccurs; ;
                }
                catch (Exception ex)
                {
                    HandleExceptions(nameof(MultiBasicLoggers.Build), ex);
                }
            }
        }


        #endregion
    }

    /// <summary>
    /// Use this class if you need to save multiple results with different contents.
    /// </summary>
    public static class MultiResultLoggers
    {
        #region Public Properties

        /// <summary>
        /// A dictionary of <see cref="ResultLoggerFactory{TClass}"/>
        /// The key is meant to be the class name of the result class type.
        /// </summary>
        public static Dictionary<string, BaseResultLoggerFactory> Factories { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// A shortcut method to save the file for a specific factory
        /// </summary>
        /// <param name="factoryName">The name of the factory that needs to perform this function</param>
        /// <returns></returns>
        public static bool Save(string factoryName)
        {
            if (!Factories.ContainsKey(factoryName))
            {
                HandleExceptions(Factories, new Exception($"{factoryName} doesn't exist in the Fatories."));
                return false;
            }
            try
            {
                return Factories[factoryName].Save();
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(MultiResultLoggers.Save), ex);
                return false;
            }
        }

        /// <summary>
        /// A shortcut method to save all the factories in the Factories
        /// </summary>
        /// <returns></returns>
        public static bool SaveAll()
        {
            try
            {
                bool ok = true;
                foreach (var key in Factories.Keys)
                {
                    bool ret = Factories[key].Save();
                    ok &= ret;
                    if (!ret)
                        // If not saved successfully
                        HandleExceptions(Factories[key], new Exception($"Fail to save {Factories[key].Filename}"));
                }
                return ok;
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(MultiResultLoggers.SaveAll), ex);
                return false;
            }
        }

        /// <summary>
        /// A shortcut to get all the factory names in the Factories
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllFactoryNames()
        {
            if (Factories == null) return null;
            if (Factories.Count == 0) return new List<string>() { "" };
            try
            {
                return Factories.Select(v => v.Key).ToList();
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(MultiResultLoggers.GetAllFactoryNames), ex);
                return null;
            }
        }

        /// <summary>
        /// A shortcut to get the instance of the specific factory
        /// </summary>
        /// <typeparam name="TClass">The class type of that specific factory</typeparam>
        /// <returns></returns>
        public static TClass Instance<TClass>()
            where TClass : class, new()
        {
            string name = typeof(TClass).Name;
            if (!Factories.ContainsKey(name))
            {
                HandleExceptions(Factories, new Exception($"{name} doesn't exist in the Fatories."));
                return default;
            }

            try
            {
                return (Factories[name] as ResultLoggerFactory<TClass>).ResultClassInstance;
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(MultiResultLoggers.Instance), ex);
                return null;
            }
        }

        /// <summary>
        /// Add and build a factory dynamically. <br></br>
        /// This is a helpful function for some scenarios where you need to add another <see cref="ResultLoggerFactory{TClass}"/> in the run time. <br></br>
        /// <br></br>
        /// <strong> NOTE: Please be sure that you have already initiated the Factories by constructing one.  Or it'll throw a <see cref="NullReferenceException"/></strong>
        /// </summary>
        public static void AddFactory(BaseResultLoggerFactory factory)
        {
            if (Factories == null) throw new NullReferenceException("The Factories dictionary haven't been constructed yet. Make sure you have call the construct function first.");
            try
            {
                Factories[factory.FactoryName] = factory;
                IoC.Logger.Log($"Add factory {factory.FactoryName} to Factories dynamically.");
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(MultiResultLoggers.AddFactory), ex);
            }
        }

        /// <summary>
        /// Remove a factory dynamically. 
        /// This is a helpful function for some scenarios where you need to remove a <see cref="ResultLoggerFactory{TClass}"/> in the run time. <br></br>
        /// <br></br>
        /// <strong> NOTE: Please be sure that you have already initiated the Factories by constructing one.  Or it'll throw a <see cref="NullReferenceException"/></strong>
        /// </summary>
        /// <param name="factoryName">The name of the factory. It'll be the class you passed in to <see cref="ResultLoggerFactory{TClass}"/> by default.</param>
        public static void RemoveFactory(string factoryName)
        {
            if (Factories == null) throw new NullReferenceException("The Factories dictionary haven't been constructed yet. Make sure you have call the construct function first.");
            try
            {
                if (Factories.ContainsKey(factoryName))
                {
                    Factories.Remove(factoryName);
                    IoC.Logger.Log($"Remove factory {factoryName} to Factories dynamically.");
                }
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(MultiResultLoggers.RemoveFactory), ex);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// A helper function to handle all the exceptions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        private static void HandleExceptions(object sender, Exception ex)
        {
            IoC.Logger.Log($"Error sent from{sender}:\r\n{ex.Message}");
            ErrorOccurs.Invoke((sender, ex));
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Fires when an error occurs.
        /// </summary>
        public static event Action<(object sender, Exception exception)> ErrorOccurs = (detail) => { };

        private static void MultiResultLoggers_ErrorOccurs((object sender, Exception exception) obj)
        {
            HandleExceptions(obj.sender, obj.exception);
        }

        #endregion

        #region Builder Methods

        /// <summary>
        ///  Construct the Factories.<br></br>
        ///  <br></br>
        /// <strong>NOTE: This needs to be called first before adding any factory.</strong>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, BaseResultLoggerFactory> Construct()
        {
            // In case some use this function to recreate a factory. Need to avoid memory leak.
            if (Factories != null) foreach (var f in Factories.Keys) Factories[f].ErrorOccurs -= MultiResultLoggers_ErrorOccurs;

            Factories = new Dictionary<string, BaseResultLoggerFactory>();

            return Factories;
        }

        /// <summary>
        /// Add a result factory to the dictionary after constructing the Factories. <br></br>
        /// New up a <see cref="ResultLoggerFactory{TClass}"/> and add all the result loggers you need with builder functions.
        /// </summary>
        /// <param name="dictionary">The dictionary to add (Extention). </param>
        /// <param name="factory">The factory to add.</param>
        /// <returns></returns>
        public static Dictionary<string, BaseResultLoggerFactory> AddFactory(this Dictionary<string, BaseResultLoggerFactory> dictionary, BaseResultLoggerFactory factory)
        {
            try
            {
                IoC.Logger.Log($"Add factory {factory.FactoryName} to Factories.");
                dictionary.Add(factory.FactoryName, factory);

                return dictionary;
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(MultiSettingsLoggers.AddFactory), ex);
                return dictionary;
            }
        }

        /// <summary>
        /// Build all the factorys in the Factories
        /// </summary>
        /// <param name="factory"></param>
        public static void Build(this Dictionary<string, BaseResultLoggerFactory> factory)
        {
            foreach (var name in factory.Keys)
            {
                try
                {
                    // Build the factory
                    factory[name].Build();

                    // Hook up the event
                    factory[name].ErrorOccurs += MultiResultLoggers_ErrorOccurs;
                }
                catch(Exception ex)
                {
                    HandleExceptions(nameof(MultiSettingsLoggers.Build), ex);
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Use this class if you need to use multiple settings with different contents.
    /// </summary>
    public static class MultiSettingsLoggers
    {
        #region Public Properties

        /// <summary>
        /// A dictionary of <see cref="SettingsLoggerFactory{TClass}"/> 
        /// The key is meant to be the class name of the settings class type. <br></br>
        /// <br></br>
        /// <strong>NOTE: Construct the factory first before using this factory.</strong>
        /// </summary>
        public static Dictionary<string, BaseSettingsLoggerFactory> Factories { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// A shortcut method to save the file for a specific factory
        /// </summary>
        /// <param name="factoryName">The name of the factory that needs to perform this function</param>
        /// <returns></returns>
        public static bool Save(string factoryName)
        {
            if (!Factories.ContainsKey(factoryName))
            {
                ErrorOccurs?.Invoke((Factories, new Exception($"{factoryName} doesn't exist in the Fatories.")));
                return false;
            }
            try
            {
                return Factories[factoryName].Save();
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(MultiSettingsLoggers.Save), ex);
                return false;
            }
        }

        /// <summary>
        /// A shortcut method to save all the factories in the Factories
        /// </summary>
        /// <returns></returns>
        public static bool SaveAll()
        {
            try
            {
                bool ok = true;
                foreach (var key in Factories.Keys)
                {
                    bool ret = Factories[key].Save();
                    ok &= ret;
                    if (!ret)
                        // If not saved successfully
                        HandleExceptions(Factories[key], new Exception($"Fail to save {Factories[key].Filename}"));
                }
                return ok;
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(MultiSettingsLoggers.SaveAll), ex);
                return false;
            }
        }

        /// <summary>
        /// A shortcut method to read the file for a specific factory
        /// </summary>
        /// <param name="factoryName">The name of the factory that needs to perform this function</param>
        /// <returns></returns>
        public static bool Read(string factoryName)
        {
            if (!Factories.ContainsKey(factoryName))
            {
                HandleExceptions(Factories, new Exception($"{factoryName} doesn't exist in the Fatories."));
                return false;
            }
            try
            {
                return Factories[factoryName].Read();
            }
            catch(Exception ex)
            {
                HandleExceptions(nameof(MultiSettingsLoggers.Read), ex);
                return false;
            }
        }

        /// <summary>
        /// A shortcut to read all the factories in the Factories
        /// </summary>
        /// <returns></returns>
        public static bool ReadAll()
        {
            try
            {
                bool ok = true;
                foreach (var key in Factories.Keys)
                {
                    bool ret = Factories[key].Read();
                    ok &= ret;
                    if (!ret)
                        // If not saved successfully
                        HandleExceptions(Factories[key], new Exception($"Fail to save {Factories[key].Filename}"));
                }
                return ok;
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(MultiSettingsLoggers.ReadAll), ex);
                return false;
            }
        }

        /// <summary>
        /// A shortcut to get all the factory names in the Factories
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllFactoryNames()
        {
            if (Factories == null) return null;
            if (Factories.Count == 0) return new List<string>() { "" };
            try
            {
                return Factories.Select(v => v.Key).ToList();
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(MultiSettingsLoggers.ReadAll), ex);
                return null;
            }
        }

        /// <summary>
        /// A shortcut to get the instance of the specific factory
        /// </summary>
        /// <typeparam name="TClass">The class type of that specific factory</typeparam>
        /// <returns></returns>
        public static TClass Instance<TClass>()
            where TClass : class, new()
        {
            string name = typeof(TClass).Name;
            if (!Factories.ContainsKey(name))
            {
                HandleExceptions(Factories, new Exception($"{name} doesn't exist in the Fatories."));
                return default;
            }

            try
            {
                return (Factories[name] as SettingsLoggerFactory<TClass>).SettingsClassInstance;
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(MultiSettingsLoggers.Instance), ex);
                return default;
            }
        }

        /// <summary>
        /// Add and build a factory dynamically.         
        /// This is a helpful function for some scenarios where you need to add another <see cref="SettingsLoggerFactory{TClass}"/> in the run time. <br></br>
        /// <br></br>
        /// <strong> NOTE: Please be sure that you have already initiated the Factories by constructing one.  Or it'll throw a <see cref="NullReferenceException"/></strong>
        /// </summary>
        public static void AddFactory(BaseSettingsLoggerFactory factory)
        {
            if (Factories == null) throw new NullReferenceException("The Factories dictionary haven't been constructed yet. Make sure you have call the construct function first.");
            try
            {
                Factories[factory.FactoryName] = factory;
                IoC.Logger.Log($"Add factory {factory.FactoryName} to Factories dynamically.");
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(MultiSettingsLoggers.AddFactory), ex);
            }
        }

        /// <summary>
        /// Remove a factory dynamically. 
        /// This is a helpful function for some scenarios where you need to remove a <see cref="SettingsLoggerFactory{TClass}"/> in the run time. <br></br>
        /// <br></br>
        /// <strong> NOTE: Please be sure that you have already initiated the Factories by constructing one.  Or it'll throw a <see cref="NullReferenceException"/></strong>
        /// </summary>
        /// <param name="factoryName">The name of the factory. It'll be the class you passed in to <see cref="SettingsLoggerFactory{TClass}"/> by default.</param>
        public static void RemoveFactory(string factoryName)
        {
            if (Factories == null) throw new NullReferenceException("The Factories dictionary haven't been constructed yet. Make sure you have call the construct function first.");
            try
            {
                if (Factories.ContainsKey(factoryName))
                {
                    Factories.Remove(factoryName);
                    IoC.Logger.Log($"Remove factory {factoryName} to Factories dynamically.");
                }
            }
            catch (Exception ex)
            {
                HandleExceptions(nameof(MultiSettingsLoggers.RemoveFactory), ex);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// A helper function to handle all the exceptions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        private static void HandleExceptions(object sender, Exception ex)
        {
            IoC.Logger.Log($"Error sent from{sender}:\r\n{ex.Message}");
            ErrorOccurs.Invoke((sender, ex));
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Fires when an error occurs.
        /// </summary>
        public static event Action<(object sender, Exception exception)> ErrorOccurs = (detail) => { };

        private static void MultiSettingsLoggers_ErrorOccurs((object sender, Exception exception) obj)
        {
            HandleExceptions(obj.sender, obj.exception);
        }

        #endregion

        #region Builder Methods

        /// <summary>
        ///  Construct the Factories.<br></br>
        ///  <br></br>
        /// <strong>NOTE: This needs to be called first before adding any factory.</strong>
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, BaseSettingsLoggerFactory> Construct()
        {
            // In case some use this function to recreate a factory. Need to avoid memory leak.
            if (Factories != null) foreach (var f in Factories.Keys) Factories[f].ErrorOccurs -= MultiSettingsLoggers_ErrorOccurs;

            Factories = new Dictionary<string, BaseSettingsLoggerFactory>();

            return Factories;
        }

        /// <summary>
        /// Add a settings logger factory to the dictionary after constructing the Factories. <br></br>
        /// New up a <see cref="SettingsLogger{TClass}"/>, select a format you need, but do not need to build.
        /// </summary>
        /// <param name="dictionary">The dictionary to add (Extention). </param>
        /// <param name="factory">The factory to add.</param>
        /// <returns></returns>
        public static Dictionary<string, BaseSettingsLoggerFactory> AddFactory(this Dictionary<string, BaseSettingsLoggerFactory> dictionary, BaseSettingsLoggerFactory factory)
        {
            try
            {
                dictionary.Add(factory.FactoryName, factory);
                IoC.Logger.Log($"Add factory {factory.FactoryName} to Factories.");

                return dictionary;
            }
            catch(Exception ex)
            {
                HandleExceptions(nameof(MultiSettingsLoggers.AddFactory), ex);
                return dictionary;
            }
        }       

        /// <summary>
        /// Build all the factorys in the dictionary
        /// </summary>
        /// <param name="factory"></param>
        public static void Build(this Dictionary<string, BaseSettingsLoggerFactory> factory)
        {
            foreach (var name in factory.Keys)
            {
                try
                {
                    // Build the factory
                    factory[name].Build();

                    // Hook up the event
                    factory[name].ErrorOccurs += MultiSettingsLoggers_ErrorOccurs;
                }
                catch(Exception ex)
                {
                    HandleExceptions(nameof(MultiSettingsLoggers.Build), ex);
                }
            }
        }

        #endregion    
    }
}
