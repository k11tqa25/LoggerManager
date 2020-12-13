using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.IO;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// A settings logger with Json format
    /// </summary>
    /// <typeparam name="TClass">The class type of the settings file</typeparam>
    public class JsonSettingsLogger<TClass> : ISettingsLogger<TClass>
        where TClass : class, new()
    {
        #region Public Properites

        /// <summary>
        /// The instance of the class
        /// </summary>
        public TClass SettingsClassInstance { get; set; }

        /// <summary>
        /// The filename of the settings file
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
        /// <param name="filename">The filename of the settings file. DO NOT append the filename extension.</param>
        /// <param name="schema">The schema that will be used to validate the settings file to be read. Leave it null if you don't want to use it.</param>
        public JsonSettingsLogger(string filename, string schema = null)
        {
            SettingsClassInstance = CommonFunctions.ActivateClass<TClass>(); 
            Filename = filename + ".json";
            Schema = schema;
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
            // Check if the file exist
            if (!File.Exists(Filename))
            {
                HandleExceptions(this, new Exception($"The JSON file: {Filename} does not exists."));
                return false;
            }

            try
            {
                // Read the file in as text
                IoC.Logger.Log($"Read {Filename} in text...");
                var read = File.ReadAllText(Filename);

                // If the schema exists, validate the json file with the schema
                if (Schema != null)
                {
                    try
                    {
                        if (Schema != string.Empty)
                            // Parse the string to json schema
                            IoC.Logger.Log($"Parsing the schema...");
                        JSchema schema = JSchema.Parse(Schema);

                        // Parse the text to json object
                        IoC.Logger.Log($"Parsing the {Filename} to JSON object...");
                        JObject json = JObject.Parse(read);

                        IoC.Logger.Log($"Validating the JSON file...");
                        // Validate the json
                        if (!json.IsValid(schema))
                        {
                            IoC.Logger.Log($"The JSON file: {Filename} does not match the schema.");
                            ErrorOccurs?.Invoke((this, new Exception($"The JSON file: {Filename} does not match the schema.")));
                            // If it's not valid
                            return false;
                        }
                        IoC.Logger.Log($"Validation Complete.");
                    }
                    catch(Exception ex)
                    {
                        HandleExceptions(this, ex);
                    }
                }

                // Deserialize the json object
                IoC.Logger.Log($"Deserializing JSON format...");
                object deserializedObj = JsonConvert.DeserializeObject(read, SettingsClassInstance.GetType());
                if (deserializedObj == null)
                {
                    HandleExceptions(this, new Exception("Cannot deserialize the object."));
                    return false;
                }
                SettingsClassInstance = (TClass)deserializedObj;
            }
            catch (Exception ex)
            {
                HandleExceptions(this, ex);
                return false;
            }

            IoC.Logger.Log($"Read Json finished. Here's the json file that has been read:\r\n" +
                $"{JsonConvert.SerializeObject(SettingsClassInstance, Formatting.Indented)}");
            return true;
        }

        /// <summary>
        ///  Save the settings file
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
                content = JsonConvert.SerializeObject(SettingsClassInstance, Formatting.Indented);
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
        /// Fires when an error occurs
        /// </summary>
        public event Action<(object sender, Exception exception)> ErrorOccurs;

        #endregion

    }
}
