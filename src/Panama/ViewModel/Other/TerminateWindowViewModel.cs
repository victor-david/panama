/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Resources;
using Restless.Panama.Utility;
using Restless.Toolkit.Mvvm;
using System;
using System.Text;
using System.Windows.Input;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the view model logic for the <see cref="View.TerminateWindow"/>.
    /// </summary>
    public class TerminateWindowViewModel : ViewModelBase
    {
        #region Private
        private readonly Exception exception;
        private string message;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the exception message
        /// </summary>
        public string Message
        {
            get => message;
            private set => SetProperty(ref message, value);
        }

        /// <summary>
        /// Gets a boolean value that determines if <see cref="SendCommand"/> is enabled.
        /// </summary>
        public bool IsSendCommandEnabled => false;

        /// <summary>
        /// Gets the command used to send the exception report
        /// </summary>
        public ICommand SendCommand
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TerminateWindowViewModel"/> class.
        /// </summary>
        public TerminateWindowViewModel(Exception e)
        {
            exception = e;
            Message = GetFullMessage(exception);
            SendCommand = RelayCommand.Create(p => RunSendCommand());
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private string GetFullMessage(Exception exception)
        {
            StringBuilder builder = new();
            builder.AppendLine(Header.FatalError);
            builder.AppendLine();
            builder.AppendLine(Logger.Instance.GetExceptionMessage(exception));
            builder.AppendLine($"{Text.Detail}: {Logger.Instance.LogFile}");
            builder.AppendLine();
            builder.AppendLine(Text.ApplicationTerminate);
            return builder.ToString();
        }

        private void RunSendCommand()
        {
        }
        #endregion
    }
}