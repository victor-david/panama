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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using TableColumns = Restless.Panama.Database.Tables.SubmissionMessageTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Submission message controller. Handles messages related to a submission.
    /// </summary>
    public class SubmissionMessageController : BaseController<SubmissionViewModel, SubmissionMessageTable>
    {
        #region Private
        private readonly StringToCleanStringConverter messageTextConverter;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        public override bool AddCommandEnabled => true;

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => false; // TODO

        /// <summary>
        /// Gets the message text (cleaned up)
        /// </summary>
        public string MessageText => GetMessageText();

        /// <summary>
        /// Gets a string representation of the message sender.
        /// </summary>
        public string From => GetAddress(TableColumns.SenderName, TableColumns.SenderEmail);

        /// <summary>
        /// Gets a string representation of the message recipient.
        /// </summary>
        public string To => GetAddress(TableColumns.RecipientName, TableColumns.RecipientEmail);
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionMessageController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public SubmissionMessageController(SubmissionViewModel owner) : base(owner)
        {
            Columns.Create("Id", TableColumns.Id).MakeFixedWidth(FixedWidth.W042);
            Columns.SetDefaultSort(Columns.Create("Date", TableColumns.MessageDate).MakeDate(), ListSortDirection.Descending);
            Columns.Create("Type", TableColumns.Protocol).MakeFixedWidth(FixedWidth.W048);
            Columns.Create("Subject", TableColumns.Display);

            messageTextConverter = new StringToCleanStringConverter();
            
            MenuItems.AddItem(Strings.MenuItemAddSubmissionMessage, AddCommand)
                .AddIconResource(ResourceKeys.Icon.PlusIconKey);

            MenuItems.AddItem(Strings.MenuItemOpenItemOrDoubleClick, OpenRowCommand)
                .AddIconResource(ResourceKeys.Icon.ChevronRightIconKey);

            MenuItems.AddSeparator();

            MenuItems.AddItem(Strings.MenuItemRemoveSubmissionMessage, DeleteCommand)
                .AddIconResource(ResourceKeys.Icon.XMediumIconKey);
        }
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

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareDateTime(item2, item1, TableColumns.MessageDate);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return (long)item[TableColumns.BatchId] == (Owner?.SelectedBatch?.Id ?? 0);
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            ListView.Refresh();
        }

        /// <inheritdoc/>
        protected override void RunAddCommand()
        {
            RunAddCommandPrivate();
        }

        /// <inheritdoc/>
        protected override void RunDeleteCommand()
        {
            if (SelectedRow != null && MessageWindow.ShowYesNo(Strings.ConfirmationRemoveSubmissionMessage))
            {
                DeleteSelectedRow();
            }
        }

        /// <summary>
        /// Runs the <see cref="DataRowViewModel{T}.OpenRowCommand"/> to open the selected message.
        /// </summary>
        /// <remarks>
        /// This method only opens a message if it is a mapi reference or a file system reference.
        /// Other messages (older) are stored in the <see cref="SubmissionMessageTable"/> directly.
        /// </remarks>
        protected override void RunOpenRowCommand()
        {
            // TODO
            //if (item is DataRowView view)
            //{
            //    string protocol = view.Row[SubmissionMessageTable.Defs.Columns.Protocol].ToString();
            //    string entryId = view.Row[SubmissionMessageTable.Defs.Columns.EntryId].ToString();
            //    switch (protocol)
            //    {
            //        case SubmissionMessageTable.Defs.Values.Protocol.Mapi:
            //            string url = $"{SubmissionMessageTable.Defs.Values.Protocol.Mapi}{Config.FolderMapi}{entryId}";
            //            OpenHelper.OpenFile(url);
            //            break;
            //        case SubmissionMessageTable.Defs.Values.Protocol.FileSystem:
            //            OpenHelper.OpenFile(Path.Combine(Config.FolderSubmissionMessage, entryId));
            //            break;
            //    }
            //}
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunAddCommandPrivate()
        {
            if (!Directory.Exists(Config.Instance.FolderSubmissionMessage))
            {
                MessageWindow.ShowError(Strings.InvalidOpSubmissionMessageFolderNotSet);
                return;
            }

            if (WindowFactory.SubmissionMessageSelect.Create().GetMessages() is List<MimeKitMessage> messages)
            {

            }
            //var window = WindowFactory.SubmissionMessageSelect.Create();
            //window.ShowDialog();

            //if (window.DataContext is SubmissionMessageSelectWindowViewModel vm)
            //{
            //    if (vm.SelectedItems != null && Owner.SelectedPrimaryKey != null)
            //    {
            //        long batchId = (long)Owner.SelectedPrimaryKey;

            //        foreach (MimeKitMessage item in vm.SelectedItems.Where((m) => !m.IsError))
            //        {
            //            Table.Add
            //            (
            //                 batchId,
            //                 item.Subject,
            //                 SubmissionMessageTable.Defs.Values.Protocol.FileSystem, Path.GetFileName(item.File),
            //                 item.MessageId, item.MessageDateUtc,
            //                 item.ToName, item.ToEmail,
            //                 item.FromName, item.FromEmail);
            //        }
            //    }
            //}
        }

        private void RunViewMessageFileCommand(object parm)
        {
            if (string.IsNullOrEmpty(Config.TextViewerFile))
            {
                Messages.ShowError(Strings.InvalidOpTextViewerNotSet);
                return;
            }
            string file = Path.Combine(Config.FolderSubmissionMessage, SelectedRow[TableColumns.EntryId].ToString());
            OpenHelper.OpenFile(Config.TextViewerFile, file);

        }

        private bool CanRunViewMessageFileCommand(object parm)
        {
            if (IsSelectedRowAccessible)
            {
                return
                    SelectedRow[TableColumns.Protocol].ToString() == SubmissionMessageTable.Defs.Values.Protocol.FileSystem;
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
                switch (SelectedRow[TableColumns.Protocol].ToString())
                {
                    case SubmissionMessageTable.Defs.Values.Protocol.Database:
                        return messageTextConverter.Convert(SelectedRow[TableColumns.Body].ToString(), StringToCleanStringOptions.RemoveHtml);
                    case SubmissionMessageTable.Defs.Values.Protocol.Mapi:
                        return Strings.InvalidOpCannotDisplayMapi;
                    case SubmissionMessageTable.Defs.Values.Protocol.FileSystem:
                        string file = SelectedRow[TableColumns.EntryId].ToString();
                        MimeKitMessage msg = new MimeKitMessage(Path.Combine(Config.FolderSubmissionMessage, file));
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