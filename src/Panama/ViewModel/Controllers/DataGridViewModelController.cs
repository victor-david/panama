using Restless.Toolkit.Core.Database.SQLite;
using System;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Extends <see cref="DataGridViewModel{T}"/> to provide for controllers
    /// that work with related data of the main view model.
    /// </summary>
    /// <typeparam name="T1">The main view model type</typeparam>
    /// <typeparam name="T2">The table</typeparam>
    public class DataGridViewModelController<T1, T2> : DataGridViewModel<T2> where T1 : ApplicationViewModel where T2 : TableBase
    {
        /// <summary>
        /// Gets the owner
        /// </summary>
        public new T1 Owner
        {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridViewModelController{T1, T2}"/> class
        /// </summary>
        /// <param name="owner">The owner</param>
        public DataGridViewModelController(T1 owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }
    }
}