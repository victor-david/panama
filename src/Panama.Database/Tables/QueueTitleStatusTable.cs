using Restless.Toolkit.Core.Database.SQLite;
using System.Collections.Generic;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains information on the status types a queue/title may have.
    /// This is a readonly lookup table.
    /// </summary>
    public class QueueTitleStatusTable : Core.ApplicationTableLookupBase
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
            public const string TableName = "queuetitlestatus";

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
                /// The name of the name column. This column holds the name of the status.
                /// </summary>
                public const string Name = "name";
            }

            /// <summary>
            /// Provides static relation names.
            /// </summary>
            public static class Relations
            {
                /// <summary>
                /// The name of the relation that relates the <see cref="QueueTitleStatusTable"/> to the <see cref="QueueTitleTable"/>.
                /// </summary>
                public const string ToQueueTitle = "StatusToQueueTitle";
            }

            /// <summary>
            /// Provides static reponse values.
            /// </summary>
            public static class Values
            {
                /// <summary>
                /// The value used when status is idle / none
                /// </summary>
                public const long StatusIdle = 0;

                /// <summary>
                /// The value used when status is pending.
                /// </summary>
                public const long StatusPending = 1;

                /// <summary>
                /// The value used when status is published
                /// </summary>
                public const long StatusPublished = 2;
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueTitleStatusTable"/> class.
        /// </summary>
        public QueueTitleStatusTable() : base(Defs.TableName)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <inheritdoc/>
        public override void Load()
        {
            Load(null, Defs.Columns.Id);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override ColumnDefinitionCollection GetColumnDefinitions()
        {
            return new ColumnDefinitionCollection()
            {
                { Defs.Columns.Id, ColumnType.Integer, true },
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
            yield return new object[] { Defs.Values.StatusIdle, "Idle" };
            yield return new object[] { Defs.Values.StatusPending, "Scheduled" };
            yield return new object[] { Defs.Values.StatusPublished, "Published" };
        }

        /// <inheritdoc/>
        protected override void SetDataRelations()
        {
            CreateParentChildRelation<QueueTitleTable>(Defs.Relations.ToQueueTitle, Defs.Columns.Id, QueueTitleTable.Defs.Columns.Status);
        }
        #endregion
    }
}