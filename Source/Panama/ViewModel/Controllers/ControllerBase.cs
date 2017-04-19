using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.Tools.Database.SQLite;
using Restless.Tools.Utility;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Represents the base class for view-model supplemental controllers.
    /// </summary>
    /// <typeparam name="VM">The view model that derives from <see cref="DataGridViewModel{T}"/></typeparam>
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
    public abstract class ControllerBase<VM,T> : DataGridViewModel<T>
        where VM: DataGridViewModel<T>
        where T: TableBase
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a header value that may be bound to a UI element.
        /// </summary>
        public virtual string Header
        {
            get 
            {
                return String.Format("{0} ({1})", HeaderPreface, SourceCount);
            }

        }
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
        protected VM Owner
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerBase{VM,T}"/> class.
        /// </summary>
        /// <param name="owner"></param>
        protected ControllerBase(VM owner)
        {
            Validations.ValidateNull(owner, "ControllerBase.Owner");
            Owner = owner;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Causes the controller to update.
        /// </summary>
        public void Update()
        {
            OnUpdate();
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
            OnPropertyChanged("Header");
        }

        /// <summary>
        /// Called when this controller needs to update, usually in response to a row change on the data grid
        /// </summary>
        protected abstract void OnUpdate();

        /// <summary>
        /// Gets the primary id from the selected row of this controller's owner.
        /// </summary>
        /// <returns>The Int64 primary id from the selected row of this controller's owner, or Int64.MinValue if none.</returns>
        protected Int64 GetOwnerSelectedPrimaryId()
        {
            if (Owner.SelectedRow != null)
            {
                object pk = Owner.SelectedPrimaryKey;
                if (pk is Int64)
                {
                    return (Int64)pk;
                }
            }
            return Int64.MinValue;
        }

        /// <summary>
        /// Gets the primary id from the selected row of this controller's owner.
        /// </summary>
        /// <returns>The string primary id from the selected row of this controller's owner, or null if none.</returns>
        protected string GetOwnerSelectedPrimaryIdString()
        {
            if (Owner.SelectedRow != null)
            {
                object pk = Owner.SelectedPrimaryKey;
                if (pk is string)
                {
                    return (string)pk;
                }
            }
            return null;
        }
        
        #endregion

        /************************************************************************/

        #region Private methods
        #endregion
    }
}
