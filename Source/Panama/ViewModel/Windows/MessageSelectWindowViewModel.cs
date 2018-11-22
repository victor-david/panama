using Restless.App.Panama.Database.Tables;
using Restless.Tools.Controls;
using Restless.Tools.Utility;
using Restless.Tools.Utility.Search;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using SysProps = Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the view model logic for the <see cref="View.MessageSelectWindow"/>.
    /// </summary>
    public class MessageSelectWindowViewModel : WindowDataGridViewModel<DummyTable>
    {
        #region Private
        private ObservableCollection<WindowsSearchResult> resultsView;
        private IList selectedDataGridItems;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a boolean value that indicates if this view model is in message select mode.
        /// If not, it is in folder select mode.
        /// </summary>
        public bool IsMessageSelectMode
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets the list of items that were selected by the user, or null if nothing selected.
        /// </summary>
        public List<WindowsSearchResult> SelectedItems
        {
            get;
            private set;
        }

        /// <summary>
        /// Sets the selected data grid items. This property is bound to the view.
        /// </summary>
        public IList SelectedDataGridItems
        {
            set
            {
                selectedDataGridItems = value;
            }
        }

        #endregion
        
        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSelectWindowViewModel"/> class.
        /// </summary>
        /// <param name="owner">The window that owns this view model.</param>
        /// <param name="options">The options that affect how the window is displayed and used.</param>
        public MessageSelectWindowViewModel(Window owner, MessageSelectOptions options)
            :base(owner)
        {
            resultsView = new ObservableCollection<WindowsSearchResult>();
            MainSource.Source = resultsView;
            switch (options.Mode)
            {
                case MessageSelectMode.Message:
                    //Columns.Create("Type", WindowsSearchResult.GetBindingReference(SysProps.System.ItemType)).MakeFixedWidth(FixedWidth.LongString);
                    Columns.Create("Size", WindowsSearchResult.GetBindingReference(SysProps.System.Size)).MakeNumeric(null, FixedWidth.LongerNumeric);
                    //Columns.Create("Created", WindowsSearchResult.GetBindingReference(SysProps.System.DateCreated)).MakeDate();
                    Columns.SetDefaultSort(Columns.Create("Date", WindowsSearchResult.GetBindingReference(SysProps.System.DateCreated)).MakeDate(), ListSortDirection.Descending);
                    //Columns.Create("File", WindowsSearchResult.GetBindingReference(SysProps.System.ItemPathDisplay)).MakeFlexWidth(2.0);
                    Columns.Create("Subject", WindowsSearchResult.GetBindingReference(SysProps.System.Subject)).MakeFlexWidth(1.25);
                    Columns.Create("From", WindowsSearchResult.GetBindingReference(SysProps.System.Message.FromName));
                    IsMessageSelectMode = true;
                    break;
                case MessageSelectMode.Folder:
                    Columns.Create("Type", WindowsSearchResult.GetBindingReference(SysProps.System.ItemTypeText)).MakeFixedWidth(112);
                    Columns.SetDefaultSort(Columns.Create("Name", WindowsSearchResult.GetBindingReference(SysProps.System.ItemNameDisplay)), ListSortDirection.Ascending);
                    IsMessageSelectMode = false;
                    break;
            }

            Commands.Add("Select", RunSelectCommand, (o) => IsSelectedRowAccessible);
            Commands.Add("Cancel", (o) => { Owner.Close(); });

            Commands.Add("OpenItem", RunOpenItemCommand, (o) => IsSelectedRowAccessible);
            FilterPrompt = "need a prompt"; // Strings.FilterPromptPublisher; MAPI/IPM.Note MAPI.Ipm.Note.Read

            RunSearch(options);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        #endregion

        /************************************************************************/
        
        #region Private methods

        private void RunSearch(MessageSelectOptions options)
        {
            resultsView.Clear();
            var provider = new WindowsMapiSearch();
            if (!string.IsNullOrEmpty(options.Scope))
            {
                provider.Scopes.Add(options.Scope);
            }
            switch (options.Mode)
            {
                case MessageSelectMode.Message:
                    provider.IncludedTypes.Add(SearchItemTypes.Mapi.Note);
                    provider.IncludedTypes.Add(SearchItemTypes.Mapi.NoteRead);
                    provider.OrderBy.Add(SysProps.System.DateCreated, ListSortDirection.Descending);
                    break;
                case MessageSelectMode.Folder:
                    provider.IncludedTypes.Add(SearchItemTypes.Mapi.Folder);
                    provider.OrderBy.Add(SysProps.System.ItemPathDisplay, ListSortDirection.Ascending);
                    provider.AddingResult += (s, e) =>
                        {
                            //Debug.WriteLine("Name: {0} URL: {1}", e.Result.Values[SysProps.System.ItemNameDisplay], e.Result.Values[SysProps.System.ItemUrl]);
                            string url = e.Result.Values[SysProps.System.ItemUrl].ToString();
                            e.Cancel = !url.Contains("/0");
                        };
                    break;
            }

            var results = provider.GetSearchResults(null);
            foreach (var result in results)
            {
                resultsView.Add(result);
            }
        }

        private void RunSelectCommand(object o)
        {
            if (selectedDataGridItems != null && selectedDataGridItems.Count > 0)
            {
                SelectedItems = new List<WindowsSearchResult>();
                foreach (var item in selectedDataGridItems.OfType<WindowsSearchResult>())
                {
                    SelectedItems.Add(item);
                }
            }
            Owner.Close();
        }

        private void RunOpenItemCommand(object o)
        {
            var row = SelectedItem as WindowsSearchResult;
            if (row != null)
            {
                OpenHelper.OpenFile(row.Values[SysProps.System.ItemUrl].ToString());
            }
        }
        #endregion
    }
}
