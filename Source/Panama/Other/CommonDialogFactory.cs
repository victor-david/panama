using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.Dialogs;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;

namespace Restless.App.Panama
{
    /// <summary>
    /// Provides static methods for creating CommonOpenFileDialog objects
    /// </summary>
    public static class CommonDialogFactory
    {

        /// <summary>
        /// Creates and returns CommonOpenFileDialog with the specified parms. All supported document types are added to the filter collection.
        /// </summary>
        /// <param name="initialDir">The initial directory for the dialog</param>
        /// <param name="title">The title</param>
        /// <param name="isFolderPicker">true if this dialog is to select a folder only.</param>
        /// <param name="selectorFileType">A value that indicates which type of file may be selected. The default (zero) allows all types.</param>
        /// <returns>A CommonOpenFileDialog object.</returns>
        public static CommonOpenFileDialog Create(string initialDir, string title, bool isFolderPicker = false, long selectorFileType = 0)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog() 
            { 
                IsFolderPicker = isFolderPicker,
                InitialDirectory = initialDir,
                Title = title 
            };

            if (!isFolderPicker)
            {
                foreach (DataRow row in DatabaseController.Instance.GetTable<DocumentTypeTable>().GetSupportedDocTypes())
                {
                    long id = (long)row[DocumentTypeTable.Defs.Columns.Id];
                    if (selectorFileType == 0 || selectorFileType == id)
                    {
                        dialog.Filters.Add(new CommonFileDialogFilter(row[DocumentTypeTable.Defs.Columns.Name].ToString(), row[DocumentTypeTable.Defs.Columns.Extensions].ToString()));
                    }
                }
            }
            return dialog;
        }
    }
}
