/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using MahApps.Metro.Controls;
using Restless.Panama.Core;
using System;
using System.Windows;
using System.Windows.Interop;

namespace Restless.Panama.View
{
    public partial class MainWindow : MetroWindow
    {
        #region Private
        private static MainWindow staticMain;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
            {
                staticMain = this;
                hwndSource.AddHook(HandleWindowMessage);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        /// <summary>
        /// Matches the HwndSourceHook delegate signature so it can be passed to AddHook() as a callback.
        /// </summary>
        /// <param name="hwnd">The window handle</param>
        /// <param name="msg">The message ID</param>
        /// <param name="wParam">The message's wParam value, historically used in the win32 api for handles and integers</param>
        /// <param name="lParam">The message's lParam value, historically used in the win32 api to pass pointers</param>
        /// <param name="handled">A value that indicates whether the message was handled</param>
        /// <returns></returns>
        private static IntPtr HandleWindowMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_SHOW_ACTIVE_WIN)
            {
                staticMain?.ShowMe();
                handled = true;
            }
            return IntPtr.Zero;
        }

        private void ShowMe()
        {
            if (WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
            }
            bool top = Topmost;
            Topmost = true;
            Topmost = top;
        }
        #endregion
    }
}