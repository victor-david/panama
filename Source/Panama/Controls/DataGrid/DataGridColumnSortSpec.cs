using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.Tools.Utility;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Represents a custom sort specification.
    /// </summary>
    /// <remarks>
    /// A custom sort enables you to specify that when one column of a data grid is sorted, that another column
    /// is used as either the primary sort or the secondary sort. For instance, if you have a data grid that display customer's first name
    /// and last name, you can specify that when sorting on last name, that the secondary sort is on first name.
    /// That way, customers with the same last name will be arranged with the first names sorted also. Without
    /// this (using normal data grid sorting), the first name sorting is undefined.
    /// </remarks>
    public class DataGridColumnSortSpec
    {
        #region Public properties
        /// <summary>
        /// Gets the name of the primary column for the sort.
        /// If this property is null, the column that is in process of sorting is used as the primary column for the sort.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This column specifies the name of the column that will be used as the primary sort column.
        /// If null, then the column that is in the process of sorting is the primary sort column.
        /// </para>
        /// <para>
        /// This property is generally null because in most cases, the column in process of sorting
        /// ought to be the primary column. However, in certain specialized cases, it may be desirable that another
        /// column be used as the primary column during the sort.
        /// </para>
        /// </remarks>
        public string Column1
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the secondary column for the sort.
        /// </summary>
        public string Column2
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the sorting behavior for <see cref="Column2"/> in relation to <see cref="Column1"/>.
        /// </summary>
        public DataGridColumnSortBehavior Behavior
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/
        
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridColumnSortSpec"/> class.
        /// </summary>
        /// <param name="column1">The name of the column to use as the primary sort, or null to allow the column in the process of sorting to be the primary.</param>
        /// <param name="column2">The name of the column to use as the secondary sort.</param>
        /// <param name="behavior">The column's behavior</param>
        public DataGridColumnSortSpec(string column1, string column2, DataGridColumnSortBehavior behavior)
        {
            Validations.ValidateNullEmpty(column2, "DataGridColumnSortSpec.ColumnName");
            Column1 = column1;
            Column2 = column2;
            Behavior = behavior;
        }
        #endregion
    }
}
