using Restless.Panama.Core;
using Restless.Panama.Core.Converters;
using Restless.Panama.Resources;
using Restless.Panama.View;
using Restless.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the view model logic for the <see cref="SubmissionMessageSelectWindow"/>.
    /// </summary>
    public class SubmissionMessageSelectWindowViewModel : DataViewModel<MimeKitMessage>, IWindow
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
        public Window Window { get; set; }

        /// <summary>
        /// Gets or sets a command to close the window. Set in <see cref="WindowFactory"/>
        /// </summary>
        public ICommand CloseWindowCommand { get; set; }

        /// <summary>
        /// Called when the window is closing. Established in <see cref="WindowFactory"/>
        /// </summary>
        /// <param name="e">The event args</param>
        public void OnWindowClosing(CancelEventArgs e)
        {
        }

        /// <summary>
        /// Called when the window has closed. Established in <see cref="WindowFactory"/>
        /// </summary>
        public void OnWindowClosed()
        {
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionMessageSelectWindowViewModel"/> class.
        /// </summary>
        public SubmissionMessageSelectWindowViewModel()
        {
            Columns.CreateResource<BooleanToResourceConverter>(Header.ErrorShort, nameof(MimeKitMessage.IsError), ResourceKeys.Icon.IconError)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W034)
                .AddToolTip(ToolTip.MessageError);

            Columns.CreateResource<BooleanToResourceConverter>(Header.InUseShort, nameof(MimeKitMessage.InUse), ResourceKeys.Icon.IconInUse)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W034)
                .AddToolTip(ToolTip.MessageInUse);

            Columns.Create(Header.Date, nameof(MimeKitMessage.MessageDateUtc))
                .MakeDate()
                .MakeInitialSortDescending();

            Columns.Create(Header.From, nameof(MimeKitMessage.FromName));
            Columns.Create(Header.Subject, nameof(MimeKitMessage.Subject));

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
            if (Window != null && SelectedMessages.Count > 0)
            {
                Window.DialogResult = true;
                Window.Close();
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