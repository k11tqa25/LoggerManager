namespace LoggerManagerLibrary
{
    /// <summary>
    /// The abstract class for the logger factory
    /// </summary>
    public abstract class AbstractLoggerFactory
    {
        /// <summary>
        /// The filename the save/read the file. <br></br>
        /// <strong>NOTE: Do not need to include the filename extension.</strong>
        /// </summary>
        public string Filename { get; protected set; }

        /// <summary>
        /// The name of this factory
        /// </summary>
        public string FactoryName { get; set; }
    }
}
