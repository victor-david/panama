using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Panama.Tools;
using Restless.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;
using OrphanValues = Restless.Panama.Database.Tables.OrphanExclusionTable.Defs.Values;
using TableColumns = Restless.Panama.Database.Tables.OrphanExclusionTable.Defs.Columns;
using TableValues = Restless.Panama.Database.Tables.OrphanExclusionTable.Defs.Values;

namespace Restless.Panama.ViewModel
{
    public class OrphanExclusionController : DataRowViewModel<OrphanExclusionTable>
    {
        private readonly List<string> excludedDirs;
        private readonly List<string> excludedDirsAuto;
        private readonly List<string> excludedExtensions;
        private readonly List<string> excludedFiles;

        private OrphanExclusionRow selectedOrphan;

        #region Properties
        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => !(SelectedOrphan?.IsSystem ?? true);

        /// <summary>
        /// Gets the selected orphan
        /// </summary>
        public OrphanExclusionRow SelectedOrphan
        {
            get => selectedOrphan;
            private set => SetProperty(ref selectedOrphan, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OrphanExclusionController"/> class
        /// </summary>
        public OrphanExclusionController()
        {
            Columns.Create(Header.Id, TableColumns.Id)
                .MakeFixedWidth(FixedWidth.W042)
                .MakeInitialSortAscending();

            Columns.Create<OrphanTypeConverter>(Header.Type, TableColumns.Type)
                .MakeFixedWidth(FixedWidth.W076);

            Columns.Create(Header.Value, TableColumns.Value);

            Columns.Create(Header.Created, TableColumns.Created)
                .MakeDate();

            MenuItems.AddItem(Strings.MenuItemRemoveExclusion, DeleteCommand)
                .AddIconResource(ResourceKeys.Icon.IconDelete);

            excludedDirs = new List<string>();
            excludedDirsAuto = new List<string>();
            excludedExtensions = new List<string>();
            excludedFiles = new List<string>();
            PopulateExclusionCache();
        }
        #endregion

        /************************************************************************/

        #region Public methods

        public bool IsDirectoryExcluded(string value)
        {
            if (excludedDirsAuto.Contains(value))
            {
                return true;
            }
            foreach (string directory in excludedDirs)
            {
                if (value.StartsWith(directory, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsFileExtensionExcluded(string value)
        {
            return excludedExtensions.Contains(value);
        }

        public bool IsFileExcluded(string value)
        {
            return excludedFiles.Contains(value);
        }

        public void AddFileExclusion(FileScanItem item)
        {
            Table.AddFileExclusion(Paths.Title.WithoutRoot(item.FullName));
            PopulateExclusionCache();
            ListView.Refresh();
        }

        public void AddFileExtensionExclusion(FileScanItem item)
        {
            Table.AddFileExtensionExclusion(item.FileExtension);
            PopulateExclusionCache();
            ListView.Refresh();
        }

        public void AddDirectoryExclusion(FileScanItem item)
        {
            Table.AddDirectoryExclusion(Path.GetDirectoryName(Paths.Title.WithoutRoot(item.FullName)));
            PopulateExclusionCache();
            ListView.Refresh();
        }
        #endregion

        /************************************************************************/

        #region Proptected methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedOrphan = OrphanExclusionRow.Create(SelectedRow);
        }

        /// <inheritdoc/>
        protected override void RunDeleteCommand()
        {
            DeleteSelectedRow();
            PopulateExclusionCache();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void PopulateExclusionCache()
        {
            excludedDirs.Clear();
            excludedDirsAuto.Clear();
            excludedExtensions.Clear();
            excludedFiles.Clear();

            // auto exclusions
            excludedDirsAuto.Add(Config.FolderSubmissionDocument);
            excludedDirsAuto.Add(Config.FolderExport);
            excludedDirsAuto.Add(Config.FolderSubmissionMessage);
            excludedDirsAuto.Add(Config.FolderSubmissionMessageAttachment);

            // user dir exclusions
            foreach (string directory in OrphanExclusionTable.EnumerateExclusion(OrphanValues.DirectoryType).Select(p => Paths.Title.WithRoot(p.Value)))
            {
                excludedDirs.Add(directory);
            }

            // user file extension exclusions
            foreach (OrphanExclusionRow item in OrphanExclusionTable.EnumerateExclusion(OrphanValues.FileExtensionType))
            {
                excludedExtensions.Add(item.Value);
            }

            // user file exclusions
            foreach (string file in OrphanExclusionTable.EnumerateExclusion(OrphanValues.FileType).Select(p => Paths.Title.WithRoot(p.Value)))
            {
                excludedFiles.Add(file);
            }
        }
        #endregion

        /************************************************************************/

        #region Private helper class
        private class OrphanTypeConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value is long type
                    ? type switch
                    {
                        TableValues.FileType => "File",
                        TableValues.FileExtensionType => "Extension",
                        TableValues.DirectoryType => "Directory",
                        _ => "???"
                    }
                    : value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

    }
}