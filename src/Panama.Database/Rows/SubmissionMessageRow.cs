/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.SubmissionMessageTable.Defs.Columns;
using Values = Restless.Panama.Database.Tables.SubmissionMessageTable.Defs.Values;


namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="SubmissionMessageTable"/>.
    /// </summary>
    public class SubmissionMessageRow : RowObjectBase<SubmissionMessageTable>
    {
        #region Public properties
        /// <summary>
        /// Gets the id for this row object.
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets the batch id for this row object.
        /// </summary>
        public long BatchId => GetInt64(Columns.BatchId);

        /// <summary>
        /// Gets the protocol
        /// </summary>
        public string Protocol => GetString(Columns.Protocol);

        /// <summary>
        /// Gets the entry id
        /// </summary>
        public string EntryId => GetString(Columns.EntryId);

        /// <summary>
        /// Gets the message id.
        /// </summary>
        public string MessageId => GetString(Columns.MessageId);

        /// <summary>
        /// Gets the message date.
        /// </summary>
        public DateTime MessageDate => GetDateTime(Columns.MessageDate);

        /// <summary>
        /// Gets the sender name.
        /// </summary>
        public string SenderName => GetString(Columns.SenderName);

        /// <summary>
        /// Gets the sender email
        /// </summary>
        public string SenderEmail => GetString(Columns.SenderEmail);

        /// <summary>
        /// Gets the sender email and name
        /// </summary>
        public string SenderFull => GetString(Columns.Calculated.SenderFull);

        /// <summary>
        /// Gets the recipient name.
        /// </summary>
        public string RecipientName => GetString(Columns.RecipientName);

        /// <summary>
        /// Gets the recipient email
        /// </summary>
        public string RecipientEmail => GetString(Columns.RecipientEmail);

        /// <summary>
        /// Gets the receipient email and name
        /// </summary>
        public string RecipientFull => GetString(Columns.Calculated.RecipientFull);

        /// <summary>
        /// Gets the subject
        /// </summary>
        public string Subject => GetString(Columns.Subject);

        /// <summary>
        /// Gets or sets the display name
        /// </summary>
        public string Display
        {
            get => GetString(Columns.Display);
            set => SetValue(Columns.Display, value);
        }

        /// <summary>
        /// Gets the body format
        /// </summary>
        public long BodyFormat => GetInt64(Columns.BodyFormat);

        /// <summary>
        /// Gets the message body
        /// </summary>
        public string Body => GetString(Columns.Body);

        /// <summary>
        /// Gets a boolean value that indicates if the message protocol is file system
        /// </summary>
        public bool IsFileSystem => Protocol == Values.Protocol.FileSystem;

        /// <summary>
        /// Gets a boolean value that indicates if the message protocol is mapi
        /// </summary>
        public bool IsMapi => Protocol == Values.Protocol.Mapi;

        /// <summary>
        /// Gets a boolean value that indicates if the message protocol is database
        /// </summary>
        public bool IsDatabase => Protocol == Values.Protocol.Database;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionMessageRow"/> class.
        /// </summary>
        /// <param name="row">The data row</param>
        public SubmissionMessageRow(DataRow row) : base(row)
        {
        }

        /// <summary>
        /// Creates a new <see cref="SubmissionMessageRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new tag row, or null.</returns>
        public static SubmissionMessageRow Create(DataRow row)
        {
            return row != null ? new SubmissionMessageRow(row) : null;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Updates <see cref="EntryId"/> with the specified value
        /// </summary>
        /// <param name="value">The updated value</param>
        public void UpdateEntryId(string value)
        {
            Row[Columns.EntryId] = value;
        }
        #endregion
    }
}