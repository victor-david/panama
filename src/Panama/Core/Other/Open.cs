using Restless.Panama.Resources;
using Restless.Panama.Utility;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.Utility;
using System;
using System.Globalization;
using System.IO;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Helper class to open various files
    /// </summary>
    public static class Open
    {
        /// <summary>
        /// Opens a title version file.
        /// </summary>
        /// <param name="file">The file name</param>
        /// <remarks>
        /// This method attempts to open a title version file. If the file name
        /// is not fully qualified, the current value of title root will be applied.
        /// </remarks>
        public static void TitleVersionFile(string file)
        {
            try
            {
                Throw.IfEmpty(file);
                file = Paths.Title.WithRoot(file);
                if (!File.Exists(file))
                {
                    throw new IOException(string.Format(CultureInfo.InvariantCulture, Strings.FormatStringFileNotFound, file, nameof(Config.FolderTitleRoot)));
                }

                OpenHelper.OpenFile(file);
            }
            catch (Exception ex)
            {
                MessageWindow.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Opens a submission documentn file.
        /// </summary>
        /// <param name="file">The file name</param>
        /// <remarks>
        /// This method attempts to open a submission document file. If the file name
        /// is not fully qualified, the current value of submission document directory will be applied.
        /// </remarks>
        public static void SubmissionDocumentFile(string file)
        {
            try
            {
                Throw.IfEmpty(file);
                file = Paths.SubmissionDocument.WithRoot(file);
                if (!File.Exists(file))
                {
                    throw new IOException(string.Format(CultureInfo.InvariantCulture, Strings.FormatStringFileNotFound, file, nameof(Config.FolderSubmissionDocument)));
                }

                OpenHelper.OpenFile(file);
            }
            catch (Exception ex)
            {
                MessageWindow.ShowError(ex.Message);
            }
        }
    }
}