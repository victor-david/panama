using Restless.App.Panama.Database.Tables;
using Restless.Tools.Database.SQLite;
using Restless.Tools.Utility;
using System.ComponentModel;

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
                return string.Format("{0} ({1})", HeaderPreface, SourceCount);
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
        protected new VM Owner
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
        /// <param name="owner">The VM that owns this view model.</param>
        protected ControllerBase(VM owner) : base(owner)
        {
            Validations.ValidateNull(owner, "ControllerBase.Owner");
            Owner = owner;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        ///// <summary>
        ///// Causes the controller to update.
        ///// </summary>
        //public void Update()
        //{
        //    OnUpdate();
        //}
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
            if (Owner.SelectedRow != null)
            {
                object pk = Owner.SelectedPrimaryKey;
                if (pk is long)
                {
                    return (long)pk;
                }
            }
            return long.MinValue;
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
