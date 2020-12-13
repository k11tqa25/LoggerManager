using System;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// The base class of the <see cref="SettingsLoggerFactory{TClass}"/>.
    /// </summary>
    public abstract class BaseSettingsLoggerFactory : AbstractLoggerFactory, ISettingsLoggerFactory
    {
        /// <summary>
        /// Fires when an error occurs
        /// </summary>
        public virtual event Action<(object sender, Exception exception)> ErrorOccurs;

        /// <summary>
        /// Build this logger
        /// </summary>
        public virtual void Build()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Read the settings file
        /// </summary>
        /// <returns></returns>
        public virtual bool Read()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save the settings file
        /// </summary>
        /// <returns></returns>
        public virtual bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
