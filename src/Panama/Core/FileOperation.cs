using Restless.Panama.Resources;
using System;
using System.IO;

namespace Restless.Panama.Core
{
    public static class FileOperation
    {
        /// <summary>
        /// Gets a boolean value that indicates whether the file is in use.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsFileInUse(string path)
        {
            return IsFileInUse(new FileInfo(path));
        }

        /// <summary>
        /// Gets a boolean value that indicates whether the file is in use.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsFileInUse(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Throws an exception if the file doesn't exist or is in use.
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static void ValidateFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(Error.FileNotFound);
            }

            if (IsFileInUse(path))
            {
                throw new InvalidOperationException(Error.FileNotAvailable);
            }
        }
    }
}