using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.Tools.Database.SQLite;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Database;
using Restless.App.Panama.Collections;
using System.Diagnostics;
using System.Windows.Input;
using System.Data;
using System.Collections.ObjectModel;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Represnets the base class for a controller that handles the display of table relations. This class must be inherited.
    /// </summary>
    public abstract class TableRelationController : ControllerBase<TableViewModel, TableTable>
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Protected fields
        /// <summary>
        /// Defines the column width that derived classes use to create a column that displays the relation name.
        /// </summary>
        protected const int RelationWidth = 150;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the collection of relations for this controller
        /// </summary>
        public ObservableCollection<DataRelation> Relations
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableRelationController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TableRelationController(TableViewModel owner)
            : base(owner)
        {
            Relations = new ObservableCollection<DataRelation>();
        }
        #endregion

        /************************************************************************/
        
        #region Public methods

        #endregion

        /************************************************************************/

        #region Protected methods
        #endregion

        /************************************************************************/

        #region Private methods
        #endregion
    }
}
