using Restless.Panama.Database.Core;
using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.PublishedTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="PublishedTable"/>.
    /// </summary>
    public class PublishedRow : RowObjectBase<PublishedTable>
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
        public long PublisherId => GetInt64(Columns.PublisherId);

        /// <summary>
        /// Gets the publisher name
        /// </summary>
        public string PublisherName => GetString(Columns.Joined.Publisher);

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
        /// Initializes a new instance of the <see cref="PublishedRow"/> class.
        /// </summary>
        /// <param name="row">The data row</param>
        public PublishedRow(DataRow row) : base(row)
        {
        }

        /// <summary>
        /// Creates a new <see cref="PublishedRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new row, or null.</returns>
        public static PublishedRow Create(DataRow row)
        {
            return row != null ? new PublishedRow(row) : null;
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

        ///// <summary>
        ///// Gets a boolean value that indicates if <see cref="Url"/> is populated.
        ///// </summary>
        ///// <returns></returns>
        //public bool HasUrl()
        //{
        //    return !string.IsNullOrEmpty(Url);
        //}

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return $"{nameof(PublishedRow)} {Id} {PublisherName}";
        }
        #endregion
    }
}