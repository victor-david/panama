/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.ViewModel;
using Restless.Toolkit.Controls;
using System.Collections.Generic;

namespace Restless.Panama.View
{
    public partial class SubmissionMessageSelectWindow : AppWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionMessageSelectWindow"/> class.
        /// </summary>
        public SubmissionMessageSelectWindow()
        {
            InitializeComponent();
        }

        public List<MimeKitMessage> GetMessages()
        {
            return ShowDialog() == true && DataContext is SubmissionMessageSelectWindowViewModel viewModel && viewModel.SelectedMessages.Count > 0
                ? viewModel.SelectedMessages
                : null;
        }
    }
}