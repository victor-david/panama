/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Panama.Tools;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Utility;
using System.ComponentModel;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the orphan finder tool.
    /// </summary>
    public class ToolOrphanViewModel : DataGridPreviewViewModel
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the controller object for the orphan finder operation.
        /// </summary>
        public ToolOrphanFinderController Controller
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolOrphanViewModel"/> class.
        /// </summary>
        public ToolOrphanViewModel()
        {
            DisplayName = Strings.CommandToolOrphan;
            MaxCreatable = 1;
            Controller = new ToolOrphanFinderController(this);
            MainSource.Source = Controller.NotFound;

            Commands.Add("Begin", (o) => Controller.Run());
            Columns.Create("Modified", nameof(FileScanDisplayObject.LastModified)).MakeDate();
            Columns.Create("Size", nameof(FileScanDisplayObject.Size)).MakeNumeric(null, FixedWidth.LongerNumeric);
            Columns.SetDefaultSort(Columns.Create("File", nameof(FileScanDisplayObject.FileName)), ListSortDirection.Ascending);
            AddViewSourceSortDescriptions();
            Commands.Add("CreateTitle", RunCreateTitleCommand, (p)=> SelectedItem != null);
            MenuItems.AddItem("Create a title entry from this file", Commands["CreateTitle"]);
            MenuItems.AddSeparator();
            // Commands["DeleteFile"] is created by ToolOrphanFinderController - it handles
            // file deletion and the removal of the corresponding item of its ObservableCollection
            MenuItems.AddItem("Delete this file", Commands["DeleteFile"]).AddImageResource("ImageDeleteMenu");
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Called by the ancestor class when a preview of the selected item is needed.
        /// </summary>
        /// <param name="selectedItem">The currently selected grid item.</param>
        protected override void OnPreview(object selectedItem)
        {
            if (selectedItem is FileScanDisplayObject item)
            {
                string fileName = Paths.Title.WithRoot(item.FileName);
                PerformPreview(fileName);
            }
        }

        /// <summary>
        /// Gets the preview mode for the specified item.
        /// </summary>
        /// <param name="selectedItem">The selected grid item</param>
        /// <returns>The preview mode</returns>
        protected override PreviewMode GetPreviewMode(object selectedItem)
        {
            if (selectedItem is FileScanDisplayObject item)
            {
                return DocumentPreviewer.GetPreviewMode(item.FileName);
            }
            return PreviewMode.Unsupported;
        }

        /// <summary>
        /// Raises the Closing event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            SetCancelIfTasksInProgress(e);
            base.OnClosing(e);
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        private void AddViewSourceSortDescriptions()
        {
            MainSource.SortDescriptions.Clear();
            MainSource.SortDescriptions.Add(new SortDescription("FileName", ListSortDirection.Ascending));
        }

        private void RunCreateTitleCommand(object parm)
        {
            if (SelectedItem is FileScanDisplayObject file && Messages.ShowYesNo(Strings.ConfirmationCreateTitleFromOrphan))
            {
                TitleTable title = DatabaseController.Instance.GetTable<TitleTable>();
                TitleVersionTable ver = DatabaseController.Instance.GetTable<TitleVersionTable>();
                TitleTable.RowObject row = new(title.AddDefaultRow())
                {
                    Title = "Title created from orphaned file",
                    Written = file.LastModified,
                    Notes = $"This entry was created from orphaned file {file.FileName}"
                };

                // Get a version controller and add a version
                ver.GetVersionController(row.Id).Add(Paths.Title.WithoutRoot(file.FileName));

                ver.Save();
                title.Save();
                Controller.NotFound.Remove(file);
            }
        }
        #endregion
    }
}