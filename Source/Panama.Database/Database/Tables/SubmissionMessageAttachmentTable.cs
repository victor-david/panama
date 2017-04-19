using System;
using Restless.App.Panama.Database;
using Restless.Tools.Database.Generic;
using Restless.Tools.Database.SQLite;

namespace Restless.App.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains submission message attachments. This table exists
    /// in the database, but is no longer used.
    /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class SubmissionMessageAttachmentTable : TableBase
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
                public const string Id = "id";
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

        /// <summary>
        /// Gets the column name of the primary key.
        /// </summary>
        public override string PrimaryKeyName
        {
            get { return Defs.Columns.Id; }
        }
        #endregion

        /************************************************************************/
        
        #region Constructor
        #pragma warning disable 1591
        public SubmissionMessageAttachmentTable() : base(DatabaseController.Instance, Defs.TableName)
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
            Load(null, String.Format("{0} DESC",Defs. Columns.Id));
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
            return Resources.Create.SubmissionMessageAttachment;
        }

        /// <summary>
        /// Sets extended properties on certain columns. See the base implemntation <see cref="TableBase.SetColumnProperties"/> for more information.
        /// </summary>
        protected override void SetColumnProperties()
        {
            SetColumnProperty(Columns[Defs.Columns.Id], DataColumnPropertyKey.ExcludeFromInsert, DataColumnPropertyKey.ExcludeFromUpdate, DataColumnPropertyKey.ReceiveInsertedId);
        }
        #endregion

        /************************************************************************/

        #region ITableImport and IColumnRowImporter implementation (commented out)
        //public bool PerformImport()
        //{
        //    return DatabaseImporter.Instance.ImportTable(this, this, "submission_message_attachment");
        //}

        //public string GetColumnName(string origColName)
        //{
        //    switch (origColName)
        //    {
        //        case "message_id": return Defs.Columns.MessageId;
        //        case "display_name": return Defs.Columns.Display;
        //        case "file_name": return Defs.Columns.FileName;
        //        case "file_size": return Defs.Columns.FileSize;
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
