using System;
using System.IO;
using System.Text;

namespace Restless.Panama.Utility
{
    /// <summary>
    /// Provides simple logging capabilities
    /// </summary>
    public class Logger
    {
        #region Private
        private const string LogFileName = "exception.log";
        private const string NullException = "(null exception)";
        #endregion

        /// <summary>
        /// Gets the full path for log file
        /// </summary>
        public string LogFile
        {
            get;
        }

        #region Constructors
        /// <summary>
        /// Gets the static singleton instance of <see cref="Logger"/>
        /// </summary>
        public static Logger Instance { get; } = new Logger();

        private Logger()
        {
            LogFile = Path.Combine(RegistryManager.AppDataDirectory, LogFileName);
            Directory.CreateDirectory(RegistryManager.AppDataDirectory);
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// LOgs an exception
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="e">The exception</param>
        public void LogException(string source, Exception e)
        {
            File.AppendAllText(LogFile, GetLogExceptionMessage(source, e));
        }

        /// <summary>
        /// Gets an exception message, includes all nested exceptions
        /// </summary>
        /// <param name="e">An exception</param>
        /// <returns>A string</returns>
        public string GetExceptionMessage(Exception e)
        {
            return GetExceptionMessage(e ?? new Exception(NullException), 0);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private string GetLogExceptionMessage(string source, Exception e)
        {
            StringBuilder builder = new StringBuilder();
            string header = $"{source} unhandled exception: {DateTime.Now} local time";
            builder.AppendLine(header);
            builder.AppendLine(string.Empty.PadLeft(header.Length, '='));
            if (e != null)
            {
                builder.Append(GetExceptionMessage(e, 0));
                builder.AppendLine("Stack trace:");
                builder.AppendLine(e.StackTrace);
                builder.AppendLine("===END");
            }
            else
            {
                builder.AppendLine(NullException);
            }
            builder.AppendLine();
            return builder.ToString();
        }

        private string GetExceptionMessage(Exception e, int level)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"Level {level} => {e.GetType().FullName}");
            builder.AppendLine(e.Message);
            
            if (e.InnerException != null)
            {
                builder.AppendLine();
                builder.Append(GetExceptionMessage(e.InnerException, level + 1));
            }
            return builder.ToString();
        }
        #endregion
    }
}