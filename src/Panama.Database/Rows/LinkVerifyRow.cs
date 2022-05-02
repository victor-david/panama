using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.LinkVerifyTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="LinkVerifyTable"/>
    /// </summary>
    public class LinkVerifyRow : RowObjectBase<LinkVerifyTable>
    {
        #region Properties
        /// <summary>
        /// Gets the default author value.
        /// </summary>
        public const string DefaultAuthor = "(none)";

        /// <summary>
        /// Gets the record id.
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets the external id.
        /// </summary>
        public long Xid => GetInt64(Columns.Xid);

        /// <summary>
        /// Gets the source.
        /// </summary>
        public string Source => GetString(Columns.Source);

        /// <summary>
        /// Gets the url
        /// </summary>
        public string Url => GetString(Columns.Url);

        /// <summary>
        /// Gets the date / time last scanned
        /// </summary>
        public DateTime? Scanned => GetNullableDateTime(Columns.Scanned);

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public long Status
        {
            get => GetInt64(Columns.Status);
            set => SetValue(Columns.Status, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkVerifyRow"/> class
        /// </summary>
        /// <param name="row">The data row</param>
        public LinkVerifyRow(DataRow row) : base(row)
        {
        }

        /// <summary>
        /// Creates a new <see cref="LinkVerifyRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new row, or null.</returns>
        public static LinkVerifyRow Create(DataRow row)
        {
            return row != null ? new LinkVerifyRow(row) : null;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Sets <see cref="Scanned"/> to the current date / time, utc
        /// </summary>
        public void SetScanned()
        {
            SetValue(Columns.Scanned, DateTime.UtcNow);
        }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return $"{Source} {Url}";
        }
        #endregion
    }
}