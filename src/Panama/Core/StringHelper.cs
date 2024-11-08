using Restless.Panama.Resources;
using Restless.Toolkit.Core.Utility;
using System;

namespace Restless.Panama.Core
{
    public static class StringHelper
    {
        /// <summary>
        /// Get submission confirmation text. Wording depends on <paramref name="openCount"/>
        /// </summary>
        /// <param name="openCount">The open count</param>
        /// <param name="publisherName">Name of publisher</param>
        /// <returns></returns>
        public static string GetSubmissionConfirmation(int openCount, string publisherName)
        {
            return
                openCount == 0 ?
                string.Format(Confirm.CreateSubmissionFormat, publisherName) :
                string.Format(Confirm.CreateSubmissionOpenFormat, publisherName);
        }

        /// <summary>
        /// Gets the text that is placed into the export read me file
        /// </summary>
        /// <returns></returns>
        public static string GetExportFileText()
        {
            AssemblyInfo a = new(AssemblyInfoType.Entry);
            return string.Format(Detail.ExportFileFormat, a.Title, DateTime.UtcNow.ToString("R"));
        }
    }
}