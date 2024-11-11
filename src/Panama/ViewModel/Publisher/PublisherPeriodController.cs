using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using System.Data;
using TableColumns = Restless.Panama.Database.Tables.SubmissionPeriodTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides logic for handling the submission periods that are associated with a publisher.
    /// </summary>
    public class PublisherPeriodController : BaseController<PublisherViewModel, SubmissionPeriodTable>
    {
        #region Private
        private SubmissionPeriodRow selectedPeriod;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <inheritdoc/>
        public override bool AddCommandEnabled => true;

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => SelectedPeriod != null;

        /// <summary>
        /// Get the selected period
        /// </summary>
        public SubmissionPeriodRow SelectedPeriod
        {
            get => selectedPeriod;
            private set => SetProperty(ref selectedPeriod, value);
        }

        /// <summary>
        /// Gets a friendly string that displays the date range in effect
        /// when adding a submission period.
        /// </summary>
        public string SelectedDisplay
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherPeriodController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public PublisherPeriodController(PublisherViewModel owner) : base(owner)
        {
            Columns.Create(Header.Id, TableColumns.Id)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create<MonthDayMultiConverter>(Header.Start, TableColumns.MonthStart, TableColumns.DayStart);
            Columns.Create<MonthDayMultiConverter>(Header.End, TableColumns.MonthEnd, TableColumns.DayEnd);
            Columns.Create(Header.Note, TableColumns.Notes).MakeSingleLine();

            MenuItems.AddItem(Menu.AddSubmissionPeriod, AddCommand).AddIconResource(ResourceKeys.Icon.IconAdd);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Menu.RemoveSubmissionPeriod, DeleteCommand).AddIconResource(ResourceKeys.Icon.IconDelete);

            Commands.Add("MakeAllYear", p =>RunMakeAllYearCommand(), p => !(SelectedPeriod?.IsAllYear ?? true));
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedPeriod = SubmissionPeriodRow.Create(SelectedRow);
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareLong(item1, item2, TableColumns.MonthStart);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return (long)item[TableColumns.PublisherId] == (Owner?.SelectedPublisher?.Id ?? 0);
        }

        /// <inheritdoc/>
        protected override void RunAddCommand()
        {
            if (MessageWindow.ShowContinueCancel(Confirm.AddSubmissionPeriod))
            {
                Table.AddSubmissionPeriod(Owner.SelectedPublisher.Id);
                Update();
            }
        }

        /// <inheritdoc/>
        protected override void RunDeleteCommand()
        {
            if (MessageWindow.ShowYesNo(Confirm.RemoveSubmissionPeriod))
            {
                // Call the specialized Table method instead of DeleteSelectedRow() to update other records
                Table.DeleteSubmissionPeriod(SelectedRow);
                Update();
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunMakeAllYearCommand()
        {
            if (SelectedPeriod != null && MessageWindow.ShowContinueCancel(Confirm.ResetSubmissionPeriod))
            {
                SelectedPeriod.MakeAllYear();
                OnPropertyChanged(nameof(SelectedPeriod));
            }
        }
        #endregion
    }
}