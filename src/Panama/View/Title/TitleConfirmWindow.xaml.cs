using MahApps.Metro.Controls;
using Restless.Panama.ViewModel;

namespace Restless.Panama.View
{
    public partial class TitleConfirmWindow : MetroWindow
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