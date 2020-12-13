using System;
using System.Collections.Generic;
using System.Text;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The result logger with ini format. <br></br>
    /// <br></br>
    /// <strong>NOTE: The ini format does NOT support List type.</strong>
    /// </summary>
    /// <typeparam name="T">The class type of the result logger.</typeparam>
    public class IniFileResultLogger<T> : IResultLogger<T>
        where T : class, new()
    {
        #region Public Properties

        /// <summary>
        /// The instance of the class
        /// </summary>
        public T ResultClassInstance { get; set; }

        /// <summary>
        /// The filename of the result logger.
        /// </summary>
        public string Filename { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="resultClassInstance">The instance of the class</param>
        /// <param name="filename">The filename of the result logger. DO NOT append the filename extension.</param>
        public IniFileResultLogger(T resultClassInstance, string filename)
        {
            ResultClassInstance = resultClassInstance;
            Filename = filename + ".ini";
            IniFileHelper.Filename = Filename;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize the logger
        /// </summary>
        public void Init()
        {
        }

        /// <summary>
        /// Save the logger
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            IoC.Logger.Log($"Saving ini file to {Filename}...");

            try
            {
                ResultClassInstance.ToIniFile();
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
        /// Fires when an error occurs.
        /// </summary>
        public event Action<(object sender, Exception exception)> ErrorOccurs;

        #endregion

    }
}
