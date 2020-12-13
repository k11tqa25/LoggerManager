using System;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The interface for the settings logger
    /// </summary>
    /// <typeparam name="T">The type of the class</typeparam>
    public interface ISettingsLogger<T>
        where T : class, new()
    {
        /// <summary>
        /// The settings class instance to specify the things to log.
        /// </summary>
        T SettingsClassInstance { get; set; }

        /// <summary>
        ///  The filename of the settings file.
        ///  <strong>DO NOT include the extension filename (i.e., .txt, .xml, ...)</strong> <br></br>
        /// </summary>
        string Filename { get; set; }

        /// <summary>
        /// The schema of the settings file.
        /// </summary>
        string Schema { get; set; }

        /// <summary>
        /// Save the settings file
        /// </summary>
        /// <returns></returns>
        bool Save();

        /// <summary>
        /// Read the settings file
        /// </summary>
        /// <returns></returns>
        bool Read();

        /// <summary>
        /// Build this logger
        /// </summary>
        void Build();

        /// <summary>
        /// The error event that fires when an error occurs.
        /// </summary>
        event Action<(object sender, Exception exception)> ErrorOccurs;
    }
}
