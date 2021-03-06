/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tools.Database.SQLite;
using System;
using System.Data;

namespace Restless.App.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains messages associated with a submission batch.
    /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class SubmissionMessageTable : TableBase
    {
        #region Public properties
        /// <summary>
        /// Provides static definitions for table properties such as column names and relation names.
        /// </summary>
        public static class Defs
        {
            /// <summary>
            /// Specifies the name of this table.
            /// </summary>
            public const string TableName = "submissionmessage";

            /// <summary>
            /// Provides static column names for this table.
            /// </summary>
            public static class Columns
            {
                /// <summary>
                /// The name of the id column. This is the table's primary key.
                /// </summary>
                public const string Id = "id";
                /// <summary>
                /// Protocol column. Holds the protocol used to access the message.
                /// </summary>
                public const string Protocol = "protocol";
                /// <summary>
                /// The name of the entry id column. Holds the path to the message.
                /// </summary>
                public const string EntryId = "entryid";
                /// <summary>
                /// The message id.
                /// </summary>
                public const string MessageId = "messageid";
                /// <summary>
                /// The message date.
                /// </summary>
                public const string MessageDate = "messagedate";
                /// <summary>
                /// The name of the batch id column.
                /// </summary>
                public const string BatchId = "submissionbatchid";
                ///// <summary>
                ///// The name of the outgoing column.
                ///// </summary>
                //public const string Outgoing = "outgoing";
                /// <summary>
                /// The name of the sender name column.
                /// </summary>
                public const string SenderName = "sendername";
                /// <summary>
                /// The name of the sender email column.
                /// </summary>
                public const string SenderEmail = "senderemail";
                /// <summary>
                /// The name of the recipient name column.
                /// </summary>
                public const string RecipientName = "recipientname";
                /// <summary>
                /// The name of the recipient email column.
                /// </summary>
                public const string RecipientEmail = "recipientemail";
                /// <summary>
                /// The name of the subject column.
                /// </summary>
                public const string Subject = "subject";
                /// <summary>
                /// The name of the display column.
                /// </summary>
                public const string Display = "display";
                ///// <summary>
                ///// The name of the sent column.
                ///// </summary>
                //public const string Sent = "sent";
                ///// <summary>
                ///// The name of the received column.
                ///// </summary>
                //public const string Received = "received";
                /// <summary>
                /// The name of the body format column.
                /// </summary>
                public const string BodyFormat = "bodyformat";
                /// <summary>
                /// The name of the body column. No longer used.
                /// </summary>
                public const string Body = "body";

                /// <summary>
                /// Provides static column names for columns that are calculated from other values.
                /// </summary>
                public static class Calculated
                {
                    /// <summary>
                    /// The name of the sender full name column. This calculated column combines sender name and email.
                    /// </summary>
                    public const string SenderFull = "SenderFull";
                    /// <summary>
                    /// The name of the recipient full name column. This calculated column combines recipient name and email.
                    /// </summary>
                    public const string RecipientFull = "RecipientFull";

                }
            }

            /// <summary>
            /// Provides static relation names.
            /// </summary>
            public static class Relations
            {
                /// <summary>
                /// The name of the relation that relates the <see cref="SubmissionMessageTable"/> to the <see cref="SubmissionMessageAttachmentTable"/>.
                /// </summary>
                public const string ToSubmissionMessageAttachment = "SubMessageToAttach";
            }

            /// <summary>
            /// Provides static values
            /// </summary>
            public static class Values
            {
                /// <summary>
                /// Provides static values used for <see cref="Columns.Protocol"/>.
                /// </summary>
                public static class Protocol
                {
                    /// <summary>
                    /// Protocol identifier for messages stored directly in the database.
                    /// </summary>
                    public const string Database = "db:";
                    /// <summary>
                    /// Protocol identifier for messages stored externally in MAPI store.
                    /// </summary>
                    public const string Mapi = "mapi:";
                    /// <summary>
                    /// Protocol identifier for messages stored externaly in the file system.
                    /// </summary>
                    public const string FileSystem = "file:";
                }
            }
        }

        /// <summary>
        /// Gets the column name of the primary key.
        /// </summary>
        public override string PrimaryKeyName
        {
            get => Defs.Columns.Id;
        }
        #endregion

        /************************************************************************/
        
        #region Constructor
        #pragma warning disable 1591
        public SubmissionMessageTable() : base(DatabaseController.Instance, Defs.TableName)
        {
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Loads the data from the database into the Rows collection for this table.
        /// </summary>
        public override void Load()
        {
            Load(null, string.Format("{0} DESC",Defs. Columns.Id));
        }

        /// <summary>
        /// Adds a message record.
        /// </summary>
        /// <param name="batchId">The batch id from the <see cref="SubmissionBatchTable"/> that owns this message.</param>
        /// <param name="subject">The message subject.</param>
        /// <param name="protocol">The protocol used to access this message</param>
        /// <param name="entryId">
        /// The entry id for the message.
        /// For an Outlook message, this is a MAPI reference to the Outlook data store.
        /// For an external .eml file, it's the name of the file without the path portion.
        /// </param>
        /// <param name="messageDate">The date / time of the message.</param>
        /// <param name="messageId">The message id</param>
        /// <param name="toName">The name of the message recipient.</param>
        /// <param name="toAddress">The email address of the message recipient.</param>
        /// <param name="fromName">The name of the message sender.</param>
        /// <param name="fromAddress">The email address of the message sender.</param>
        public void Add(long batchId, string subject, string protocol, string entryId, string messageId, DateTime messageDate, string toName, string toAddress, string fromName, string fromAddress)
        {
            DataRow row = NewRow();
            if (string.IsNullOrEmpty(subject))
            {
                subject = "(no subject)";
            }
            row[Defs.Columns.BatchId] = batchId;
            row[Defs.Columns.Body] = DBNull.Value;
            row[Defs.Columns.BodyFormat] = 0;
            row[Defs.Columns.Display] = subject;
            row[Defs.Columns.Protocol] = protocol;
            row[Defs.Columns.EntryId] = entryId;
            row[Defs.Columns.MessageId] = messageId;
            row[Defs.Columns.RecipientEmail] = toAddress;
            row[Defs.Columns.RecipientName] = toName;
            row[Defs.Columns.SenderEmail] = fromAddress;
            row[Defs.Columns.SenderName] = fromName;
            row[Defs.Columns.MessageDate] = messageDate;
            row[Defs.Columns.Subject] = subject;
            Rows.Add(row);
            Save();
        }

        /// <summary>
        /// Gets a boolean value that indicates if the specified protocol and entry id is currently in use.
        /// </summary>
        /// <param name="protocol">The protocol specifier</param>
        /// <param name="entryId">The entry id</param>
        /// <returns>true if protocol / entryid is in use; otherwise, false.</returns>
        public bool MessageInUse(string protocol, string entryId)
        {
            entryId = entryId.Replace("'", "''");
            DataRow[] rows = Select($"{Defs.Columns.Protocol}='{protocol}' AND {Defs.Columns.EntryId}='{entryId}'");
            return rows.Length > 0;
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Gets the DDL needed to create this table.
        /// </summary>
        /// <returns>A SQL string that describes how to create this table.</returns>
        protected override string GetDdl()
        {
            return Resources.Create.SubmissionMessage;
        }

        /// <summary>
        /// Enables this class to set column properties such as read only.
        /// </summary>
        protected override void SetColumnProperties()
        {
            SetColumnProperty(Columns[Defs.Columns.Id], DataColumnPropertyKey.ExcludeFromInsert, DataColumnPropertyKey.ExcludeFromUpdate, DataColumnPropertyKey.ReceiveInsertedId);
        }

        /// <summary>
        /// Establishes parent / child relationships with other tables.
        /// </summary>
        protected override void SetDataRelations()
        {
            CreateParentChildRelation<SubmissionMessageAttachmentTable>(Defs.Relations.ToSubmissionMessageAttachment, Defs.Columns.Id, SubmissionMessageAttachmentTable.Defs.Columns.MessageId);
        }

        /// <summary>
        /// Creates the <see cref="Defs.Columns.Calculated"/> columns for this table.
        /// </summary>
        protected override void UseDataRelations()
        {
            CreateExpressionColumn<string>(Defs.Columns.Calculated.SenderFull, string.Format("{0}+' ('+{1}+')'", Defs.Columns.SenderName, Defs.Columns.SenderEmail));
            CreateExpressionColumn<string>(Defs.Columns.Calculated.RecipientFull, string.Format("{0}+' ('+{1}+')'", Defs.Columns.RecipientName, Defs.Columns.RecipientEmail));
        }
        #endregion

        /************************************************************************/

        #region ITableImport and IColumnRowImporter implementation (commented out)
        //public bool PerformImport()
        //{
        //    return DatabaseImporter.Instance.ImportTable(this, this, "submission_message");
        //}

        //public string GetColumnName(string origColName)
        //{
        //    switch (origColName)
        //    {
        //        case "entry_id": return Defs.Columns.EntryId;
        //        case "sender_name": return Defs.Columns.SenderName;
        //        case "sender_email": return Defs.Columns.SenderEmail;
        //        case "recipient_name": return Defs.Columns.RecipientName;
        //        case "recipient_email": return Defs.Columns.RecipientEmail;
        //        case "sent_date": return Defs.Columns.Sent;
        //        case "received_date": return Defs.Columns.Received;
        //        case "body_format": return Defs.Columns.BodyFormat;
        //        default: return origColName;
        //    }
        //}

        //public bool IncludeColumn(string origColName)
        //{
        //    return true;
        //}

        //public bool GetRowConfirmation(System.Data.DataRow row)
        //{
        //    return true;
        //}
        #endregion



    }
}