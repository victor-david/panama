/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Represents the base view model that is associated with a window that uses a dummy table.
    /// This class must be inherited.
    /// </summary>
    public abstract class WindowViewModel : WindowViewModel<DummyTable>
    {
    }

    /// <summary>
    /// Represents the base view model that is associated with a window.
    /// This class must be inherited.
    /// </summary>
    public abstract class WindowViewModel<T> : DataRowViewModel<T>, IWindowOwner where T : TableBase
    {
        #region Private
        private Window windowOwner;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the application information object.
        /// </summary>
        public ApplicationInfo AppInfo
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region IWindowOwner
        /// <summary>
        /// Gets or sets the window owner. Set in <see cref="WindowFactory"/>
        /// </summary>
        public Window WindowOwner
        {
            get => windowOwner;
            set
            {
                if (windowOwner != null)
                {
                    throw new InvalidOperationException($"{nameof(WindowOwner)} already set");
                }

                windowOwner = value ?? throw new ArgumentNullException(nameof(WindowOwner));
                windowOwner.Closing += WindowOwnerClosing;
                windowOwner.Closed += WindowOwnerClosed;
            }
        }

        /// <summary>
        /// Gets or sets a command to close the window. Set in <see cref="WindowFactory"/>
        /// </summary>
        public ICommand CloseWindowCommand { get; set; }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowViewModel"/> class.
        /// </summary>
        public WindowViewModel()
        {
            AppInfo = ApplicationInfo.Instance;
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// A derived class can override this method to provide
        /// custom logic when <see cref="WindowOwner"/> is closing.
        /// The base implementation does nothing.
        /// </summary>
        /// <param name="e">The event args</param>
        protected virtual void OnWindowClosing(CancelEventArgs e)
        {
        }

        /// <summary>
        /// A derived class can override this method to provide
        /// custom logic when <see cref="WindowOwner"/> has closed.
        /// The base implementation does nothing.
        /// </summary>
        protected virtual void OnWindowClosed()
        {
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void WindowOwnerClosed(object sender, EventArgs e)
        {
            OnWindowClosed();
        }

        private void WindowOwnerClosing(object sender, CancelEventArgs e)
        {
            OnWindowClosing(e);
        }
        #endregion
    }
}