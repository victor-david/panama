using Restless.Panama.Database.Tables;

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
        #endregion
    }
}