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
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.Utility;
using Restless.Toolkit.Utility;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Submission message controller. Handles messages related to a submission.
    /// </summary>
    public class SubmissionMessageController : SubmissionController
    {
        #region Private
        private readonly StringToCleanStringConverter messageTextConverter;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the message text (cleaned up)
        /// </summary>
        public string MessageText
        {
            get => GetMessageText();
        }

        /// <summary>
        /// Gets a string representation of the message sender.
        /// </summary>
        public string From
        {
            get => GetAddress(SubmissionMessageTable.Defs.Columns.SenderName, SubmissionMessageTable.Defs.Columns.SenderEmail);
        }

        /// <summary>
        /// Gets a string representation of the message recipient.
        /// </summary>
        public string To
        {
            get => GetAddress(SubmissionMessageTable.Defs.Columns.RecipientName, SubmissionMessageTable.Defs.Columns.RecipientEmail);
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
            DataView.RowFilter = string.Format("{0}=-1", SubmissionMessageTable.Defs.Columns.BatchId);
            DataView.Sort = $"{SubmissionMessageTable.Defs.Columns.MessageDate} DESC";
            Columns.Create("Id", SubmissionMessageTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.SetDefaultSort(Columns.Create("Date", SubmissionMessageTable.Defs.Columns.MessageDate).MakeDate(), ListSortDirection.Descending);
            Columns.Create("Type", SubmissionMessageTable.Defs.Columns.Protocol).MakeFixedWidth(FixedWidth.ShortString);
            Columns.Create("Subject", SubmissionMessageTable.Defs.Columns.Display);
            HeaderPreface = Strings.HeaderMessages;
            messageTextConverter = new StringToCleanStringConverter();
            Commands.Add("AddMessage", RunAddMessageCommand);
            Commands.Add("RemoveMessage", RunRemoveMessageCommand, (o) => IsSelectedRowAccessible);
            Commands.Add("ViewMessageFile", RunViewMessageFileCommand, CanRunViewMessageFileCommand);

            MenuItems.AddItem("View message file", Commands["ViewMessageFile"]).AddImageResource("ImageNoteMenu");
            MenuItems.AddSeparator();
            MenuItems.AddItem("Remove", Commands["RemoveMessage"]).AddImageResource("ImageDeleteMenu");
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
            OnPropertyChanged(nameof(From));
            OnPropertyChanged(nameof(To));
            OnPropertyChanged(nameof(MessageText));
        }

        /// <summary>
        /// Called by the <see cref=" ControllerBase{VM,T}.Owner"/> of this controller
        /// in order to update the controller values.
        /// </summary>
        protected override void OnUpdate()
        {
            long id = GetOwnerSelectedPrimaryId();
            DataView.RowFilter = string.Format("{0}={1}", SubmissionMessageTable.Defs.Columns.BatchId, id);
        }

        /// <summary>
        /// Runs the <see cref="DataGridViewModel{T}.OpenRowCommand"/> to open the selected message.
        /// </summary>
        /// <param name="item">The <see cref="DataRowView"/> object of the selected row.</param>
        /// <remarks>
        /// This method only opens a message if it is a mapi reference or a file system reference.
        /// Other messages (older) are stored in the <see cref="SubmissionMessageTable"/> directly.
        /// </remarks>
        protected override void RunOpenRowCommand(object item)
        {
            if (item is DataRowView view)
            {
                string protocol = view.Row[SubmissionMessageTable.Defs.Columns.Protocol].ToString();
                string entryId = view.Row[SubmissionMessageTable.Defs.Columns.EntryId].ToString();
                switch (protocol)
                {
                    case SubmissionMessageTable.Defs.Values.Protocol.Mapi:
                        string url = $"{SubmissionMessageTable.Defs.Values.Protocol.Mapi}{Config.FolderMapi}{entryId}";
                        OpenHelper.OpenFile(url);
                        break;
                    case SubmissionMessageTable.Defs.Values.Protocol.FileSystem:
                        OpenHelper.OpenFile(Path.Combine(Config.FolderSubmissionMessage, entryId));
                        break;
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunAddMessageCommand(object o)
        {
            string folder = Config.Instance.FolderSubmissionMessage;
            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
            {
                Messages.ShowError(Strings.InvalidOpSubmissionMessageFolderNotSet);
                return;
            }

            var window = WindowFactory.MessageFileSelect.Create(Strings.CaptionSelectSubmissionMessage, folder);
            window.ShowDialog();

            if (window.DataContext is MessageFileSelectWindowViewModel vm)
            {
                if (vm.SelectedItems != null && Owner.SelectedPrimaryKey != null)
                {
                    long batchId = (long)Owner.SelectedPrimaryKey;
                    var table = DatabaseController.Instance.GetTable<SubmissionMessageTable>();

                    foreach (MimeKitMessage item in vm.SelectedItems.Where((m)=>!m.IsError))
                    {
                        table.Add
                        (
                             batchId,
                             item.Subject,
                             SubmissionMessageTable.Defs.Values.Protocol.FileSystem, Path.GetFileName(item.File),
                             item.MessageId, item.MessageDateUtc,
                             item.ToName, item.ToEmail,
                             item.FromName, item.FromEmail);
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

        private void RunViewMessageFileCommand(object parm)
        {
            if (string.IsNullOrEmpty(Config.TextViewerFile))
            {
                Messages.ShowError(Strings.InvalidOpTextViewerNotSet);
                return;
            }
            string file = Path.Combine(Config.FolderSubmissionMessage, SelectedRow[SubmissionMessageTable.Defs.Columns.EntryId].ToString());
            OpenHelper.OpenFile(Config.TextViewerFile, file);

        }

        private bool CanRunViewMessageFileCommand(object parm)
        {
            if (IsSelectedRowAccessible)
            {
                return
                    SelectedRow[SubmissionMessageTable.Defs.Columns.Protocol].ToString() == SubmissionMessageTable.Defs.Values.Protocol.FileSystem;
            }
            return false;
        }

        private string GetAddress(string nameCol, string emailCol)
        {
            if (IsSelectedRowAccessible)
            {
                string name = SelectedRow[nameCol].ToString();
                string email = SelectedRow[emailCol].ToString();
                if (string.IsNullOrEmpty(name) || name == email)
                {
                    return $"<{email}>";
                }
                return $"{name} <{email}>";
            }
            return null;
        }

        private string GetMessageText()
        {
            if (IsSelectedRowAccessible)
            {
                switch (SelectedRow[SubmissionMessageTable.Defs.Columns.Protocol].ToString())
                {
                    case SubmissionMessageTable.Defs.Values.Protocol.Database:
                        return messageTextConverter.Convert(SelectedRow[SubmissionMessageTable.Defs.Columns.Body].ToString(), StringToCleanStringOptions.RemoveHtml);
                    case SubmissionMessageTable.Defs.Values.Protocol.Mapi:
                        return Strings.InvalidOpCannotDisplayMapi;
                    case SubmissionMessageTable.Defs.Values.Protocol.FileSystem:
                        string file = SelectedRow[SubmissionMessageTable.Defs.Columns.EntryId].ToString();
                        var msg = new MimeKitMessage(Path.Combine(Config.FolderSubmissionMessage, file));
                        if (msg.TextFormat == MimeKitMessage.MessageTextFormat.Unknown)
                        {
                            return "Message has unknown message format";
                        }
                        StringToCleanStringOptions ops = (msg.TextFormat == MimeKitMessage.MessageTextFormat.Text) ? StringToCleanStringOptions.None : StringToCleanStringOptions.All;
                        return messageTextConverter.Convert(msg.MessageText, ops);
                }
            }
            return null;
        }
        #endregion

    }
}