/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tools.Database.SQLite;
using Restless.Tools.Utility;
using System.Windows;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Represents the base view model that is associated with a Window that uses a DataGrid. This class must be inherited.
    /// </summary>
    /// <typeparam name="T">The table type associated with this class.</typeparam>
    public abstract class WindowDataGridViewModel<T> : DataGridViewModel<T> where T: TableBase
    {
        /// <summary>
        /// Gets the window that owns this view model
        /// </summary>
        protected new Window Owner
        {
            get;
            private set;
        }

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowDataGridViewModel{T}"/> class.
        /// </summary>
        /// <param name="owner">The window that owns this instance.</param>
        public WindowDataGridViewModel(Window owner) : base(null)
        {
            Validations.ValidateNull(owner, "Owner");
            Owner = owner;
            Owner.DataContext = this;
            Owner.SetValue(WindowViewModel.ViewModelProperty, this);
        }
        #endregion
    }
}