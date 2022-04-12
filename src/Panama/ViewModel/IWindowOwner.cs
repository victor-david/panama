/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.Windows;
using System.Windows.Input;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Defines an interface that describes window ownership.
    /// </summary>
    public interface IWindowOwner
    {
        /// <summary>
        /// Gets or sets the window owner.
        /// </summary>
        Window WindowOwner { get; set; }

        /// <summary>
        /// Gets or sets a command to close the window.
        /// </summary>
        ICommand CloseWindowCommand { get; set; }
    }
}
