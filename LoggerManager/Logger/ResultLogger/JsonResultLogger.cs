using Newtonsoft.Json;
using System;
using System.IO;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The result logger with JSON format.
    /// </summary>
    /// <typeparam name="T">The class type of the result logger.</typeparam>
    public class JsonResultLogger<T> : IResultLogger<T>
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

        /// <summary>
        /// The schema to validate a json file when read. 
        /// Refer to Json.NET Schema to see how to specify a schema for JSON file in C#.
        /// </summary>
        public string Schema { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="resultClassInstance">The instance of the class</param>
        /// <param name="filename">The filename of the result logger. DO NOT append the filename extension.</param>
        /// <param name="schema">The schema for the validation. Leave it null if not needed.</param>
        public JsonResultLogger(T resultClassInstance, string filename = "", string schema = null)
        {
            ResultClassInstance = resultClassInstance;
            Filename = filename + ".json";
            Schema = schema;
        }

        #endregion

        #region Public Properties

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
            IoC.Logger.Log($"Saving Json file to {Filename}...");

            // The content after serialized to json format
            string content = "";

            try
            {
                // Serialize to json format
                content = JsonConvert.SerializeObject(ResultClassInstance, Formatting.Indented);
            }
            catch (Exception ex)
            {
                HandleExceptions(this, ex);
                return false;
            }

            try
            {
                // Write to file
                File.WriteAllText(Filename, content);
            }
            catch (Exception ex)
            {
                HandleExceptions(this, ex);
                return false;
            }

            IoC.Logger.Log($"Json file saved.");
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
