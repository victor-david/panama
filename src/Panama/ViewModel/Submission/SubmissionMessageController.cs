/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TableColumns = Restless.Panama.Database.Tables.SubmissionMessageTable.Defs.Columns;
using TableValues = Restless.Panama.Database.Tables.SubmissionMessageTable.Defs.Values;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Submission message controller. Handles messages related to a submission.
    /// </summary>
    public class SubmissionMessageController : BaseController<SubmissionViewModel, SubmissionMessageTable>
    {
        #region Private
        private SubmissionMessageRow selectedMessage;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        public override bool AddCommandEnabled => Owner.SelectedBatch != null;

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => IsSelectedRowAccessible;

        /// <summary>
        /// Gets the selected message.
        /// </summary>
        public SubmissionMessageRow SelectedMessage
        {
            get => selectedMessage;
            private set => SetProperty(ref selectedMessage, value);
        }

        /// <summary>
        /// Gets the message text (cleaned up)
        /// </summary>
        public string MessageText => GetMessageText();
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
            Columns.Create("Date", TableColumns.MessageDate)
                .MakeDate()
                .MakeInitialSortDescending();

            Columns.Create("Type", TableColumns.Protocol).MakeFixedWidth(FixedWidth.W048);
            Columns.Create("Subject", TableColumns.Display);
           
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
            SelectedMessage = SubmissionMessageRow.Create(SelectedRow);
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
        /// This method only opens a message if it is a file system reference.
        /// Other messages (older) are stored in the <see cref="SubmissionMessageTable"/> directly.
        /// </remarks>
        protected override void RunOpenRowCommand()
        {
            if (SelectedMessage?.IsFileSystem ?? false)
            {
                Open.SubmissionMessageFile(SelectedMessage.EntryId);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunAddCommandPrivate()
        {
            try
            {
                if (!Directory.Exists(Config.Instance.FolderSubmissionMessage))
                {
                    throw new IOException(Strings.InvalidOpSubmissionMessageFolderNotSet);
                }

                if (WindowFactory.SubmissionMessageSelect.Create().GetMessages() is List<MimeKitMessage> messages)
                {
                    foreach (MimeKitMessage message in messages.Where(m => !m.IsError))
                    {
                        Table.Add
                        (
                             Owner.SelectedBatch.Id,
                             message.Subject,
                             TableValues.Protocol.FileSystem,
                             Path.GetFileName(message.File),
                             message.MessageId, message.MessageDateUtc,
                             message.ToName, message.ToEmail,
                             message.FromName, message.FromEmail
                         );
                    }
                    Table.Save();
                    ListView.Refresh();
                }
            }

            catch (Exception ex)
            {
                MessageWindow.ShowError(ex.Message);
            }
        }

        private string GetMessageText()
        {
            if (SelectedMessage != null)
            {
                switch (SelectedMessage.Protocol)
                {
                    case TableValues.Protocol.Database:
                        return StringClean.Clean(SelectedMessage.Body, StringCleanOptions.RemoveHtml);

                    case TableValues.Protocol.Mapi:
                        return Strings.InvalidOpCannotDisplayMapi;

                    case TableValues.Protocol.FileSystem:
                        string file = SelectedMessage.EntryId;
                        MimeKitMessage msg = new(Path.Combine(Config.FolderSubmissionMessage, file));
                        if (msg.TextFormat == MimeKitMessage.MessageTextFormat.Unknown)
                        {
                            return "Message has unknown message format";
                        }

                        return StringClean.Clean(msg.MessageText, TextFormatToStringCleanOptions(msg.TextFormat));

                    default:
                        break;
                }
            }
            return null;
        }

        private StringCleanOptions TextFormatToStringCleanOptions(MimeKitMessage.MessageTextFormat format)
        {
            return (format == MimeKitMessage.MessageTextFormat.Text) ? StringCleanOptions.None : StringCleanOptions.All;
        }
        #endregion
    }
}