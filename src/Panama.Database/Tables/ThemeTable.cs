using Restless.Toolkit.Core.Database.SQLite;
using System.Data;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the theme table.
    /// </summary>
    public class ThemeTable : Core.ApplicationTableBase
    {
        #region Public properties
        /// <summary>
        /// Provides static definitions for table properties such as column names and relation names.
        /// </summary>
        public static class Defs
        {
            /// <summary>
            /// Specifies the name of this table.
            /// </summary>
            public const string TableName = "theme";

            /// <summary>
            /// Provides static column names for this table.
            /// </summary>
            public static class Columns
            {
                /// <summary>
                /// The name of the id column. This is the table's primary key.
                /// </summary>
                public const string Id = DefaultPrimaryKeyName;

                /// <summary>
                /// The theme id
                /// </summary>
                public const string ThemeId = "themeid";

                /// <summary>
                /// The base of the theme (light or dark)
                /// </summary>
                public const string ThemeBase = "base";

                /// <summary>
                /// The theme color (red, blue, etc)
                /// </summary>
                public const string ThemeColor = "color";

                /// <summary>
                /// The theme display name
                /// </summary>
                public const string ThemeDisplay = "name";
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeTable"/> class.
        /// </summary>
        public ThemeTable() : base(Defs.TableName)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Loads the data from the database into the Data collection for this table.
        /// </summary>
        public override void Load()
        {
            Load(null, $"{Defs.Columns.ThemeBase} desc, {Defs.Columns.ThemeId} asc");
        }

        public void InsertTheme(string baseScheme, string colorScheme, string themeDisplay, string themeId)
        {
            DataRow[] rows = Select($"{Defs.Columns.ThemeId}='{themeId}'");
            if (rows.Length == 0)
            {
                DataRow newRow = NewRow();
                newRow[Defs.Columns.ThemeBase] = baseScheme;
                newRow[Defs.Columns.ThemeColor] = colorScheme;
                newRow[Defs.Columns.ThemeId] = themeId;
                newRow[Defs.Columns.ThemeDisplay] = themeDisplay;
                Rows.Add(newRow);
            }
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Gets the column definitions for this table.
        /// </summary>
        /// <returns>A <see cref="ColumnDefinitionCollection"/>.</returns>
        protected override ColumnDefinitionCollection GetColumnDefinitions()
        {
            return new ColumnDefinitionCollection()
            {
                { Defs.Columns.Id, ColumnType.Integer, true },
                { Defs.Columns.ThemeId, ColumnType.Text },
                { Defs.Columns.ThemeBase, ColumnType.Text },
                { Defs.Columns.ThemeColor, ColumnType.Text },
                { Defs.Columns.ThemeDisplay, ColumnType.Text,false },
            };
        }
        #endregion
    }
}