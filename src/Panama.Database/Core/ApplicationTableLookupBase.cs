using System;
using System.Collections.Generic;
using System.Data;

namespace Restless.Panama.Database.Core
{
    /// <summary>
    /// Represents a table that is used only for lookup. Its row values are pre populated and not subject to change.
    /// The constructors for this class set IsReadyOnly = true
    /// </summary>
    /// <remarks>
    /// Tables that provide only lookup values should derive from this class. The main app can
    /// use the <see cref="ApplySessionChanges(Action{DataRow})"/> to modify values without
    /// persisting them to the database. The IsReadOnly flag does not prevent prepopulation.
    /// </remarks>
    public abstract class ApplicationTableLookupBase : ApplicationTableBase
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationTableLookupBase"/>.
        /// </summary>
        /// <param name="tableName">The table name</param>
        protected ApplicationTableLookupBase(string tableName) : base(tableName)
        {
            IsReadOnly = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationTableLookupBase"/> class using the specified schema name.
        /// </summary>
        /// <param name="schemaName">The schema name.</param>
        /// <param name="tableName">The table name</param>
        protected ApplicationTableLookupBase(string schemaName, string tableName) : base(schemaName, tableName)
        {
            IsReadOnly = true;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Applies changes that are not persisted to the database
        /// </summary>
        /// <param name="callback"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <remarks>
        /// This method loops through all data rows, passing each one to the callback
        /// for examination and modification. Calls AcceptChanges() at the end
        /// </remarks>
        public void ApplySessionChanges(Action<DataRow> callback)
        {
            _ = callback ?? throw new ArgumentNullException(nameof(callback));

            foreach (DataRow row in EnumerateRows())
            {
                callback(row);
            }
            AcceptChanges();
        }
        #endregion

        /************************************************************************/

        #region Protected methods

        /// <inheritdoc/>
        protected override List<string> GetPopulateColumnList()
        {
            throw new NotImplementedException("Derived classes must override this method");
        }

        /// <inheritdoc/>
        protected override IEnumerable<object[]> EnumeratePopulateValues()
        {
            throw new NotImplementedException("Derived classes must override this method");
        }
        #endregion
    }
}