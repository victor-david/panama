using Microsoft.WindowsAPICodePack.Dialogs;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;

namespace Restless.Panama.Core
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
            CommonOpenFileDialog dialog = new()
            {
                IsFolderPicker = isFolderPicker,
                InitialDirectory = initialDir,
                Title = title
            };

            if (!isFolderPicker)
            {
                foreach (DocumentTypeRow row in DatabaseController.Instance.GetTable<DocumentTypeTable>().EnumerateSupported())
                {
                    if (selectorFileType == 0 || selectorFileType == row.Id)
                    {
                        dialog.Filters.Add(new CommonFileDialogFilter(row.Name, row.Extensions));
                    }
                }
            }
            return dialog;
        }
    }
}