using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.AlertTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="AlertTable"/>.
    /// </summary>
    public class AlertRow : DateRowObject<AlertTable>
    {
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
            set => SetDateValue(Columns.Date, value, nameof(DateFormatted));
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
        /// Gets a formatted value for <see cref="Date"/>.
        /// </summary>
        public string DateFormatted => GetFormattedDate(Date);
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AlertRow"/> class.
        /// </summary>
        /// <param name="row">The data row</param>
        public AlertRow(DataRow row) : base(row)
        {
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
        /// Postpones the alert by the specified number of days
        /// </summary>
        /// <param name="days">Number of days to postpone</param>
        public void Postpone(int days)
        {
            Date = Utility.GetNowZero().AddDays(days);
        }

        /// <summary>
        /// Dismisses the alert by setting <see cref="Enabled"/> to false
        /// </summary>
        public void Dismiss()
        {
            Enabled = false;
        }
        #endregion
    }
}