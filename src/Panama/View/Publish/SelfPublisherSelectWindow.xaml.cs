using MahApps.Metro.Controls;
using Restless.Panama.Database.Tables;
using Restless.Panama.ViewModel;

namespace Restless.Panama.View
{
    public partial class SelfPublisherSelectWindow : MetroWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelfPublisherSelectWindow"/> class
        /// </summary>
        public SelfPublisherSelectWindow()
        {
            InitializeComponent();
        }

        public SelfPublisherRow GetPublisher()
        {
            return ShowDialog() == true && DataContext is SelfPublisherSelectWindowViewModel viewModel
                ? viewModel.SelectedPublisher
                : null;
        }
    }
}