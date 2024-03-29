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
using Columns = Restless.Panama.Database.Tables.TitleVersionTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="TitleVersionTable"/>.
    /// </summary>
    public class TitleVersionRow : RowObjectBase<TitleVersionTable>
    {
        #region Public properties
        /// <summary>
        /// Gets the id for this row object.
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets the title id for this row object.
        /// </summary>
        public long TitleId => GetInt64(Columns.TitleId);

        /// <summary>
        /// Gets or sets the document type for this row object.
        /// </summary>
        public long DocType
        {
            get => GetInt64(Columns.DocType);
            set => SetValue(Columns.DocType, value);
        }

        /// <summary>
        /// Gets or sets the file name for this row object.
        /// </summary>
        public string FileName
        {
            get => GetString(Columns.FileName);
            set => SetValue(Columns.FileName, value);
        }

        /// <summary>
        /// Gets or sets the note for this row object.
        /// </summary>
        public string Note
        {
            get => GetString(Columns.Note);
            set => SetValue(Columns.Note, value);
        }

        /// <summary>
        /// Gets or sets the updated value for this row object.
        /// </summary>
        public DateTime Updated
        {
            get => GetDateTime(Columns.Updated);
            set => SetValue(Columns.Updated, value);
        }

        /// <summary>
        /// Gets or sets the size for this row object.
        /// </summary>
        public long Size
        {
            get => GetInt64(Columns.Size);
            set => SetValue(Columns.Size, value);
        }

        /// <summary>
        /// Gets the version for this row object.
        /// </summary>
        public long Version
        {
            get => GetInt64(Columns.Version);
            internal set => SetValue(Columns.Version, value);
        }

        /// <summary>
        /// Gets the revision
        /// </summary>
        public long Revision
        {
            get => GetInt64(Columns.Revision);
            internal set => SetValue(Columns.Revision, value);
        }

        /// <summary>
        /// Gets or sets the language id.
        /// </summary>
        public string LanguageId
        {
            get => GetString(Columns.LangId);
            set => SetValue(Columns.LangId, value);
        }

        /// <summary>
        /// Gets or sets the word count for this row object.
        /// </summary>
        public long WordCount
        {
            get => GetInt64(Columns.WordCount);
            set => SetValue(Columns.WordCount, value);
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
        /// Initializes a new instance of the <see cref="TitleVersionRow"/> class.
        /// </summary>
        /// <param name="row">The data row</param>
        public TitleVersionRow(DataRow row) : base(row)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
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
        /// require synchronization with those of <see cref="Info"/>.
        /// Checks last updated date and size.
        /// </summary>
        /// <returns>true if synchronization needed; otherwise, false</returns>
        public bool RequireSynchonization()
        {
            return Info != null && (Updated != Info.LastWriteTimeUtc || Size != Info.Length);
        }

        /// <summary>
        /// Gets a boolean value that indicates whether the properties of this instance
        /// require synchronization with those of <see cref="Info"/>.
        /// Checks last updated date, size, and the specified word count.
        /// </summary>
        /// <param name="wordCount">The word count</param>
        /// <returns>true if synchronization needed; otherwise, false</returns>
        public bool RequireSynchonization(long wordCount)
        {
            return RequireSynchonization() || WordCount != wordCount;
        }

        /// <summary>
        /// Sets the properties of this object to be synchronized with those of <see cref="Info"/>
        /// and the specified word count.
        /// </summary>
        /// <param name="wordCount">The word count</param>
        public void Synchronize(long wordCount)
        {
            if (Info != null)
            {
                Updated = Info.LastWriteTimeUtc;
                Size = Info.Length;
                WordCount = wordCount;
            }
        }
        #endregion
    }
}