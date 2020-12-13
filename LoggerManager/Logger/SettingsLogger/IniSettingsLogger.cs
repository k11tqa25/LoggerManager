using System;
using System.Collections.Generic;
using System.Text;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// A settings logger with ini format
    /// </summary>
    /// <typeparam name="TClass">The class type of the settings file</typeparam>
    public class IniSettingsLogger<TClass> : ISettingsLogger<TClass>
        where TClass : class, new()
    {

        #region Public Properties

        /// <summary>
        /// The instance of the class
        /// </summary>
        public TClass SettingsClassInstance { get; set; }

        /// <summary>
        /// The filename of the settings file
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Of no use.
        /// </summary>
        public string Schema { get; set; }

        #endregion

        #region Contructor

        /// <summary>
        /// The constructor 
        /// </summary>
        /// <param name="filename">The filename of the settings file. DO NOT append the filename extension.</param>
        public IniSettingsLogger(string filename)
        {
            SettingsClassInstance = CommonFunctions.ActivateClass<TClass>();
            Filename = filename + ".ini";
            IniFileHelper.Filename = Filename;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Build this logger
        /// </summary>
        public void Build()
        {
        }

        /// <summary>
        ///  Read the settings file
        /// </summary>
        /// <returns></returns>
        public bool Read()
        {
            IoC.Logger.Log($"Reading ini file to {Filename}...");

            try
            {
                SettingsClassInstance.FromIniFile();
            }
            catch (Exception ex)
            {
                HandleExceptions(this, ex);
                return false;
            }

            IoC.Logger.Log($"Ini file read.");
            return true;
        }

        /// <summary>
        ///  Save the settings file
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            IoC.Logger.Log($"Saving ini file to {Filename}...");

            try
            {
                SettingsClassInstance.ToIniFile();
            }
            catch (Exception ex)
            {
                HandleExceptions(this, ex);
                return false;
            }

            IoC.Logger.Log($"Ini file saved.");
            return true;
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

        #region Public Events

        /// <summary>
        /// Fires when an error occurs
        /// </summary>
        public event Action<(object sender, Exception exception)> ErrorOccurs;

        #endregion

    }
}
