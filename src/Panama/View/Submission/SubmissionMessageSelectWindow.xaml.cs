using MahApps.Metro.Controls;
using Restless.Panama.Core;
using Restless.Panama.ViewModel;
using System.Collections.Generic;

namespace Restless.Panama.View
{
    public partial class SubmissionMessageSelectWindow : MetroWindow
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