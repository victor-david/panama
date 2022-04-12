/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Resources;
using Restless.Panama.View;
using Restless.Toolkit.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the view model logic for the <see cref="SubmissionMessageSelectWindow"/>.
    /// </summary>
    public class SubmissionMessageSelectWindowViewModel : DataViewModel<MimeKitMessage>, IWindowOwner
    {
        #region Private
        private readonly ObservableCollection<MimeKitMessage> messageCollection;
        #endregion

        /************************************************************************/

        #region Properties
        public int SelectedFilterValue
        {
            get => Config.SubmissionMessageDisplay;
            set
            {
                Config.SubmissionMessageDisplay = value;
                ListView.Refresh();
            }
        }
        /// <summary>
        /// Gets the list of messages that were selected by the user
        /// </summary>
        public List<MimeKitMessage> SelectedMessages
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region IWindowOwner
        /// <summary>
        /// Gets or sets the window owner. Set in <see cref="WindowFactory"/>
        /// </summary>
        public Window WindowOwner { get; set; }

        /// <summary>
        /// Gets or sets a command to close the window. Set in <see cref="WindowFactory"/>
        /// </summary>
        public ICommand CloseWindowCommand { get; set; }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionMessageSelectWindowViewModel"/> class.
        /// </summary>
        public SubmissionMessageSelectWindowViewModel()
        {
            Columns.CreateResource<BooleanToPathConverter>("E", nameof(MimeKitMessage.IsError), ResourceKeys.Icon.SquareSmallRedIconKey)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(Strings.TooltipMessageError);

            Columns.CreateResource<BooleanToPathConverter>("U", nameof(MimeKitMessage.InUse), ResourceKeys.Icon.SquareSmallBlueIconKey)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(Strings.TooltipMessageInUse);

            Columns.SetDefaultSort(
                Columns.Create("Date", nameof(MimeKitMessage.MessageDateUtc)).MakeDate(),
                ListSortDirection.Descending);

            Columns.Create("From", nameof(MimeKitMessage.FromName));
            Columns.Create("Subject", nameof(MimeKitMessage.Subject));

            Commands.Add("Select", RunSelectCommand, p => SelectedCount > 0);

            SelectedMessages = new List<MimeKitMessage>();

            messageCollection = new ObservableCollection<MimeKitMessage>();

            PopulateMessageCollection();

            InitListView(messageCollection);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override int OnDataRowCompare(MimeKitMessage item1, MimeKitMessage item2)
        {
            return DateTime.Compare(item2.MessageDateUtc, item1.MessageDateUtc);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(MimeKitMessage item)
        {
            return SelectedFilterValue switch
            {
                0 => true,
                1 => !item.InUse,
                _ => DateTime.Compare(DateTime.UtcNow, item.MessageDateUtc.AddDays(SelectedFilterValue)) < 0,
            };
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void PopulateMessageCollection()
        {
            messageCollection.Clear();
            foreach (string file in Directory.EnumerateFiles(Config.FolderSubmissionMessage, "*.eml"))
            {
                messageCollection.Add(new MimeKitMessage(file));
            }
        }

        private void RunSelectCommand(object parm)
        {
            PopulateSelectedMessages();
            if (WindowOwner != null && SelectedMessages.Count > 0)
            {
                WindowOwner.DialogResult = true;
                WindowOwner.Close();
            }
        }

        private void PopulateSelectedMessages()
        {
            SelectedMessages.Clear();
            foreach (MimeKitMessage message in SelectedItems.OfType<MimeKitMessage>())
            {
                SelectedMessages.Add(message);
            }
        }
        #endregion
    }
}