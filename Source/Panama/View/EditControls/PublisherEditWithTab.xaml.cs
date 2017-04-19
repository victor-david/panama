using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Restless.App.Panama.ViewModel;

namespace Restless.App.Panama.View
{
    public partial class PublisherEditWithTab : UserControl
    {
        #pragma warning disable 1591
        public PublisherEditWithTab()
        {
            InitializeComponent();
        }
        #pragma warning restore 1591

        //private void DataGridSorting(object sender, DataGridSortingEventArgs e)
        //{
        //    var vm = DataContext as PublisherViewModel;
        //    var col = e.Column as DataGridBoundColumn;
        //    if (vm != null && col != null)
        //    {
        //        vm.Titles.SortItemSource(col);
        //        e.Handled = true;
        //    }
        //}
    }
}
