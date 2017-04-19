using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.Tools.Database.SQLite;
using System.Data;
using Restless.Tools.Utility;

namespace Restless.App.Panama.Database
{
    /// <summary>
    /// Decsribes the interface that a consumer must implement to use the DatabaseImporter class
    /// </summary>
    public interface IColumnRowImporter
    {
        /// <summary>
        /// Gets the column name from the original column name
        /// </summary>
        /// <param name="origColName">The original column name</param>
        /// <returns>The new column name</returns>
        string GetColumnName(string origColName);

        /// <summary>
        /// Gets a boolean value that indicates if this column should be included in the import
        /// </summary>
        /// <param name="origColName">The name of the column that's being imported.</param>
        /// <returns>true if this column should be imported; otherwise, false.</returns>
        bool IncludeColumn(string origColName);

        /// <summary>
        /// Returns a boolean value that indicates if the row should be imported
        /// </summary>
        /// <param name="row">The data row from the source</param>
        /// <returns>true if the row should be inserted into the destination; otherwise, false.</returns>
        bool GetRowConfirmation(DataRow row);

    }
}
