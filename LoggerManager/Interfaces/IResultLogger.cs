using System;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The interface of the result logger 
    /// </summary>
    /// <typeparam name="T">The class type of the result logger</typeparam>
    public interface IResultLogger<T>
        where T: class, new()
    {
        /// <summary>
        /// The result class instance to specify the things to log.
        /// </summary>
         T ResultClassInstance { get; set; }

        /// <summary>
        ///  The filename of the result file.
        ///  <strong>DO NOT include the extension filename (i.e., .txt, .xml, ...)</strong> <br></br>
        /// </summary>
        string Filename { get; set; }

        /// <summary>
        /// To initialize the result logger
        /// </summary>
        void Init();

        /// <summary>
        /// Save the result file
        /// </summary>
        /// <returns></returns>
        bool Save();

        /// <summary>
        /// The event triggered when an error occurs.
        /// </summary>
        event Action<(object sender, Exception exception)> ErrorOccurs;
    }
}
