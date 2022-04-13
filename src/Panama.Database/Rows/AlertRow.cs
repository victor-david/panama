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
using System.Runtime.CompilerServices;
using Columns = Restless.Panama.Database.Tables.AlertTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="AlertTable"/>.
    /// </summary>
    public class AlertRow : RowObjectBase<AlertTable>, INotifyPropertyChanged
    {
        #region Private
        private string dateFormat;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the default title value.
        /// </summary>
        public const string DefaultTitle = "(none)";

        /// <summary>
        /// Gets the id
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string Title
        {
            get => GetString(Columns.Title);
            set => SetValue(Columns.Title, value.ToDefaultValue(DefaultTitle));
        }

        /// <summary>
        /// Gets or sets the url
        /// </summary>
        public string Url
        {
            get => GetString(Columns.Url);
            set => SetValue(Columns.Url, value);
        }

        /// <summary>
        /// Gets or sets the date for alert.
        /// </summary>
        public DateTime Date
        {
            get => GetDateTime(Columns.Date);
            set => SetValue(Columns.Date, value);
        }

        /// <summary>
        /// Gets or sets the enabled status.
        /// </summary>
        public bool Enabled
        {
            get => GetBoolean(Columns.Enabled);
            set => SetValue(Columns.Enabled, value);
        }

        /// <summary>
        /// Gets a boolean value that indicates if this object contains a url.
        /// </summary>
        public bool HasUrl => !string.IsNullOrEmpty(Url);

        /// <summary>
        /// Gets a formatted value for <see cref="Date"/> converted to local time.
        /// </summary>
        public string DateLocal => Date.ToLocalTime().ToString(dateFormat, CultureInfo.InvariantCulture);
        #endregion

        /************************************************************************/

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RowObject"/> class.
        /// </summary>
        /// <param name="row">The data row</param>
        public AlertRow(DataRow row) : base(row)
        {
            dateFormat = "MMM dd, yyyy";
        }

        /// <summary>
        /// Creates a new <see cref="AlertRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new row, or null.</returns>
        public static AlertRow Create(DataRow row)
        {
            return row != null ? new AlertRow(row) : null;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Sets the date format used for <see cref="DateLocal"/>
        /// </summary>
        /// <param name="value"></param>
        public void SetDateFormat(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                dateFormat = value;
            }
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnSetValue(string columnName, object value)
        {
            if (columnName == Columns.Date)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DateLocal)));
            }
        }
        #endregion
    }
}