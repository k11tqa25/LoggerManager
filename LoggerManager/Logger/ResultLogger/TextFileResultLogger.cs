using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// A result logger with plain text format.
    /// </summary>
    /// <typeparam name="T">The class type of the result logger.</typeparam>
    public class TextFileResultLogger<T> : IResultLogger<T>
        where T : class, new()
    {
        #region Private Properties

        private bool _displayColumnNames;

        private string _fileExtension;

        #endregion

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
        /// <param name="filenameExtension">Apply any filename extension you need besides ".txt". </param>
        /// <param name="displayColumnNames">Display the name of columns.</param>
        public TextFileResultLogger(T resultClassInstance, string filename, string filenameExtension = ".txt", bool displayColumnNames = false)
        {
            ResultClassInstance = resultClassInstance;
            Filename = filename;
            _fileExtension = filenameExtension;
            _displayColumnNames = displayColumnNames;
        }

        #endregion

        #region Public Methods


        /// <summary>
        /// Initialize the logger
        /// </summary>
        public void Init()
        {
            // Something that needs to be initialized.
        }

        /// <summary>
        /// Save the logger
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            IoC.Logger.Log($"Saving Text file to {Filename}...");

            try
            {
                // First convert to a data table, and then save it as text file
                foreach (DataTable t in ResultClassInstance.ToDataset().Tables)  t.SaveToTextFile($"{Filename}-{t.TableName}{_fileExtension}", _displayColumnNames);
            }
            catch (Exception ex)
            {
                HandleExceptions(this, ex);
                return false;
            }
            IoC.Logger.Log($"File saved.");

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
        /// Fires whenever an error occurs
        /// </summary>
        public event Action<(object sender, Exception exception)> ErrorOccurs = (details) => { };
         
        #endregion
    }
}
