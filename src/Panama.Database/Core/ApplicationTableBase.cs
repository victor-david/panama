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
        /// <summary>
        /// Called for each table to perform updates. Override if needed.
        /// </summary>
        internal virtual void PerformDataUpdate()
        {
        }

        protected SchemaTable SchemaTable => Controller.GetTable<SchemaTable>();

        /// <summary>
        /// Called by a derived class to perform an update
        /// </summary>
        /// <param name="dataVersion">The data version for the update</param>
        /// <param name="callback">The callback that performs the actual processing of the update</param>
        /// <param name="schemaNote">The note to entered into the schema table</param>
        /// <remarks>
        /// If a schema record with the current schema version and the specified data version
        /// does not exist in the schema table, the callback is called, the table saved, and
        /// a record added to the schema table to indicate that the update has been performed.
        /// All updates should be repeatable without causing a problem. For instance, when
        /// adding a column to a table, use the <see cref="AddColumnIf(string, string, Type)"/>
        /// method which adds a column only if it doesn't already exist. Other operations
        /// should be similar; if adding a value to a lookup table (for instance), make sure
        /// that the value doesn't already exist. The initial data version for all tables is 1.
        /// Values passed to this method should be 2,3,4, etc.
        /// </remarks>
        protected void PerformUpdateFor(long dataVersion, Action callback, string schemaNote)
        {
            if (!SchemaTable.HaveSchemaRecord(TableName, SchemaVersion, dataVersion))
            {
                callback();

                /* Table may be readonly */
                bool readOnly = IsReadOnly;
                IsReadOnly = false;
                Save();
                IsReadOnly = readOnly;

                SchemaTable.AddSchemaRecord(TableName, SchemaVersion, dataVersion, schemaNote);
                SchemaTable.Save();
            }

        }

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