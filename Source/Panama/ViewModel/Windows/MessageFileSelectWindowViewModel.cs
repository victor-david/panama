using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Search;
using Restless.Tools.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
//using SysProps = Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the view model logic for the <see cref="View.MessageFileSelectWindow"/>.
    /// </summary>
    public class MessageFileSelectWindowViewModel : WindowDataGridViewModel<DummyTable>
    {
        #region Private
        private readonly string folder;
        private readonly ObservableCollection<Message> resultsView;
        private IList selectedDataGridItems;
        #endregion

        /************************************************************************/


        /// <summary>
        /// Represents a single message
        /// </summary>
        public class Message
        {
            /// <summary>
            /// Gets the file name for this message.
            /// </summary>
            public string File
            {
                get;
            }

            /// <summary>
            /// Gets the message id.
            /// </summary>
            public string MessageId
            {
                get;
            }

            /// <summary>
            /// Gets the message date
            /// </summary>
            public DateTime MessageDate
            {
                get;
            }

            /// <summary>
            /// Gets the name of the sender.
            /// </summary>
            public string FromName
            {
                get;
            }

            /// <summary>
            /// Gets the email address of the sender.
            /// </summary>
            public string FromEmail
            {
                get;
            }

            /// <summary>
            /// Gets the name of the recipient
            /// </summary>
            public string ToName
            {
                get;
            }

            /// <summary>
            /// Gets the email address of the recipient.
            /// </summary>
            public string ToEmail
            {
                get;
            }

            /// <summary>
            /// Gets the subject.
            /// </summary>
            public string Subject
            {
                get;
            }

            /// <summary>
            /// Gets a boolean value that indicates if the parse failed.
            /// </summary>
            public bool IsError
            {
                get;
            }

            /// <summary>
            /// Gets a boolean value that indicates if <see cref="File"/> is already in use
            /// in <see cref="SubmissionMessageTable"/>.
            /// </summary>
            public bool InUse
            {
                get;
            }

            /// <summary>
            /// Gets the parse exception, or null if none.
            /// </summary>
            public Exception ParseException
            {
                get;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Message"/> class.
            /// </summary>
            /// <param name="file">The file name</param>
            public Message(string file)
            {
                try
                {
                    File = file;
                    var msg = MimeKit.MimeMessage.Load(file);
                    MessageId = msg.MessageId;
                    MessageDate = msg.Date.UtcDateTime;
                    FromName = msg.From[0].Name;
                    FromEmail = ((MimeKit.MailboxAddress)msg.From[0]).Address;
                    ToName = msg.To[0].Name;
                    ToEmail = ((MimeKit.MailboxAddress)msg.To[0]).Address;

                    Subject = msg.Subject;
                    if (string.IsNullOrEmpty(Subject))
                    {
                        Subject = "(no subject)";
                    }
                    if (string.IsNullOrEmpty(FromName))
                    {
                        FromName = FromEmail;
                    }
                    if (string.IsNullOrEmpty(ToName))
                    {
                        ToName = ToEmail;
                    }
                    IsError = false;
                    InUse = DatabaseController.Instance.GetTable<SubmissionMessageTable>().MessageInUse(SubmissionMessageTable.Defs.Values.Protocol.FileSystem, Path.GetFileName(File));
                }
                catch (Exception ex)
                {
                    ParseException = ex;
                    IsError = true;
                }
            }
        }


        #region Public properties
        /// <summary>
        /// Gets the list of items that were selected by the user, or null if nothing selected.
        /// </summary>
        public List<Message> SelectedItems
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
        /// Initializes a new instance of the <see cref="MessageFileSelectWindowViewModel"/> class.
        /// </summary>
        /// <param name="owner">The window that owns this view model.</param>
        /// <param name="folder">The folder where submission message file are stored.</param>
        public MessageFileSelectWindowViewModel(Window owner, string folder)
            :base(owner)
        {
            Validations.ValidateNullEmpty(folder, nameof(folder));
            this.folder = folder;
            resultsView = new ObservableCollection<Message>();
            MainSource.Source = resultsView;
            MainSource.SortDescriptions.Add(new SortDescription(nameof(Message.MessageDate), ListSortDirection.Descending));

            Columns.CreateImage<BooleanToImageConverter>("E", nameof(Message.IsError), "ImageExclamation")
                .AddToolTip(Strings.TooltipMessageError);

            Columns.CreateImage<BooleanToImageConverter>("U", nameof(Message.InUse))
                .AddToolTip(Strings.TooltipMessageInUse);

            var dateCol = Columns.Create("Date", nameof(Message.MessageDate)).MakeDate();
            Columns.Create("From", nameof(Message.FromName));
            Columns.Create("Subject", nameof(Message.Subject));
            Columns.SetDefaultSort(dateCol, ListSortDirection.Descending);
            Commands.Add("Select", RunSelectCommand, CanRunCommandIfRowSelected);
            Commands.Add("Cancel", (o) => Owner.Close());
            GetResults();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        #endregion

        /************************************************************************/

        #region Private methods

        private void GetResults()
        {
            resultsView.Clear();
            foreach (string file in Directory.EnumerateFiles(Config.FolderSubmissionMessage, "*.eml"))
            {
                resultsView.Add(new Message(file));
            }
        }

        private void RunSelectCommand(object o)
        {
            if (selectedDataGridItems != null && selectedDataGridItems.Count > 0)
            {
                SelectedItems = new List<Message>();
                foreach (var item in selectedDataGridItems.OfType<Message>())
                {
                    SelectedItems.Add(item);
                }
            }
            Owner.Close();
        }
        #endregion
    }
}
