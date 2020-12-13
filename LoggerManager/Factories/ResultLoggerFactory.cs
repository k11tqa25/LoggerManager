using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The factory that handles the work for the result logger.
    /// </summary>
    /// <typeparam name="TClass">The class type of the result logger.</typeparam>
    public class ResultLoggerFactory<TClass> : BaseResultLoggerFactory
        where TClass : class, new()
    {
        /// <summary>
        /// The list of result loggers in this factory
        /// </summary>
        protected List<IResultLogger<TClass>> mResultLoggers = new List<IResultLogger<TClass>>();

        #region Public Properties

        /// <summary>
        /// The class instance to store the result and then it'll be serialized to any format the user choose.
        /// This is used as an reference that would be passed in to all the result loggers.
        /// </summary>
        public TClass ResultClassInstance { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="filename">The filename of the result file. Do not append the extention filename.</param>
        public ResultLoggerFactory(string filename)
        {
            ResultClassInstance = CommonFunctions.ActivateClass<TClass>();

            FactoryName = typeof(TClass).Name;

            filename = CommonFunctions.GetFilenameWithoutExtension(filename);

            filename = CommonFunctions.NormalizePath(filename);

            filename = CommonFunctions.ResolvePath(filename);

            Filename = filename;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get all the filenames from the result loggers.
        /// </summary>
        /// <returns></returns>
        public override List<string> GetFilenames()
        {
            try
            {
                return mResultLoggers.Select(r => r.Filename).ToList();
            }
            catch (Exception ex)
            {
                HandleExceptions(this, ex);
                return null;
            }
        }

        /// <summary>
        /// Save the result.
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            bool ret = true;

            try
            {
                // Save for all loggers. If any logger return a false, the return will be false
                mResultLoggers.ForEach(logger => ret &= logger.Save());
            }
            catch (Exception ex)
            {
                HandleExceptions(this, ex);
                ret = false;
            }

            return ret;
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
        /// Add a text result logger to the result logger list.
        /// </summary>
        /// <returns></returns>
        /// <param name="filenameExtension">Apply any filename extension you need besides ".txt". </param>
        /// <param name="displayColumnNames">Display the name of the columns.</param>
        public ResultLoggerFactory<TClass> UseTextFileLoggerFactory(string filenameExtension = ".txt", bool displayColumnNames = false)
        {
            // New up a text result logger. Notice the filename has been normalized already.
            var txtLogger = new TextFileResultLogger<TClass>(ResultClassInstance, Filename, filenameExtension, displayColumnNames);

            // Add a text result logger to the logger list
            mResultLoggers.Add(txtLogger);

            // Register the error event
            txtLogger.ErrorOccurs += Logger_ErrorOccurs;

            // Chain the method
            return this;

        }

        /// <summary>
        /// Add an ini result logger to the result logger list.
        /// </summary>
        /// <returns></returns>
        public ResultLoggerFactory<TClass> UseIniFileLoggerFactory()
        {
            // New up a text result logger. Notice the filename has been normalized already.
            var iniLogger = new IniFileResultLogger<TClass>(ResultClassInstance, Filename);

            // Add a text result logger to the logger list
            mResultLoggers.Add(iniLogger);

            // Register the error event
            iniLogger.ErrorOccurs += Logger_ErrorOccurs;

            // Chain the method
            return this;

        }

        /// <summary>
        /// Add a json result logger to the result logger list
        /// </summary>
        /// <returns></returns>
        /// <param name="schema">The schema to validate a json file while reading. <br></br>
        /// Refer to https://www.newtonsoft.com/jsonschema to see how to specify a schema for JSON file in C#. </param>
        public ResultLoggerFactory<TClass> UseJsonResultLogger(string schema = null)
        {
            // New up a json result logger. Notice the filename has been normalized already.
            var jsonLogger = new JsonResultLogger<TClass>(ResultClassInstance, Filename, schema);

            // Add a json result logger to the logger list
            mResultLoggers.Add(jsonLogger);

            // Register the error event
            jsonLogger.ErrorOccurs += Logger_ErrorOccurs;

            // Chain the method
            return this;
        }

        /// <summary>
        /// Add a libre office result logger to the result logger list
        /// </summary>
        /// <param name="templateFilename">The template ods file to refer to. The file shouldn't be password protected.</param>
        /// <returns></returns>
        public ResultLoggerFactory<TClass> UseLibreOfficeResultLogger(string templateFilename)
        {
            // New up a LibreOffice result logger.Notice the filename has been normalized already.
            var libreLogger = new LibreOfficeOdsResultLogger<TClass>(ResultClassInstance, Filename, templateFilename);

            // Add a LibreOffice result logger to the logger list
            mResultLoggers.Add(libreLogger);

            // Register the error event
            libreLogger.ErrorOccurs += Logger_ErrorOccurs;

            // Chain the method
            return this;
        }

        /// <summary>
        /// Build this factory
        /// </summary>
        public override void Build()
        {
            // initialize all the result loggers
            mResultLoggers.ForEach(logger => logger.Init());
        }

        #endregion

        #region Events

        /// <summary>
        ///  The error event that fires when an error occurs.
        /// </summary>
        public override event Action<(object sender, Exception exception)> ErrorOccurs = (detail) => { };

        private void Logger_ErrorOccurs((object sender, Exception exception) obj)
        {
            HandleExceptions(obj.sender, obj.exception);
        }

        #endregion
    }
}
