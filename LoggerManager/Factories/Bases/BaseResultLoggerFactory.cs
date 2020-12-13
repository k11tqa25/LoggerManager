using System;
using System.Collections.Generic;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The base class for the <see cref="ResultLoggerFactory{TClass}"/>.
    /// </summary>
    public abstract class BaseResultLoggerFactory : AbstractLoggerFactory, IResultLoggerFactory
    {
        /// <summary>
        /// Fires when an error occurs.
        /// </summary>
        public virtual event Action<(object sender, Exception exception)> ErrorOccurs;

        /// <summary>
        /// Build this logger
        /// </summary>
        public virtual void Build()
        {
        }

        /// <summary>
        /// Get all the filenames in the Factory
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetFilenames()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save the result logger files
        /// </summary>
        /// <returns></returns>
        public virtual bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
