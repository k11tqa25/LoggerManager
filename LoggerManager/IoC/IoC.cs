using Ninject;
using System.Diagnostics;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// A Inversion of Control class that helps to handle the dependency injection of this logger manager.
    /// </summary>
    public static class IoC
    {
        #region Private Properties      

        /// <summary>
        /// This is used to avoid setting up the IoC repeatedly.
        /// </summary>
        private static bool _hasSetup = false;

        #endregion

        #region Public Properties

        /// <summary>
        /// The kernel for our IoC container
        /// </summary>
        public static IKernel Kernel { get; private set; } = new StandardKernel();

        /// <summary>
        /// A shortcut to access the <see cref="IBasicLogger"/>  => Outdated
        /// </summary>
        public static IBasicLoggerFactory Logger => IoC.Get<IBasicLoggerFactory>();

        /// <summary>
        /// A shortcut to access the <see cref="IFileManager"/>
        /// </summary>
        public static IFileManager File => IoC.Get<IFileManager>();

        /// <summary>
        /// A shortcut to access the <see cref="ITaskManager"/>
        /// </summary>
        public static ITaskManager Task => IoC.Get<ITaskManager>();

        #endregion

        /// <summary>
        /// Sets up the IoC container, binds all information required and is ready for use
        /// NOTE: Must be called as soon as your application starts up to ensure all 
        ///       services can be found
        /// </summary>
        static IoC()
        {
            // If the IoC haven't been setup 
            if (!_hasSetup)
            {
                // Bind a FileManager
                Kernel.Bind<IFileManager>().ToConstant(new FileManager());

                // Bind a TaskManager
                Kernel.Bind<ITaskManager>().ToConstant(new TaskManager());

                //  Bind a logger
                Kernel.Bind<IBasicLoggerFactory>().ToConstant(new BasicLoggerFactory().UseDebugLogger());
            }

            Logger.Log("LoggerManager has successfully setup.");

            // IoC has setup successfully.
            _hasSetup = true;
        }

        /// <summary>
        /// Get's a service from the IoC, of the specified type
        /// </summary>
        /// <typeparam name="T">The type to get</typeparam>
        /// <returns></returns>
        public static T Get<T>()
        {
            return Kernel.Get<T>();
        }
    }
}
