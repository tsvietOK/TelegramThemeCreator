using System.IO;

namespace TelegramThemeCreator.Utils
{
    /// <summary>
    /// Provides static methods for files and folders access.
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// Removes the specified file, if it exists.
        /// </summary>
        /// <param name="path">A string containing path to file.</param>
        public static void DeleteIfExists(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// Removes and creates a folder in the specified path.
        /// </summary>
        /// <param name="path">A string containing path to folder.</param>
        public static void RecreateDirectory(string path)
        {
            DeleteIfExists(path);

            Directory.CreateDirectory(path);
        }
    }
}