using Restless.Toolkit.Core.Database.SQLite;
using System.Data;
using Columns = Restless.Panama.Database.Tables.ThemeTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="ThemeTable"/>
    /// </summary>
    public class ThemeRow : RowObjectBase<ThemeTable>
    {
        #region Properties
        /// <summary>
        /// Gets the record id.
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets the theme id.
        /// </summary>
        public string ThemeId => GetString(Columns.ThemeId);

        /// <summary>
        /// Gets the theme base.
        /// </summary>
        public string ThemeBase => GetString(Columns.ThemeBase);

        /// <summary>
        /// Gets the theme color.
        /// </summary>
        public string ThemeColor => GetString(Columns.ThemeColor);

        /// <summary>
        /// Gets the theme display name.
        /// </summary>
        public string ThemeDisplay => GetString(Columns.ThemeDisplay);

        /// <summary>
        /// Gets or sets whether theme is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get => GetBoolean(Columns.IsEnabled);
            set => SetValue(Columns.IsEnabled, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Creates a new <see cref="ThemeRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new row, or null.</returns>
        public static ThemeRow Create(DataRow row)
        {
            return row != null ? new ThemeRow(row) : null;
        }

        private ThemeRow(DataRow row) : base(row)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString() => ThemeId;
        #endregion
    }
}