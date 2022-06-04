/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Toolkit.Controls;
using System.Collections.ObjectModel;
using System.Data;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Represents a controller that displays relations of a table
    /// </summary>
    public class TableRelationController : TableBaseController<DataRelation>
    {
        #region Private
        private readonly ObservableCollection<DataRelation> dataRelations;
        #endregion

        public enum ControllerType
        {
            Parent,
            Child
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableRelationController"/> class
        /// </summary>
        /// <param name="owner">The owner</param>
        /// <param name="type">The type of relation, parent or child</param>
        public TableRelationController(TableViewModel owner, ControllerType type) : base(owner)
        {
            Columns.Create("Name", nameof(DataRelation.RelationName)).MakeFixedWidth(FixedWidth.W180);
            if (type == ControllerType.Parent)
            {
                Columns.Create("Parent Table", $"{nameof(DataRelation.ParentTable)}.{nameof(DataRelation.ParentTable.TableName)}");
            }
            else
            {
                Columns.Create("Child Table", $"{nameof(DataRelation.ChildTable)}.{nameof(DataRelation.ChildTable.TableName)}");
            }

            Columns.Create("Parent Column", "ParentColumns[0].ColumnName");
            Columns.Create("Child Column", "ChildColumns[0].ColumnName");

            dataRelations = new ObservableCollection<DataRelation>();
            InitListView(dataRelations);
        }

        /// <summary>
        /// Updates this controler
        /// </summary>
        /// <param name="relationCollection">The source relation collection</param>
        public void Update(DataRelationCollection relationCollection)
        {
            if (relationCollection != null)
            {
                dataRelations.Clear();
                foreach (DataRelation relation in relationCollection)
                {
                    dataRelations.Add(relation);
                }
            }
        }
    }
}