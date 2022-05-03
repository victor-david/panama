using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using System.Text;
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
        /// Gets the status code, ex: 200
        /// </summary>
        public long Status => GetInt64(Columns.Status);

        /// <summary>
        /// Gets the status text, ex: 200
        /// </summary>
        public string StatusText => GetString(Columns.StatusText);

        /// <summary>
        /// Gets the size
        /// </summary>
        public long Size => GetInt64(Columns.Size);

        /// <summary>
        /// Gets the error string if any
        /// </summary>
        public string Error => GetString(Columns.Error);
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
        /// <returns>This instance</returns>
        public LinkVerifyRow SetScanned()
        {
            SetValue(Columns.Scanned, DateTime.UtcNow);
            return this;
        }

        /// <summary>
        /// Sets <see cref="Status"/> to the specified value
        /// </summary>
        /// <param name="value">The value to set</param>
        /// <returns>This instance</returns>
        public LinkVerifyRow SetStatus(long value)
        {
            SetValue(Columns.Status, value);
            return this;
        }

        /// <summary>
        /// Sets <see cref="StatusText"/> to the specified value
        /// </summary>
        /// <param name="value">The value to set</param>
        /// <returns>This instance</returns>
        public LinkVerifyRow SetStatusText(string value)
        {
            SetValue(Columns.StatusText, value);
            return this;
        }

        /// <summary>
        /// Sets <see cref="Size"/> to the specified value
        /// </summary>
        /// <param name="value">The value to set</param>
        /// <returns>This instance</returns>
        public LinkVerifyRow SetSize(long value)
        {
            SetValue(Columns.Size, value);
            return this;
        }

        /// <summary>
        /// Sets <see cref="Error"/> to the specified value
        /// </summary>
        /// <param name="value">The value to set</param>
        /// <returns>This instance</returns>
        public LinkVerifyRow SetError(Exception value)
        {
            SetValue(Columns.Error, GetExceptionMessage(value, 0));
            return this;
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

        #region Private methods
        private string GetExceptionMessage(Exception e, int level)
        {
            if (e == null)
            {
                return null;
            }
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"Level {level} => {e.GetType().FullName}");
            builder.AppendLine(e.Message);

            if (e.InnerException != null)
            {
                builder.AppendLine();
                builder.Append(GetExceptionMessage(e.InnerException, level + 1));
            }
            return builder.ToString();
        }
        #endregion
    }
}