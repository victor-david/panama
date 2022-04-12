/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.ViewModel;
using Restless.Toolkit.Controls;

namespace Restless.Panama.View
{
    public partial class TitleConfirmWindow : AppWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleConfirmWindow"/> class
        /// </summary>
        public TitleConfirmWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets a boolean value that indicates if titles are confirmed
        /// </summary>
        /// <returns>true if user confirms titles (or no titles need to be confirmed); otherwise, false</returns>
        public bool ConfirmTitles()
        {
            return DataContext is TitleConfirmWindowViewModel viewModel && (viewModel.GetConfirmationCount() == 0 || ShowDialog() == true);
        }
    }
}