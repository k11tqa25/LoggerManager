using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace LoggerManagerLibrary
{
    /// <summary>
    /// Handles reading/writing and querying the file system
    /// This is specifically use for logging the debug message of this <see cref="LoggerManagerLibrary"/> itself.
    /// </summary>
    public class InternalFileManager : IFileManager
    {
        public string Filename { get; set; }

        public InternalFileManager()
        {
            Filename = @"./LoggerManager.log";

            if (LoggerManagerConfiguration.OverrideDebugFile)
            {
                // If user want to override the existing file
                if(File.Exists(Filename)) File.Delete(Filename);
            }
        }

        /// <summary>
        /// Writes the text to the specified file
        /// </summary>
        /// <param name="text">The text to write</param>
        /// <param name="path">The path of the file to write to</param>
        /// <param name="append">If true, writes the text to the end of the file, otherwise overrides any existing file</param>
        /// <returns></returns>
        public async Task WriteTextToFileAsync(string text, string path = "", bool append = false)
        {
            // If the user don't want to save the debug logger. 
            if (!LoggerManagerConfiguration.SaveDebugFile) 
                // Then return.
                return;

            path = Filename;

            // Normalize path
            path = CommonFunctions.NormalizePath(path);

            // Resolve to absolute path
            path = CommonFunctions.ResolvePath(path);

            // Lock the task
            await AsyncAwaiter.AwaitAsync(nameof(FileManager) + path, async () =>
            {
                // Run the synchronous file access as a new task
                await IoC.Task.Run(() =>
                {
                    // Write the log message to file
                    using (var fileStream = (TextWriter)new StreamWriter(File.Open(path, FileMode.Append)))
                        fileStream.Write(text);
                });
            });
        }
    }
}
