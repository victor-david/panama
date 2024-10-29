/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Utility;
using Restless.Toolkit.Core.OpenXml;
using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Restless.Panama.Core
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
            Throw.IfEmpty(fileName);
            long docType = DatabaseController.Instance.GetTable<DocumentTypeTable>().GetDocTypeFromFileName(fileName);
            return docType switch
            {
                DocumentTypeTable.Defs.Values.WordOpenXmlFileType or DocumentTypeTable.Defs.Values.TextFileType or DocumentTypeTable.Defs.Values.HtmlFileType => PreviewMode.Text,
                DocumentTypeTable.Defs.Values.ImageFileType => PreviewMode.Image,
                _ => PreviewMode.Unsupported,
            };
        }

        /// <summary>
        /// Gets the preview text from the specified file.
        /// </summary>
        /// <param name="fileName">The file name</param>
        /// <returns>The text.</returns>
        public static string GetText(string fileName)
        {
            Throw.IfEmpty(fileName);
            long docType = DatabaseController.Instance.GetTable<DocumentTypeTable>().GetDocTypeFromFileName(fileName);
            return docType switch
            {
                DocumentTypeTable.Defs.Values.WordOpenXmlFileType => PreviewWordOpenXml(fileName),
                DocumentTypeTable.Defs.Values.TextFileType => PreviewText(fileName),
                DocumentTypeTable.Defs.Values.HtmlFileType => PreviewHtml(fileName),
                _ => "Unsupported document type",
            };
        }

        /// <summary>
        /// Gets a bitmap image for the specified file
        /// </summary>
        /// <param name="fileName">The file</param>
        /// <returns>The image</returns>
        public static BitmapImage GetImage(string fileName)
        {
            Throw.IfEmpty(fileName);

            int width = 250;
            using (FileStream stream = new(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BitmapFrame bitmapFrame = BitmapFrame.Create(stream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                width = bitmapFrame.PixelWidth;
            }

            BitmapImage img = new();
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
            return GetPreviewText(fileName, f => OpenXmlDocument.Reader.GetText(f));
        }

        private static string PreviewText(string fileName)
        {
            return GetPreviewText(fileName, f => File.ReadAllText(f));
        }

        private static string PreviewHtml(string fileName)
        {
            return PreviewText(fileName);
        }

        private static string GetPreviewText(string fileName, Func<string, string> evaluator)
        {
            try
            {
                return evaluator(fileName);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
    }
}