using MahApps.Metro.Controls;
using Restless.Panama.Database.Tables;
using Restless.Panama.ViewModel;
using System.Collections.Generic;

namespace Restless.Panama.View
{
    public partial class TitleSelectWindow : MetroWindow
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