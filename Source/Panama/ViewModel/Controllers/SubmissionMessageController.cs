using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Database.SQLite;
using SysProps = Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties;
using Restless.Tools.Utility;
using System.Windows;
using Restless.App.Panama.Configuration;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Submission message controller. Handles messages related to a submission.
    /// </summary>
    public class SubmissionMessageController : SubmissionController
    {
        #region Private
        private StringToCleanStringConverter converter;
        private bool isMapiMessage;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a boolean value that indicates if the selected message is a Mapi message.
        /// </summary>
        public bool IsMapiMessage
        {
            get { return isMapiMessage; }
            private set
            {
                isMapiMessage = value;
                OnPropertyChanged("IsMapiMessage");
            }
        }

        /// <summary>
        /// Gets the message text (cleaned up)
        /// </summary>
        public string MessageText
        {
            get
            {
                if (SelectedRow != null)
                {
                    return converter.Convert(SelectedRow[SubmissionMessageTable.Defs.Columns.Body].ToString(), StringToCleanStringOptions.All);
                }
                return null;
            }
        }
        #endregion

        /************************************************************************/

        #region Protected properties
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionMessageController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public SubmissionMessageController(SubmissionViewModel owner)
            : base(owner)
        {
            AssignDataViewFrom(DatabaseController.Instance.GetTable<SubmissionMessageTable>());
            DataView.RowFilter = String.Format("{0}=-1", SubmissionMessageTable.Defs.Columns.BatchId);
            DataView.Sort = String.Format("{0} DESC", SubmissionMessageTable.Defs.Columns.Received);
            //Columns.Create("BT", SubmissionMessageTable.Defs.Columns.BodyFormat).MakeFixedWidth(FixedWidth.MediumNumeric);
            Columns.SetDefaultSort(Columns.Create("Date", SubmissionMessageTable.Defs.Columns.Received).MakeDate(), ListSortDirection.Descending);
            Columns.Create("Subject", SubmissionMessageTable.Defs.Columns.Display);
            HeaderPreface = Strings.HeaderMessages;
            converter = new StringToCleanStringConverter();
            Owner.RawCommands.Add("SelectMessage", RunSelectMessageCommand);
            Owner.RawCommands.Add("RemoveMessage", RunRemoveMessageCommand, (o) => SelectedItem != null);
        }
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the selected item on the associated data grid has changed.
        /// </summary>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            OnPropertyChanged(nameof(MessageText));
            IsMapiMessage = false;
            if (SelectedRow != null)
            {
                string protocol = SelectedRow[SubmissionMessageTable.Defs.Columns.Protocol].ToString();
                IsMapiMessage = protocol == SubmissionMessageTable.Defs.Values.Protocol.Mapi;
            }
        }

        /// <summary>
        /// Called by the <see cref=" ControllerBase{VM,T}.Owner"/> of this controller
        /// in order to update the controller values.
        /// </summary>
        protected override void OnUpdate()
        {
            Int64 id = GetOwnerSelectedPrimaryId();
            DataView.RowFilter = String.Format("{0}={1}", SubmissionMessageTable.Defs.Columns.BatchId, id);
        }

        /// <summary>
        /// Runs the <see cref="DataGridViewModel{T}.OpenRowCommand"/> to open the selected message.
        /// </summary>
        /// <param name="item">The <see cref="DataRowView"/> object of the selected row.</param>
        /// <remarks>
        /// This method only opens a message if it is a mapi reference. Other messages are stored in the
        /// <see cref="SubmissionMessageTable"/> directly, and are displayed in a text box.
        /// </remarks>
        protected override void RunOpenRowCommand(object item)
        {
            if (item is DataRowView view)
            {
                string entryId = view.Row[SubmissionMessageTable.Defs.Columns.EntryId].ToString();
                if (IsMapiMessage)
                {
                    string url = $"{SubmissionMessageTable.Defs.Values.Protocol.Mapi}{Config.FolderMapi}{entryId}";
                    OpenHelper.OpenFile(url);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunSelectMessageCommand(object o)
        {
            if (String.IsNullOrEmpty(Config.Instance.FolderMapi))
            {
                Messages.ShowError(Strings.InvalidOpMapiFolderNotSet);
                return;
            }
            var ops = new MessageSelectOptions(MessageSelectMode.Message, Config.Instance.FolderMapi);
            var w = WindowFactory.MessageSelect.Create(Strings.CaptionSelectSubmissionMessage, ops);
            w.ShowDialog();

            if (w.GetValue(WindowViewModel.ViewModelProperty) is MessageSelectWindowViewModel vm)
            {
                if (vm.SelectedItems != null && Owner.SelectedPrimaryKey != null)
                {
                    string header = $"{SubmissionMessageTable.Defs.Values.Protocol.Mapi}{Config.FolderMapi}";
                    Int64 batchId = (Int64)Owner.SelectedPrimaryKey;
                    var table = DatabaseController.Instance.GetTable<SubmissionMessageTable>();
                    foreach (var item in vm.SelectedItems)
                    {
                        string url = item.Values[SysProps.System.ItemUrl].ToString().Substring(header.Length);
                        table.Add
                            (
                                batchId,
                                item.Values[SysProps.System.Subject].ToString(),
                                SubmissionMessageTable.Defs.Values.Protocol.Mapi,
                                url,
                                item.Values[SysProps.System.Message.DateReceived],
                                item.Values[SysProps.System.Message.DateSent],
                                item.Values[SysProps.System.Message.ToName].ToString(),
                                item.Values[SysProps.System.Message.ToAddress].ToString(),
                                item.Values[SysProps.System.Message.FromName].ToString(),
                                item.Values[SysProps.System.Message.FromAddress].ToString()
                            );
                    }
                }
            }
        }

        private void RunRemoveMessageCommand(object o)
        {
            if (SelectedRow != null && Messages.ShowYesNo(Strings.ConfirmationRemoveSubmissionMessage))
            {
                SelectedRow.Delete();
                DatabaseController.Instance.GetTable<SubmissionMessageTable>().Save();
            }
        }
        #endregion

    }
}
