﻿using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.SelfPublishedTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="SelfPublishedTable"/>.
    /// </summary>
    public class SelfPublishedRow : RowObjectBase<SelfPublishedTable>
    {
        #region Public properties
        /// <summary>
        /// Gets the id.
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets the title id.
        /// </summary>
        public long TitleId => GetInt64(Columns.TitleId);

        /// <summary>
        /// Gets the publisher id.
        /// </summary>
        public long PublisherId => GetInt64(Columns.SelfPublisherId);

        /// <summary>
        /// Gets the publisher name
        /// </summary>
        public string PublisherName => GetString(Columns.Joined.SelfPublisher);

        /// <summary>
        /// Gets the date / time record added
        /// </summary>
        public DateTime Added => GetDateTime(Columns.Added);

        /// <summary>
        /// Gets the published date
        /// </summary>
        /// <remarks>
        /// To set this property, use <see cref="SetPublishedDate(DateTime?)"/>
        /// </remarks>
        public DateTime? Published
        {
            get => GetNullableDateTime(Columns.Published);
            private set => SetValue(Columns.Published, value);
        }

        /// <summary>
        /// Gets a boolean value that indicates if <see cref="Published"/> has a value
        /// </summary>
        public bool HasPublishedDate => Published != null;

        /// <summary>
        /// Gets or sets the published url
        /// </summary>
        public string Url
        {
            get => GetString(Columns.Url);
            set => SetValue(Columns.Url, value);
        }

        /// <summary>
        /// Gets a boolean value that indicates if <see cref="Url"/> is populated.
        /// </summary>
        public bool HasUrl => !string.IsNullOrEmpty(Url);

        /// <summary>
        /// Gets or sets published notes
        /// </summary>
        public string Notes
        {
            get => GetString(Columns.Notes);
            set => SetValue(Columns.Notes, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SelfPublishedRow"/> class.
        /// </summary>
        /// <param name="row">The data row</param>
        public SelfPublishedRow(DataRow row) : base(row)
        {
        }

        /// <summary>
        /// Creates a new <see cref="SelfPublishedRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new row, or null.</returns>
        public static SelfPublishedRow Create(DataRow row)
        {
            return row != null ? new SelfPublishedRow(row) : null;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Sets <see cref="Published"/>
        /// </summary>
        /// <param name="value">The value to set</param>
        public void SetPublishedDate(DateTime? value)
        {
            Published = value;
        }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return $"{nameof(SelfPublishedRow)} {Id} {PublisherName}";
        }
        #endregion
    }
}