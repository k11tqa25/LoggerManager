using System;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The interface for the settings logger factory
    /// </summary>
    public interface ISettingsLoggerFactory
    {
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
        /// Build the settings logger
        /// </summary>
        void Build();

        /// <summary>
        /// The error event that fires when an error occurs.
        /// </summary>
        event Action<(object sender, Exception exception)> ErrorOccurs;
    }
}
