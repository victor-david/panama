/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using System;
using System.IO;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents a single message
    /// </summary>
    public class MimeKitMessage
    {
        /// <summary>
        /// Provides an enumeration that describes the type of content held by <see cref="MessageText"/>.
        /// </summary>
        public enum MessageTextFormat
        {
            /// <summary>
            /// Unknown
            /// </summary>
            Unknown,
            /// <summary>
            /// Plain text
            /// </summary>
            Text,
            /// <summary>
            /// Html
            /// </summary>
            Html
        }

        #region Public properties
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
        /// Gets the UTC message date.
        /// </summary>
        public DateTime MessageDateUtc
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
        /// Gets the message text.
        /// </summary>
        public string MessageText
        {
            get;
        }

        /// <summary>
        /// Gets the format of <see cref="MessageText"/>.
        /// </summary>
        public MessageTextFormat TextFormat
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
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MimeKitMessage"/> class.
        /// </summary>
        /// <param name="file">The file name</param>
        public MimeKitMessage(string file)
        {
            try
            {
                File = file;
                var msg = MimeKit.MimeMessage.Load(file);
                MessageId = msg.MessageId;
                MessageDateUtc = msg.Date.UtcDateTime;

                var from = ExtractAddress(msg.From);
                FromName = from.Item1;
                FromEmail = from.Item2;

                var to = ExtractAddress(msg.To);
                ToName = to.Item1;
                ToEmail = to.Item2;

                Subject = msg.Subject;
                if (string.IsNullOrEmpty(Subject))
                {
                    Subject = "(no subject)";
                }

                TextFormat = MessageTextFormat.Unknown;
                if (!string.IsNullOrEmpty(msg.TextBody))
                {
                    MessageText = msg.TextBody;
                    TextFormat = MessageTextFormat.Text;
                }
                else if (!string.IsNullOrEmpty(msg.HtmlBody))
                {
                    MessageText = msg.HtmlBody;
                    TextFormat = MessageTextFormat.Html;
                }

                IsError = false;
                InUse = DatabaseController.Instance.GetTable<SubmissionMessageTable>().MessageInUse(SubmissionMessageTable.Defs.Values.Protocol.FileSystem, Path.GetFileName(File));
            }
            catch (Exception ex)
            {
                ParseException = ex;
                IsError = true;
                TextFormat = MessageTextFormat.Unknown;
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        /// <summary>
        /// Extracts the name and address from the specified address list.
        /// </summary>
        /// <param name="addrList">The address list</param>
        /// <returns>A Tuple object with name as the first item, email as the second item.</returns>
        private Tuple<string, string> ExtractAddress(MimeKit.InternetAddressList addrList)
        {
            string name = "(unknown)";
            string email = "(unknown)";
            if (addrList.Count > 0)
            {
                name = addrList[0].Name;
                email = ((MimeKit.MailboxAddress)addrList[0]).Address;
                if (string.IsNullOrEmpty(name))
                {
                    name = email;
                }
            }
            return Tuple.Create(name, email);
        }
        #endregion
    }
}