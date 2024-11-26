using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.PublishedAllTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="PublishedAllTable"/>.
    /// </summary>
    public class PublishedAllRow : RowObjectBase<PublishedAllTable>
    {
        #region Public properties
        /// <summary>
        /// Gets the id.
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets the type id
        /// </summary>
        public long TypeId => GetInt64(Columns.TypeId);

        /// <summary>
        /// Gets the related id
        /// </summary>
        public long RelatedId => GetInt64(Columns.RelatedId);

        /// <summary>
        /// Gets the date / time record added
        /// </summary>
        public DateTime Added => GetDateTime(Columns.Added);

        /// <summary>
        /// Gets or sets the published date
        /// </summary>
        public DateTime? Published
        {
            get => GetNullableDateTime(Columns.Published);
            set => SetPublished(value);
        }

        /// <summary>
        /// Gets a boolean value that indicates if <see cref="Published"/> has a value
        /// </summary>
        public bool HasPublishedDate => Published.HasValue;

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title => GetString(Columns.Title);

        /// <summary>
        /// Gets the publisher name
        /// </summary>
        public string Publisher => GetString(Columns.Publisher);

        /// <summary>
        /// Gets or sets the published url
        /// </summary>
        public string Url
        {
            get => GetString(Columns.Url);
            set => SetUrl(value);
        }

        /// <summary>
        /// Gets a boolean value that indicates if <see cref="Url"/> is populated.
        /// </summary>
        public bool HasUrl => !string.IsNullOrEmpty(Url);

        /// <summary>
        /// Gets or sets a note
        /// </summary>
        public string Note
        {
            get => GetString(Columns.Note);
            set => SetNote(value);
        }

        public string Display => $"{Title} [{Publisher}]";
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Creates a new <see cref="PublishedAllRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new row, or null.</returns>
        public static PublishedAllRow Create(DataRow row)
        {
            return row != null ? new PublishedAllRow(row) : null;
        }

        private PublishedAllRow(DataRow row) : base(row)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString() => Display;
        #endregion

        /************************************************************************/

        #region Private methods

        private void SetPublished(DateTime? value)
        {
            SetValue(Columns.Published, value);
            // TODO - propagate to proper real table
        }

        private void SetUrl(string value)
        {
            SetValue(Columns.Url, value);
            // TODO - propagate to proper real table
        }

        private void SetNote(string value)
        {
            SetValue(Columns.Note, value);
            // TODO - propagate to proper real table
        }
        #endregion
    }
}