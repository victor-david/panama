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
    /// Represents a controller that 
    /// </summary>
    public class TableChildRelationController : TableRelationController
    {
        #region Private
        #endregion

        /************************************************************************/
        
        #region Public properties
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableChildRelationController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TableChildRelationController(TableViewModel owner)
            : base(owner)
        {
            Columns.Create("Name", "RelationName").MakeFixedWidth(RelationWidth);
            Columns.Create("Col", "ParentColumns[0].ColumnName").MakeFixedWidth(FixedWidth.Standard);
            Columns.Create("Child Table", "ChildTable.TableName");
            Columns.Create("Child Column", "ChildColumns[0].ColumnName").MakeFlexWidth(1.5);
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
                Relations.Clear();
                foreach (DataRelation relation in table.ChildRelations)
                {
                    Relations.Add(relation);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        #endregion
    }
}
