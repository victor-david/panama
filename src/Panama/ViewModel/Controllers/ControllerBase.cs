/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.ComponentModel;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Represents the base class for view-model supplemental controllers.
    /// </summary>
    /// <typeparam name="TVM">The view model that derives from <see cref="DataGridViewModel{T}"/></typeparam>
    /// <typeparam name="T">The database table that derives from <see cref="TableBase"/></typeparam>
    /// <remarks>
    /// <para>
    /// An object derived from <see cref="ControllerBase{VM,T}"/> is created by various view models that derive from 
    /// <see cref="DataGridViewModel{T}"/>/ to offload processing into a different object, especially when the processing involves
    /// another table that's different from the table that the view-model is associated with.
    /// </para>
    /// <para>
    /// For instance, <see cref="TitleViewModel"/> displays records from the <see cref="TitleTable"/>. When a row is selected by the user,
    /// a controller attached to <see cref="TitleViewModel"/> displays related records from the <see cref="TitleVersionTable"/>.
    /// </para>
    /// </remarks>
    public abstract class ControllerBase<TVM, T> : DataGridViewModel<T> where TVM : DataGridViewModel<T> where T : TableBase
    {
        #region Public properties
        /// <summary>
        /// Gets a header value that may be bound to a UI element.
        /// </summary>
        public virtual string Header => $"{HeaderPreface} ({SourceCount})";
        #endregion

        /************************************************************************/

        #region Protected properties
        /// <summary>
        /// Gets or sets a string to use as the header preface.
        /// </summary>
        protected string HeaderPreface
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the ViewModel that owns this controller.
        /// </summary>
        protected new TVM Owner
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerBase{TVM,T}"/> class.
        /// </summary>
        /// <param name="owner">The VM that owns this view model.</param>
        protected ControllerBase(TVM owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the source list changes.
        /// </summary>
        /// <param name="e">The list changed event args</param>
        protected override void OnDataViewListChanged(ListChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Header));
        }

        /// <summary>
        /// Gets the primary id from the selected row of this controller's owner.
        /// </summary>
        /// <returns>The Int64 primary id from the selected row of this controller's owner, or Int64.MinValue if none.</returns>
        protected long GetOwnerSelectedPrimaryId()
        {
            return Owner.SelectedRow != null && Owner.SelectedPrimaryKey is long @int ? @int : long.MinValue;
        }

        /// <summary>
        /// Gets the primary id from the selected row of this controller's owner.
        /// </summary>
        /// <returns>The string primary id from the selected row of this controller's owner, or null if none.</returns>
        protected string GetOwnerSelectedPrimaryIdString()
        {
            return Owner.SelectedRow != null && Owner.SelectedPrimaryKey is string @string ? @string : null;
        }

        #endregion
    }
}