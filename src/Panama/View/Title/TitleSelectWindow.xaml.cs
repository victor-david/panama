/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Tables;
using Restless.Panama.ViewModel;
using Restless.Toolkit.Controls;
using System.Collections.Generic;

namespace Restless.Panama.View
{
    public partial class TitleSelectWindow : AppWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleSelectWindow"/> class
        /// </summary>
        public TitleSelectWindow()
        {
            InitializeComponent();
        }

        public List<TitleRow> GetTitles()
        {
            return ShowDialog() == true && DataContext is TitleSelectWindowViewModel viewModel && viewModel.SelectedTitles.Count > 0
                ? viewModel.SelectedTitles
                : null;
        }
    }
}