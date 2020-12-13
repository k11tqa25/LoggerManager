using System;
using System.Collections.Generic;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The interface of the result logger factory.
    /// </summary>
    public interface IResultLoggerFactory
    {
        /// <summary>
        /// Save the result file
        /// </summary>
        /// <returns></returns>
        bool Save();

        /// <summary>
        /// Get all the current filenames
        /// </summary>
        /// <returns></returns>
        List<string> GetFilenames();

        /// <summary>
        /// Build the factory. This could function as a constructor.
        /// </summary>
        void Build();

        /// <summary>
        /// The error event that fires when an error occurs.
        /// </summary>
        event Action<(object sender, Exception exception)> ErrorOccurs;
    }
}
