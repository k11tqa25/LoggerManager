using System;
using System.IO;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The factory that handles the setting logger
    /// </summary>
    /// <typeparam name="TClass">The settings class type</typeparam>
    public class SettingsLoggerFactory<TClass> : BaseSettingsLoggerFactory
        where TClass : class, new()
    {
        #region Private Properties

        /// <summary>
        /// A private instance to ensure that there is always a thing to return.
        /// </summary>
        private TClass _instance = null;

        #endregion

        #region Public Properties

        /// <summary>
        /// The instance of the class
        /// </summary>
        public TClass SettingsClassInstance
        {
            get
            {
                if (SettingsLogger != null) return SettingsLogger.SettingsClassInstance;
                else return _instance;
            }
            set
            {
                _instance = value;
                if (SettingsLogger != null) SettingsLogger.SettingsClassInstance = value;
            }
        }

        /// <summary>
        /// The specific settings logger for this factory
        /// </summary>
        public ISettingsLogger<TClass> SettingsLogger { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filename">The filename of the result file. Do not append the extention filename.</param>
        public SettingsLoggerFactory(string filename)
        {
            FactoryName = typeof(TClass).Name;

            filename = CommonFunctions.GetFilenameWithoutExtension(filename);

            filename = CommonFunctions.NormalizePath(filename);

            filename = CommonFunctions.ResolvePath(filename);

            Filename = filename;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Build this logger.
        /// </summary>
        public override void Build()
        {
        }

        /// <summary>
        /// Read the settings file.
        /// </summary>
        /// <returns></returns>
        public override bool Read()
        {
            try
            {
                return SettingsLogger.Read();
            }
            catch (Exception ex)
            {
                HandleExceptions(this, ex);
                return false;
            }
        }

        /// <summary>
        /// Save the settings file
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            try
            {
                return SettingsLogger.Save();
            }
            catch(Exception ex)
            {
                HandleExceptions(this, ex);
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
        private void HandleExceptions(object sender, Exception ex)
        {
            IoC.Logger.Log($"Error sent from{sender}:\r\n{ex.Message}");
            ErrorOccurs.Invoke((sender, ex));
        }

        #endregion        

        #region Builder Methods

        /// <summary>
        /// Use a settings logger with built-in format.
        /// </summary>
        /// <param name="format">The format of the logger.</param>
        /// <param name="schema">The schema that will be used to validate the settings file when read. Leave it null if you don't need it.</param>
        /// <returns></returns>
        public SettingsLoggerFactory<TClass> UseBuiltInFormatSettingsLogger(SettingsFileFormat format, string schema = null)
        {
            switch (format)
            {
                case SettingsFileFormat.JSON:
                    SettingsLogger = new JsonSettingsLogger<TClass>(Filename, schema);
                    break;

                case SettingsFileFormat.INI:
                    SettingsLogger = new IniSettingsLogger<TClass>(Filename);
                    break;
            }

            SettingsLogger.ErrorOccurs += _settingsLogger_ErrorOccurs;

            // Chain the method
            return this;
        }

        /// <summary>
        /// Implement a <see cref="ISettingsLogger{T}"/> of your own and pass in to the logger. <br></br>
        /// It'll help you to create an instance of <see cref="SettingsClassInstance"/> and register the <c>ErrorOccurs</c> event for your custom setting logger. 
        /// </summary>
        /// <param name="customSettingsLogger"></param>
        public SettingsLoggerFactory<TClass> UseCustomSettingsLogger(ISettingsLogger<TClass> customSettingsLogger)
        {
            SettingsLogger = customSettingsLogger;

            SettingsLogger.SettingsClassInstance = CommonFunctions.ActivateClass<TClass>();

            SettingsLogger.ErrorOccurs += _settingsLogger_ErrorOccurs;

            // Chain the method
            return this;
        }

        #endregion

        #region Events

        /// <summary>
        /// Fires when an error occurs.
        /// </summary>
        public override event Action<(object sender, Exception exception)> ErrorOccurs;

        private void _settingsLogger_ErrorOccurs((object sender, Exception exception) obj)
        {
            HandleExceptions(obj.sender, obj.exception);
        }

        #endregion
    }
}
