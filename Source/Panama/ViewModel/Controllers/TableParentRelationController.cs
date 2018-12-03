using Restless.App.Panama.Database;
using Restless.Tools.Controls;
using System.Data;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Represents a controller that displays the parent relations of a table.
    /// </summary>
    public class TableParentRelationController : TableRelationController
    {
        #region Private
        #endregion

        /************************************************************************/
        
        #region Public properties

        //public ObservableCollection<DataRelation> Relations
        //{
        //    get;
        //    private set;
        //}
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableParentRelationController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TableParentRelationController(TableViewModel owner)
            : base(owner)
        {
            Columns.Create("Name", "RelationName").MakeFixedWidth(RelationWidth);
            Columns.Create("Parent Table", "ParentTable.TableName");
            Columns.Create("Parent Column", "ParentColumns[0].ColumnName");
            Columns.Create("Col", "ChildColumns[0].ColumnName");
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
                foreach (DataRelation relation in table.ParentRelations)
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
