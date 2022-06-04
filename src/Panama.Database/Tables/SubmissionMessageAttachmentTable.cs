/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Core.Database.SQLite;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains submission message attachments. This table exists
    /// in the database, but is no longer used.
    /// </summary>
    public class SubmissionMessageAttachmentTable : Core.ApplicationTableBase
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
            public const string TableName = "submissionmessageattachment";

            /// <summary>
            /// Provides static column names for this table.
            /// </summary>
            public static class Columns
            {
                /// <summary>
                /// The name of the id column. This is the table's primary key.
                /// </summary>
                public const string Id = DefaultPrimaryKeyName;
                /// <summary>
                /// The name of the message id column. Holds the id of the message that owns an attachment.
                /// </summary>
                public const string MessageId = "messageid";
                /// <summary>
                /// The name of the display column. Holds the display name of an attchment.
                /// </summary>
                public const string Display = "display";
                /// <summary>
                /// The name of the file name column.
                /// </summary>
                public const string FileName = "filename";
                /// <summary>
                /// The name of the file size column.
                /// </summary>
                public const string FileSize = "filesize";
                /// <summary>
                /// The name of the type column.
                /// </summary>
                public const string Type = "type";
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionMessageAttachmentTable"/> class.
        /// </summary>
        public SubmissionMessageAttachmentTable() : base(Defs.TableName)
        {
        }
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
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Gets the column definitions for this table.
        /// </summary>
        /// <returns>A <see cref="ColumnDefinitionCollection"/>.</returns>
        protected override ColumnDefinitionCollection GetColumnDefinitions()
        {
            return new ColumnDefinitionCollection()
            {
                { Defs.Columns.Id, ColumnType.Integer, true },
                { Defs.Columns.MessageId, ColumnType.Integer, false, false, 0, IndexType.Index },
                { Defs.Columns.Display, ColumnType.Text },
                { Defs.Columns.FileName, ColumnType.Text },
                { Defs.Columns.FileSize, ColumnType.Integer },
                { Defs.Columns.Type, ColumnType.Integer, false, false, 0 },
            };
        }
        #endregion
    }
}