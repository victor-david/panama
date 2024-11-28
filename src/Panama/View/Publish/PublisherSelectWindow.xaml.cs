using MahApps.Metro.Controls;
using Restless.Panama.Database.Tables;
using Restless.Panama.ViewModel;

namespace Restless.Panama.View
{
    public partial class PublisherSelectWindow : MetroWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherSelectWindow"/> class
        /// </summary>
        public PublisherSelectWindow()
        {
            InitializeComponent();
        }

        public PublisherRow GetPublisher()
        {
            return ShowDialog() == true && DataContext is PublisherSelectWindowViewModel viewModel
                ? viewModel.SelectedPublisher
                : null;
        }
    }
}