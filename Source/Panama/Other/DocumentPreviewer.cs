using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.Tools.OpenXml;
using Restless.Tools.Utility;
using System.IO;

namespace Restless.App.Panama
{
    /// <summary>
    /// Provides static methods to preview a document
    /// </summary>
    public static class DocumentPreviewer
    {
        #region Public methods
        /// <summary>
        /// Gets the preview text from the specified file.
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>The text.</returns>
        public static string Preview(string fileName)
        {
            Validations.ValidateNullEmpty(fileName, "Preview.FileName");
            Int64 docType = DatabaseController.Instance.GetTable<DocumentTypeTable>().GetDocTypeFromFileName(fileName);
            switch (docType)
            {
                case DocumentTypeTable.Defs.Values.WordOpenXmlFileType:
                    return PreviewWordOpenXml(fileName);
                case DocumentTypeTable.Defs.Values.TextFileType:
                    return PreviewText(fileName);
                case DocumentTypeTable.Defs.Values.HtmlFileType:
                    return PreviewHtml(fileName);
                default:
                    return "Unsupported document type";
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods

        private static string PreviewWordOpenXml(string fileName)
        {
            try
            {
                return OpenXmlDocument.Reader.GetText(fileName);
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        private static string PreviewText(string fileName)
        {
            try
            {
                return File.ReadAllText(fileName);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static string PreviewHtml(string fileName)
        {
            return PreviewText(fileName);
        }
        #endregion
    }
}
