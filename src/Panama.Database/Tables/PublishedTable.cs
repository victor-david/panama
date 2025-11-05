using Restless.Panama.Database.Core;
using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains information about titles that have been published.
    /// </summary>
    public class PublishedTable : Core.ApplicationTableBase
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
            public const string TableName = "published";

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
                /// The name of the title id column.
                /// </summary>
                public const string TitleId = "titleid";

                /// <summary>
                /// The name of the publisher id column.
                /// </summary>
                public const string PublisherId = "publisherid";

                /// <summary>
                /// The name of the added column. This column holds the date that the published record was added.
                /// </summary>
                public const string Added = "added";

                /// <summary>
                /// The name of the published column. This column holds the date that the corresponding title was published.
                /// </summary>
                public const string Published = "published";

                /// <summary>
                /// The name of the url column. This column holds the url to the published title.
                /// </summary>
                public const string Url = "url";

                /// <summary>
                /// The name of the notes column. This column holds any notes associated with the published record.
                /// </summary>
                public const string Notes = "notes";

                /// <summary>
                /// Holds whether the record is active or not. Used to mark defunct publications.
                /// </summary>
                public const string Active = "active";

                /// <summary>
                /// Provides static column names for columns that get their value fron another table.
                /// </summary>
                public static class Joined
                {
                    /// <summary>
                    /// The name of the publisher column. This column gets its value from the <see cref="PublisherTable"/>.
                    /// </summary>
                    public const string Publisher = "JoinPubName";
                }

                /// <summary>
                /// Provides static column names for calculated columns
                /// </summary>
                public static class Calculated
                {
                    /// <summary>
                    /// The name of the column that holds the current active count.
                    /// </summary>
                    public const string CurrentActiveCount = "CalcCurrActiveCount";
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublishedTable"/> class.
        /// </summary>
        public PublishedTable() : base(Defs.TableName)
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
            Load(null, Defs.Columns.TitleId);
        }

        /// <summary>
        /// Provides an enumerable that gets all entries in order of id ASC.
        /// </summary>
        /// <returns>An enumerable that gets all entries</returns>
        public IEnumerable<PublishedRow> EnumerateAll()
        {
            foreach (DataRow row in EnumerateRows(null, Defs.Columns.Id))
            {
                yield return PublishedRow.Create(row);
            }
        }

        /// <summary>
        /// Gets the row with the specified id or null if doesn't exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PublishedRow GetRow(long id) => EnumerateAll().FirstOrDefault(p => p.Id == id);

        /// <summary>
        /// Adds a published record
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="publisherId">The publisher id</param>
        public void Add(long titleId, long publisherId)
        {
            DataRow row = NewRow();
            row[Defs.Columns.TitleId] = titleId;
            row[Defs.Columns.PublisherId] = publisherId;
            row[Defs.Columns.Added] = DateTime.Now.ToZero();
            row[Defs.Columns.Active] = true;
            Rows.Add(row);
            Save();
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
                { Defs.Columns.TitleId, ColumnType.Integer, false, false, 0, IndexType.Index },
                { Defs.Columns.PublisherId, ColumnType.Integer, false, false, 0, IndexType.Index },
                { Defs.Columns.Added, ColumnType.Timestamp },
                { Defs.Columns.Published, ColumnType.Timestamp, false, true },
                { Defs.Columns.Url, ColumnType.Text, false, true },
                { Defs.Columns.Notes, ColumnType.Text, false, true },
                { Defs.Columns.Active, ColumnType.Boolean }
            };
        }

        /// <summary>
        /// Creates the <see cref="Defs.Columns.Joined"/> columns for this table.
        /// </summary>
        protected override void UseDataRelations()
        {
            CreateChildToParentColumn(Defs.Columns.Joined.Publisher, PublisherTable.Defs.Relations.ToPublished, PublisherTable.Defs.Columns.Name);
            CreateExpressionColumn<long>(Defs.Columns.Calculated.CurrentActiveCount, $"IIF({Defs.Columns.Active}=1,1,0)");
        }
        #endregion

        /************************************************************************/

        #region Update (Internal)
        internal override void PerformDataUpdate()
        {
            PerformUpdateFor(2, UpdateDates, "Set dates to zeroed time");
            PerformUpdateFor(3, AddActiveColumn, "Add active column");
        }

        private void UpdateDates()
        {
            foreach (PublishedRow item in EnumerateAll().Where(p => p.HasPublishedDate))
            {
                item.SetPublishedDate(item.Published.Value.ToZero());
                item.Row[Defs.Columns.Added] = item.Added.ToZero();
            }
        }

        private void AddActiveColumn()
        {
            AddColumnIf(Defs.Columns.Active, "boolean not null default 1", typeof(bool));
        }
        #endregion
    }
}