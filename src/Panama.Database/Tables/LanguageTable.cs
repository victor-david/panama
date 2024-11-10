using Restless.Toolkit.Core.Database.SQLite;
using System.Collections.Generic;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that holds the language identifiers that are available when assigning
    /// a language id to a title version. This is a readonly lookup table.
    /// </summary>
    public class LanguageTable : Core.ApplicationTableLookupBase
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
            public const string TableName = "language";

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
                /// The name of the name column. This column holds the descriptive name of the language.
                /// </summary>
                public const string Name = "name";
            }

            /// <summary>
            /// Provides static values for the <see cref="Columns.Id"/>  column.
            /// </summary>
            public static class Values
            {
                /// <summary>
                /// The value of the <see cref="Columns.Id"/> column for the default language.
                /// </summary>
                public const string DefaultLanguageId = "en-us";
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageTable"/> class.
        /// </summary>
        public LanguageTable() : base(Defs.TableName)
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
            Load(null, Defs.Columns.Name);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override ColumnDefinitionCollection GetColumnDefinitions()
        {
            return new ColumnDefinitionCollection()
            {
                { Defs.Columns.Id, ColumnType.Text, true },
                { Defs.Columns.Name, ColumnType.Text },
            };
        }

        /// <inheritdoc/>
        protected override List<string> GetPopulateColumnList()
        {
            return new List<string>() { Defs.Columns.Id, Defs.Columns.Name };
        }

        /// <inheritdoc/>
        protected override IEnumerable<object[]> EnumeratePopulateValues()
        {
            yield return new object[] { "en-us", "English (US)" };
            yield return new object[] { "es-mx", "Spanish (Mexico)" };
        }

        /// <inheritdoc/>
        protected override void SetColumnProperties()
        {
            // override the base method to do nothing
        }
        #endregion
    }
}