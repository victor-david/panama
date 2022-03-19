/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using System.IO;
using Columns = Restless.Panama.Database.Tables.SubmissionDocumentTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="SubmissionDocumentTable"/>.
    /// </summary>
    public class SubmissionDocumentRow : RowObjectBase<SubmissionDocumentTable>
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
        /// Gets or sets the title for this row object.
        /// </summary>
        public string Title
        {
            get => GetString(Columns.Title);
            set => SetValue(Columns.Title, value);
        }

        /// <summary>
        /// Gets the document type for this row object.
        /// </summary>
        public long DocType => GetInt64(Columns.DocType);

        /// <summary>
        /// Gets or sets the document id (usually a file name) for this row object.
        /// </summary>
        public string DocumentId
        {
            get => GetString(Columns.DocId);
            set => SetValue(Columns.DocId, value);
        }

        /// <summary>
        /// Gets or sets the updated date for this row object.
        /// </summary>
        public DateTime Updated
        {
            get => GetDateTime(Columns.Updated);
            set => SetValue(Columns.Updated, value);
        }

        /// <summary>
        /// Gets or sets the file size.
        /// </summary>
        public long Size
        {
            get => GetInt64(Columns.Size);
            set => SetValue(Columns.Size, value);
        }

        /// <summary>
        /// Gets the file information object for this row object.
        /// You must call the <see cref="SetFileInfo"/> method before accessing this property.
        /// </summary>
        public FileInfo Info
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionDocumentRow"/> class.
        /// </summary>
        /// <param name="row">The data row</param>
        public SubmissionDocumentRow(DataRow row) : base(row)
        {
        }
        #endregion

        /// <summary>
        /// Sets the <see cref="Info"/> property according to the specified full file name.
        /// </summary>
        /// <param name="fullName">The full path to the file.</param>
        public void SetFileInfo(string fullName)
        {
            Info = new FileInfo(fullName);
        }

        /// <summary>
        /// Gets a boolean value that indicates whether the properties of this instance
        /// require synchronization with those of <see cref="Info"/>
        /// </summary>
        /// <returns>true if synchronization needed; otherwise, false</returns>
        public bool RequireSynchonization()
        {
            return Info != null && (Updated != Info.LastWriteTimeUtc || Size != Info.Length);
        }

        /// <summary>
        /// Sets the properties of this object to be synchronized with those of <see cref="Info"/>
        /// </summary>
        public void Synchronize()
        {
            if (Info != null)
            {
                Updated = Info.LastWriteTimeUtc;
                Size = Info.Length;
            }
        }
    }
}