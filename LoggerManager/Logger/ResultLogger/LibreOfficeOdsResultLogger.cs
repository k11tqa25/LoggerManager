using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The result logger class for logging in LibreOffice ODS file format.
    /// </summary>
    /// <typeparam name="T">The class type of the result logger</typeparam>
    public class LibreOfficeOdsResultLogger<T> : IResultLogger<T>
        where T : class, new()
    {
        #region Private Properties

        private string styledTemplate = @".\StyledTemplate.ods";

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

        /// <summary>
        /// A specific filename of the template to save a ods file. 
        /// </summary>
        public string TemplateFilename { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize the ods result logger.
        /// </summary>
        /// <param name="resultClassInstance">The class instance</param>
        /// <param name="resultFilename">The filename to be saved. Do not append the extension filename</param>
        /// <param name="templateFilename">The filename of the ods template. Note that this tamplate CANNOT be a password protected file.</param>
        public LibreOfficeOdsResultLogger(T resultClassInstance, string resultFilename, string templateFilename)
        {
            ResultClassInstance = resultClassInstance;

            resultFilename = CommonFunctions.GetFilenameWithoutExtension(resultFilename);

            resultFilename = CommonFunctions.NormalizePath(resultFilename);

            resultFilename = CommonFunctions.ResolvePath(resultFilename);

            Filename = resultFilename + ".ods";

            templateFilename = CommonFunctions.NormalizePath(templateFilename);

            templateFilename = CommonFunctions.ResolvePath(templateFilename);

            TemplateFilename = templateFilename;
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
        /// Save the result logger file
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            IoC.Logger.Log($"Saving ods file to {Filename}...");

            string newTemplate = "";

            try
            {
                // The style dataset
                DataSet styleDataset = null;

                // Convert the class to a dataset
                DataSet data = ResultClassInstance.ToDataset();

                #region Prepare for style

                Dictionary<string, CellStyle> styleDictionary;

                // Get all the defined styles
                var styles = (DefineStyleAttribute[])ResultClassInstance.GetType().GetCustomAttributes(typeof(DefineStyleAttribute), false);
                if (styles != null)
                {
                    styleDictionary = new Dictionary<string, CellStyle>();

                    foreach (var style in styles)
                    {
                        styleDictionary.Add(style.StyleName,
                            new CellStyle()
                            {
                                ForegroundColor = style.ForegroundColor != "" ? Color.FromName(style.ForegroundColor) : Color.Empty,
                                BackgroundColor = style.BackgroundColor != "" ? Color.FromName(style.BackgroundColor) : Color.Empty
                            });


                        IoC.Logger.Log($"New style added:  {style.StyleName} = (Foreground {style.ForegroundColor}, Background {style.BackgroundColor})");
                    }

                    // Remove the existing styled template
                    if (File.Exists(styledTemplate)) File.Delete(styledTemplate);

                    // Create a new one
                    if (LibreOfficeHelper.CreateStyleTemplate(styleDictionary, TemplateFilename, styledTemplate))
                    {
                        // If successfully created 
                        newTemplate = styledTemplate;

                        // Log it
                        IoC.Logger.Log($"Style tempate created.");

                        // Create style dataset
                        styleDataset = ResultClassInstance.ToDatasetStyle(new HashSet<string>(styleDictionary.Keys));

                        // Debug
                        //LibreOfficeHelper.WriteToOdsFile(styleDataset, ".\\StyleDataset.ods", newTemplate);
                    };
                }

                #endregion


                string template = newTemplate == "" ? TemplateFilename : newTemplate;

                // Save as ods file
                LibreOfficeHelper.WriteToOdsFile(data, Filename, template, styleDataset);

                // Remove the style template
                if (File.Exists(styledTemplate)) File.Delete(styledTemplate);
            }
            catch (Exception ex)
            {
                HandleExceptions(this, ex);
                return false;
            }

            IoC.Logger.Log($"Ods file saved.");
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
