using Restless.Panama.Database.Tables;
using System;

namespace Restless.Panama.Database.Core
{
    /// <summary>
    /// Represents the base class for application tables. This class must be inherited.
    /// </summary>
    public abstract class ApplicationTableBase : Toolkit.Core.Database.SQLite.ApplicationTableBase
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationTableBase"/> class using <see cref="DatabaseController.MainAppSchemaName"/>.
        /// </summary>
        /// <param name="tableName">The table name</param>
        protected ApplicationTableBase(string tableName) : base(DatabaseController.Instance, DatabaseController.MainAppSchemaName, tableName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationTableBase"/> class using the specified schema name.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name</param>
        protected ApplicationTableBase(string schemaName, string tableName) : base(DatabaseController.Instance, schemaName, tableName)
        {
        }
        #endregion

        /************************************************************************/

        #region Internal / Protected
        internal virtual long DataVersion => 1;
        internal virtual void PerformSchemaUpdate()
        {
        }
        internal virtual void PerformDataUpdate()
        {
        }
        protected SchemaTable SchemaTable => Controller.GetTable<SchemaTable>();

        /// <summary>
        /// Adds a column to the database (if it doesn't exists) and
        /// adds a column to the Columns collection (if it doesn't exist)
        /// </summary>
        /// <param name="colName">The column name</param>
        /// <param name="colDefinition">The column definition for the database</param>
        /// <param name="type">The type used when adding to the Columns collection</param>
        /// <remarks>
        /// When updating an existing table, the column doesn't exist the first run.
        /// This method will add it, and add the corresponding column to the Columns collection.
        /// When creating from scratch (fresh install with no upgrade), the column should already
        /// exist both in the database and the Columns collection because GetColumnDefinitions()
        /// will have been called on the table.
        /// </remarks>
        protected void AddColumnIf(string colName, string colDefinition, Type type)
        {
            AddColumn(colName, colDefinition);
            if (Columns[colName] == null)
            {
                Columns.Add(colName, type);
            }
        }
        #endregion
    }
}