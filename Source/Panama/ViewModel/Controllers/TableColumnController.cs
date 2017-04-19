using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.Tools.Database.SQLite;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Represents a controller that displays the columns of a table.
    /// </summary>
    public class TableColumnController : ControllerBase<TableViewModel, TableTable>
    {
        #region Private
        #endregion

        /************************************************************************/
        
        #region Public properties

        /// <summary>
        /// Gets the list of data columns
        /// </summary>
        public ObservableCollection<DataColumn> DataColumns
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableColumnController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TableColumnController(TableViewModel owner)
            : base(owner)
        {
            DataColumns = new ObservableCollection<DataColumn>();
            Columns.Create("Name", "ColumnName");
            Columns.Create("Type", "DataType");
            Columns.Create("Expression", "Expression").MakeFlexWidth(2.5);
        }
        #endregion

        /************************************************************************/
        
        #region Public methods

        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called by the <see cref=" ControllerBase{VM,T}.Owner"/> of this controller
        /// in order to update the controller values.
        /// </summary>
        protected override void OnUpdate()
        {
            string tableName = GetOwnerSelectedPrimaryIdString();
            if (tableName != null)
            {
                var table = DatabaseController.Instance.DataSet.Tables[tableName];
                DataColumns.Clear();
                foreach (DataColumn col in table.Columns)
                {
                    DataColumns.Add(col);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        #endregion
    }
}
