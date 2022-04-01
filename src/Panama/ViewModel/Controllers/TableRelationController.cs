/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Tables;
using System.Collections.ObjectModel;
using System.Data;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Represnets the base class for a controller that handles the display of table relations. This class must be inherited.
    /// </summary>
    public abstract class TableRelationController : BaseController<TableViewModel, TableTable>
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