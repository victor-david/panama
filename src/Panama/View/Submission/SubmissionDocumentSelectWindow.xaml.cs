using MahApps.Metro.Controls;
using Restless.Panama.ViewModel;

namespace Restless.Panama.View
{
    public partial class SubmissionDocumentSelectWindow : MetroWindow
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