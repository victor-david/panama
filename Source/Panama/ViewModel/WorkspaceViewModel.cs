using System;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using Restless.App.Panama.Collections;
using Restless.Tools.Utility;
using Restless.Tools.Controls.DragDrop;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the Close command and properties that are common to all view models. This class must be interited.
    /// </summary>
    public abstract class WorkspaceViewModel : ViewModelBase
    {
        #region Private Vars
        private MainWindowViewModel mainViewModel;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets the owner of this view model. Once set, this property cannot be changed.
        /// </summary>
        public MainWindowViewModel MainViewModel
        {
            get { return mainViewModel; }
            set
            {
                Validations.ValidateInvalidOperation(mainViewModel != null, "This property may only be set once");
                mainViewModel = value;
            }
        }

        /// <summary>
        /// Gets the maximum number of workspaces that may be created. -1 means unlimited. The default is 1.
        /// </summary>
        public int MaxCreatable
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the command that removes this workspace from the user interface.
        /// </summary>
        public ICommand CloseCommand
        {
            get;
            protected set;
        }

        /// <summary>
        /// Returns a list of commands that the UI can display and execute.
        /// </summary>
        public List<VisualCommandViewModel> VisualCommands
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a list of filter commands that the UI can display and execute.
        /// </summary>
        public List<VisualCommandViewModel> FilterCommands
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a dictionary of commands. Unlike <see cref="VisualCommands"/>, these are necessarily related to  a visual representation
        /// These can be added ad-hoc and bound to a control with: Command="{Binding RawCommands[name]}"
        /// </summary>
        public CommandDictionary Commands
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the singleton instance of the configuration object. 
        /// Although derived classes can access the singleton instance directly,
        /// this enables easy binding to certain configuration properties
        /// </summary>
        public Restless.App.Panama.Configuration.Config Config
        {
            get { return Restless.App.Panama.Configuration.Config.Instance; }
        }
        #endregion

        /************************************************************************/
        
        #region Protected properties
        /// <summary>
        /// Provides the standard image size for a visual command.
        /// </summary>
        protected const double VisualCommandImageSize = 20.0;
        /// <summary>
        /// Provides the standard font size for a visual command.
        /// </summary>
        protected const double VisualCommandFontSize = 11;
        #endregion

        /************************************************************************/
        
        #region Constructor
        #pragma warning disable 1591
        protected WorkspaceViewModel()
        {
            VisualCommands = new List<VisualCommandViewModel>();
            FilterCommands = new List<VisualCommandViewModel>();

            Commands = new CommandDictionary();
            CloseCommand = new RelayCommand((o)=>
            {
                OnClosing(new CancelEventArgs());
            });
            MaxCreatable = 1;
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Events
        /// <summary>
        /// Raised when this workspace should be removed from the UI.
        /// </summary>
        public event CancelEventHandler Closing;

        #endregion
        
        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Override in a derived class to receive notification that a a data record was added.
        /// The base implementation does nothing.
        /// </summary>
        /// <remarks>
        /// A derived class only needs to override this method if another VM can add records
        /// that affect the VM. For instance, a new submission is created by the Publisher VM
        /// and we want to make sure that the Submission VM (if open) receives notification.
        /// This is mostly because of the fact that adding a record messes up the sorting and/or
        /// we want the VM to return to its default sort to display the latest record on the top.
        /// </remarks>
        public virtual void OnRecordAdded()
        {
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Raises the Closing event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected virtual void OnClosing(CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                CancelEventHandler handler = Closing;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

        }
        #endregion
    }
}