/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using Columns = Restless.Panama.Database.Tables.TitleTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="TitleTable"/>.
    /// </summary>
    public class TitleRow : DateRowObject<TitleTable>
    {

        #region Public properties
        /// <summary>
        /// Gets the default title value.
        /// </summary>
        public const string DefaultTitle = "(none)";

        /// <summary>
        /// Gets the id.
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get => GetString(Columns.Title);
            set => SetValue(Columns.Title, value.Trim().ToDefaultValue(DefaultTitle));
        }

        /// <summary>
        /// Gets the created date/time value.
        /// </summary>
        public DateTime Created => GetDateTime(Columns.Created);

        /// <summary>
        /// Gets or sets the written date/time value.
        /// </summary>
        public DateTime Written
        {
            get => GetDateTime(Columns.Written);
            set => SetValue(Columns.Written, value);
        }

        /// <summary>
        /// Gets or sets the author id.
        /// </summary>
        public long AuthorId
        {
            get => GetInt64(Columns.AuthorId);
            set => SetValue(Columns.AuthorId, value);
        }

        /// <summary>
        /// Gets or sets the ready flag.
        /// </summary>
        public bool Ready
        {
            get => GetBoolean(Columns.Ready);
            set => SetValue(Columns.Ready, value);
        }

        /// <summary>
        /// Gets or sets the quick tag flag.
        /// </summary>
        public bool QuickFlag
        {
            get => GetBoolean(Columns.QuickFlag);
            set => SetValue(Columns.QuickFlag, value);
        }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        public string Notes
        {
            get => GetString(Columns.Notes);
            set => SetValue(Columns.Notes, value);
        }

        /// <summary>
        /// Gets a formatted value for <see cref="Written"/> converted to local time.
        /// </summary>
        public string WrittenLocal => Written.ToLocalTime().ToString(DateFormat, CultureInfo.InvariantCulture);
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleRow"/> class.
        /// </summary>
        /// <param name="row">The data row</param>
        public TitleRow(DataRow row) : base(row)
        {
        }

        /// <summary>
        /// Creates a new <see cref="TitleRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new row, or null.</returns>
        public static TitleRow Create(DataRow row)
        {
            return row != null ? new TitleRow(row) : null;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Toggles the value of the <see cref="QuickFlag"/> property.
        /// </summary>
        public void ToggleQuickFlag()
        {
            QuickFlag = !QuickFlag;
        }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return $"{nameof(TitleRow)} {Id} {Title}";
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnSetValue(string columnName, object value)
        {
            if (columnName == Columns.Written)
            {
                InvokePropertyChanged(nameof(WrittenLocal));
            }
        }
        #endregion
    }
}