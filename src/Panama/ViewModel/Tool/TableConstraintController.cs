using Restless.Panama.Core;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Represents a controller that displays tables contraints.
    /// </summary>
    /// <typeparam name="T">The type of contraint that this controller handles.</typeparam>
    public class TableConstraintController<T> : TableBaseController<Constraint>
    {
        #region Private
        private readonly ObservableCollection<T> constraints;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableConstraintController{T}"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TableConstraintController(TableViewModel owner) : base(owner)
        {
            Columns.Create(Header.Name, nameof(Constraint.ConstraintName)).MakeFixedWidth(FixedWidth.W180);
            Columns.Create(Header.Table, $"{nameof(Constraint.Table)}.{nameof(Constraint.Table.TableName)}");
            Columns.Create(Header.Column, "Columns[0].ColumnName");
            if (typeof(T) == typeof(ForeignKeyConstraint))
            {
                Columns.Create(Header.DeleteRule, nameof(ForeignKeyConstraint.DeleteRule));
            }

            constraints = new ObservableCollection<T>();
            InitListView(constraints);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called by the <see cref=" ControllerBase{VM,T}.Owner"/> of this controller
        /// in order to update the controller values.
        /// </summary>
        protected override void OnUpdate()
        {
            if (Owner.SelectedTable != null)
            {
                constraints.Clear();
                foreach (T constraint in Owner.SelectedTable.Constraints.OfType<T>())
                {
                    constraints.Add(constraint);
                }
            }
        }

        protected override int OnDataRowCompare(Constraint item1, Constraint item2)
        {
            return string.Compare(item1.ConstraintName, item2.ConstraintName, StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }
}