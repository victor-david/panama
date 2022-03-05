/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Controls;
using Restless.Toolkit.Mvvm;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the Close command and properties that are common to all view models. This class must be interited.
    /// </summary>
    public abstract class ApplicationViewModel : ViewModelBase, INavigator
    {
        #region Private Vars
        #endregion

        /************************************************************************/

        #region Public properties
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
        /// Gets the singleton instance of the configuration object.
        /// Although derived classes can access the singleton instance directly,
        /// this enables easy binding to certain configuration properties
        /// </summary>
        public Core.Config Config
        {
            get => Core.Config.Instance;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationViewModel"/> class.
        /// </summary>
        protected ApplicationViewModel()
        {
            CloseCommand = RelayCommand.Create((p) =>
            {
                OnClosing(new CancelEventArgs());
            });
            MaxCreatable = 1;
        }
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
        /// If tasks are running, sets e.Cancel to true and displays "tasks are running" message.
        /// If tasks are not running, sets e.Cancel to false.
        /// </summary>
        /// <param name="e">The cancel event args.</param>
        protected void SetCancelIfTasksInProgress(CancelEventArgs e)
        {
            // TODO
            //e.Cancel = TaskManager.Instance.WaitForAllRegisteredTasks(() =>
            //{
            //    MainWindowViewModel.Instance.CreateNotificationMessage(Strings.NotificationCannotExitTasksAreRunning);
            //}, null);
        }

        /// <summary>
        /// Raises the Closing event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected virtual void OnClosing(CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                Closing?.Invoke(this, e);
            }
        }
        #endregion
    }
}