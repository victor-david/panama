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
    public partial class SubmissionDocumentSelectWindow : AppWindow
    {
        public SubmissionDocumentSelectWindow()
        {
            InitializeComponent();
        }

        public SubmissionDocumentCreationType GetDocumentCreationType()
        {
            return ShowDialog() == true && DataContext is SubmissionDocumentSelectWindowViewModel viewModel
                ? viewModel.CreateType
                : SubmissionDocumentCreationType.None;
        }
    }
}