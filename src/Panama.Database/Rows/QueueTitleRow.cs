using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using System.Globalization;
using Columns = Restless.Panama.Database.Tables.QueueTitleTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="QueueTitleTable"/>
    /// </summary>
    public class QueueTitleRow : DateRowObject<QueueTitleTable>
    {
        #region Properties
        /// <summary>
        /// Gets the default name value.
        /// </summary>
        public const string DefaultValue = "(none)";

        /// <summary>
        /// Gets the queue id.
        /// </summary>
        public long QueueId => GetInt64(Columns.QueueId);

        /// <summary>
        /// Gets the title id.
        /// </summary>
        public long TitleId => GetInt64(Columns.TitleId);

        /// <summary>
        /// Gets the title
        /// </summary>
        public string Title => GetString(Columns.Joined.Title);

        /// <summary>
        /// Gets the written date
        /// </summary>
        public DateTime Written => GetDateTime(Columns.Joined.Written);

        /// <summary>
        /// Gets the title updated date
        /// </summary>
        public DateTime Updated => GetDateTime(Columns.Joined.Updated);

        /// <summary>
        /// Gets the title word count
        /// </summary>
        public long WordCount => GetInt64(Columns.Joined.WordCount);

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public long Status
        {
            get => GetInt64(Columns.Status);
            set => SetValue(Columns.Status, value);
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime ?Date
        {
            get => GetNullableDateTime(Columns.Date);
            set => SetValue(Columns.Date, value);
        }

        /// <summary>
        /// Gets a formatted value for <see cref="Date"/> converted to local time.
        /// </summary>
        public string DateLocal => Date?.ToLocalTime().ToString(DateFormat, CultureInfo.InvariantCulture);
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueTitleRow"/> class
        /// </summary>
        /// <param name="row">The data row</param>
        public QueueTitleRow(DataRow row) : base(row)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Creates a new <see cref="QueueTitleRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new row, or null.</returns>
        public static QueueTitleRow Create(DataRow row)
        {
            return row != null ? new QueueTitleRow(row) : null;
        }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return $"{QueueId} => {TitleId} {Status}";
        }

        public void ClearDate()
        {
            Date = null;
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnSetValue(string columnName, object value)
        {
            if (columnName == Columns.Date)
            {
                InvokePropertyChanged(nameof(DateLocal));
            }
        }
        #endregion
    }
}