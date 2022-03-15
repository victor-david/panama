using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.SelfPublisherTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="SelfPublisherTable"/>.
    /// </summary>
    public class SelfPublisherRow : RowObjectBase<SelfPublisherTable>
    {
        #region Public properties
        /// <summary>
        /// Gets the default name value
        /// </summary>
        public const string DefaultName = "(none)";

        /// <summary>
        /// Gets the id.
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets or sets the publisher name
        /// </summary>
        public string Name
        {
            get => GetString(Columns.Name);
            set => SetValue(Columns.Name, value.Trim().ToDefaultValue(DefaultName));
        }

        /// <summary>
        /// Gets or sets publisher url
        /// </summary>
        public string Url
        {
            get => GetString(Columns.Url);
            set => SetValue(Columns.Url, value);
        }

        /// <summary>
        /// Gets or sets publisher notes
        /// </summary>
        public string Notes
        {
            get => GetString(Columns.Notes);
            set => SetValue(Columns.Notes, value);
        }

        /// <summary>
        /// Gets the date / time publisher added
        /// </summary>
        public DateTime Added => GetDateTime(Columns.Added);
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SelfPublisherRow"/> class.
        /// </summary>
        /// <param name="row">The data row</param>
        public SelfPublisherRow(DataRow row) : base(row)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Creates a new <see cref="SelfPublisherRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new tag row, or null.</returns>
        public static SelfPublisherRow Create(DataRow row)
        {
            return row != null ? new SelfPublisherRow(row) : null;
        }

        /// <summary>
        /// Gets a boolean value that indicates if <see cref="Url"/> is populated.
        /// </summary>
        /// <returns></returns>
        public bool HasUrl()
        {
            return !string.IsNullOrEmpty(Url);
        }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return $"{nameof(SelfPublisherRow)} {Id} {Name}";
        }
        #endregion
    }
}