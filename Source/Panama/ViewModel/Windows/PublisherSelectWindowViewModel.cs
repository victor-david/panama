using Restless.App.Panama.Controls;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using System.ComponentModel;
using System.Windows;
using Restless.App.Panama.Converters;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the display and selection logic for the <see cref="View.PublisherSelectWindow"/>.
    /// </summary>
    public class PublisherSelectWindowViewModel : WindowDataGridViewModel<PublisherTable>
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the selected publisher id, or -1 if none selected.
        /// </summary>
        public long SelectedPublisherId
        {
            get;
            private set;
        }
        #endregion
        
        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherSelectWindowViewModel"/> class.
        /// </summary>
        /// <param name="owner">The window that owns this view model.</param>
        public PublisherSelectWindowViewModel(Window owner)
            :base(owner)
        {
            Columns.Create("Id", PublishedTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.Create("Name", PublisherTable.Defs.Columns.Name);
            var col = Columns.Create("Added", PublisherTable.Defs.Columns.Added).MakeDate();
            Columns.SetDefaultSort(col, ListSortDirection.Descending);
            Columns.Create("Last Sub", PublisherTable.Defs.Columns.Calculated.LastSub).MakeDate()
                .AddSort(null, PublisherTable.Defs.Columns.Name, DataGridColumnSortBehavior.AlwaysAscending);
            Columns.Create("SC", PublisherTable.Defs.Columns.Calculated.SubCount).MakeFixedWidth(FixedWidth.MediumNumeric)
                .AddSort(null,PublisherTable.Defs.Columns.Name, DataGridColumnSortBehavior.AlwaysAscending);

            Commands.Add("Select", RunSelectCommand, (o) => IsSelectedRowAccessible);
            FilterPrompt = Strings.FilterPromptPublisher;
            SelectedPublisherId = -1;
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the filter text has changed to set the filter on the underlying data.
        /// </summary>
        /// <param name="text">The filter text.</param>
        protected override void OnFilterTextChanged(string text)
        {
            DataView.RowFilter = string.Format("{0} LIKE '%{1}%'", PublisherTable.Defs.Columns.Name, text);
        }

        #endregion

        /************************************************************************/
        
        #region Private methods
        private void RunSelectCommand(object o)
        {
            if (SelectedPrimaryKey != null)
            {
                SelectedPublisherId = (long)SelectedPrimaryKey;
            }
            Owner.Close();
        }
        #endregion
    }
}
