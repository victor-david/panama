using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Restless.Tools.Utility;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Represents the base view model that is associated with a Window. This class must be inherited.
    /// </summary>
    public abstract class WindowViewModel : WorkspaceViewModel
    {
        #region Public properties
        /// <summary>
        /// Gets the window that owns this view model
        /// </summary>
        protected Window Owner
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Static dependency property declarations
        /// <summary>
        /// Declares an attached dependency property that enables an object
        /// that derives from <see cref="WorkspaceViewModel"/> to be associated with the window.
        /// </summary>
        /// <AttachedPropertyComments>
        /// <summary>
        /// This attached property provides access to the view model that is associated with the window.
        /// </summary>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.RegisterAttached
            (
                "ViewModel", typeof(WorkspaceViewModel), typeof(WindowViewModel)
            );
        #endregion

        /************************************************************************/

        #region
        /// <summary>
        /// Gets the <see cref="ViewModelProperty"/> for the specified element.
        /// </summary>
        /// <param name="obj">The dependency object to get the property for.</param>
        /// <returns>The attached property, or null if none.</returns>
        public static WorkspaceViewModel GetViewModel(DependencyObject obj)
        {
            return (WorkspaceViewModel)obj.GetValue(ViewModelProperty);
        }

        /// <summary>
        /// Sets the <see cref="ViewModelProperty"/> on the specified element.
        /// </summary>
        /// <param name="obj">The dependency object to set the property on.</param>
        /// <param name="value">The property to set.</param>
        public static void SetViewModel(DependencyObject obj, WorkspaceViewModel value)
        {
            obj.SetValue(ViewModelProperty, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowViewModel"/> class.
        /// </summary>
        /// <param name="owner">The window that owns this instance.</param>
        public WindowViewModel(Window owner)
        {
            Validations.ValidateNull(owner, "Owner");
            Owner = owner;
            Owner.DataContext = this;
            Owner.SetValue(WindowViewModel.ViewModelProperty, this);
        }
        #endregion
    }
}
