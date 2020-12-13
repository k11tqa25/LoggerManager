using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace LoggerManagerLibrary
{
    /// <summary>
    ///  The common functions that' ll be used in this project
    /// </summary>
    public static class CommonFunctions
    {
        /// <summary>
        /// Normalizing a path based on the current operating system
        /// </summary>
        /// <param name="path">The path to normalize</param>
        /// <returns></returns>
        public static string NormalizePath(string path)
        {
            // If on Windows...
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                // Replace any / with \
                return path?.Replace('/', '\\').Trim();
            // If on Linux/Mac
            else
                // Replace any \ with /
                return path?.Replace('\\', '/').Trim();
        }

        /// <summary>
        /// Resolves any relative elements of the path to absolute
        /// </summary>
        /// <param name="path">The path to resolve</param>
        /// <returns></returns>
        public static string ResolvePath(string path)
        {
            // Resolve the path to absolute
            return Path.GetFullPath(path);
        }

        /// <summary>
        /// Get the filename without the extension
        /// </summary>
        /// <param name="filename">The filename with our without the extension.</param>
        /// <returns></returns>
        public static string GetFilenameWithoutExtension(string filename)
        {
            string filepath = Path.GetDirectoryName(filename);
            string name = Path.GetFileNameWithoutExtension(filename);
            return filepath + "\\" + name;
        }

        /// <summary>
        /// Create an instance of a class.
        /// </summary>
        /// <typeparam name="T">The class type</typeparam>
        /// <returns></returns>
        public static T ActivateClass<T>()
            where T : class, new()
        {
            // Create an instance of the class
            T classInstance = new T();

            // Get all properties of the subject
            var propertyInfos = classInstance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // See if there's other class type
            foreach (var prop in propertyInfos)
            {
                // If this property is a generic list
                if (prop.PropertyType.IsGenericType)
                {
                    var listType = typeof(List<>);
                    var constructedListType = listType.MakeGenericType(prop.PropertyType.GetGenericArguments());

                    prop.SetValue(classInstance, Activator.CreateInstance(constructedListType));
                }
                // If this property is not system type
               else  if (prop.PropertyType.Namespace != "System")
                {
                    var p = prop.PropertyType; 
                    prop.SetValue(classInstance, Activator.CreateInstance(prop.PropertyType));
                }

            }

            return classInstance;
        }
    }
}
