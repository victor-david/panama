using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.Tools.OpenXml;
using Restless.Tools.Utility;
using System.IO;
using System.Windows.Media.Imaging;

namespace Restless.App.Panama
{
    /// <summary>
    /// Provides static methods to preview a document
    /// </summary>
    public static class DocumentPreviewer
    {
        #region Public methods
        /// <summary>
        /// Gets the preview mode for the specified file.
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>The preview mode</returns>
        public static PreviewMode GetPreviewMode(string fileName)
        {
            Validations.ValidateNullEmpty(fileName, "Preview.FileName");
            long docType = DatabaseController.Instance.GetTable<DocumentTypeTable>().GetDocTypeFromFileName(fileName);
            switch (docType)
            {
                case DocumentTypeTable.Defs.Values.WordOpenXmlFileType:
                case DocumentTypeTable.Defs.Values.TextFileType:
                case DocumentTypeTable.Defs.Values.HtmlFileType:
                    return PreviewMode.Text;
                case DocumentTypeTable.Defs.Values.ImageFileType:
                    return PreviewMode.Image;
            }
            return PreviewMode.Unsupported;
        }

        /// <summary>
        /// Gets the preview text from the specified file.
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>The text.</returns>
        public static string GetText(string fileName)
        {
            Validations.ValidateNullEmpty(fileName, "Preview.FileName");
            long docType = DatabaseController.Instance.GetTable<DocumentTypeTable>().GetDocTypeFromFileName(fileName);
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

        /// <summary>
        /// Gets a bitmap image for the specified file
        /// </summary>
        /// <param name="fileName">The file</param>
        /// <returns>The image</returns>
        public static BitmapImage GetImage(string fileName)
        {
            int width = 250;
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var bitmapFrame = BitmapFrame.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                width = bitmapFrame.PixelWidth;
            }

            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.UriSource = new Uri(fileName);
            img.DecodePixelWidth = width;
            img.EndInit();
            return img;
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
