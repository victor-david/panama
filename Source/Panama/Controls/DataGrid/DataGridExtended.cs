using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Extends <see cref="System.Windows.Controls.DataGrid"/> to provide custom functionality.
    /// </summary>
    public class DataGridExtended : DataGrid
    {
        #region Static dependency property declarations
        /// <summary>
        /// Defines an attached dependency property that is used to provide custom sorting
        /// on a DataGridColumn by adding a secondary sort on another column.
        /// </summary>
        public static readonly DependencyProperty CustomSortProperty = 
            DependencyProperty.RegisterAttached
            (
                "CustomSort", typeof(DataGridColumnSortSpec), typeof(DataGridExtended), new PropertyMetadata()
            );

        /// <summary>
        /// Defines an attached dependency property that enables binding the mouse double-click 
        /// on a data grid row to a command.
        /// </summary>
        /// <remarks>
        /// See:
        /// http://stackoverflow.com/questions/17419570/bind-doubleclick-command-from-datagrid-row-to-vm
        /// </remarks>
        /// <AttachedPropertyComments>
        /// <summary>
        /// This attached property provides access to the command that is bound to the mouse double-click.
        /// </summary>
        /// </AttachedPropertyComments>
        public static DependencyProperty DoubleClickCommandProperty =
           DependencyProperty.RegisterAttached
           (
                "DoubleClickCommand", typeof(ICommand), typeof(DataGridExtended), new PropertyMetadata(DoubleClickPropertyChanged)
           );

        /// <summary>
        /// Defines a dependency property that allows the consumer to run a command when a context menu associated with the DataGrid is opening.
        /// </summary>
        public static DependencyProperty ContextMenuOpeningCommandProperty =
             DependencyProperty.Register
             (
                "ContextMenuOpeningCommand", typeof(ICommand), typeof(DataGridExtended), new PropertyMetadata()
             );

        /// <summary>
        /// Defines a dependency property that allows the consumer to run a command when the DataGrid is sorting.
        /// </summary>
        public static DependencyProperty SortingCommandProperty =
             DependencyProperty.Register
             (
                "SortingCommand", typeof(ICommand), typeof(DataGridExtended), new PropertyMetadata()
             );

        /// <summary>
        /// Defines a dependency property that enables the consumer to bind to multiple selected items
        /// </summary>
        public static readonly DependencyProperty SelectedItemsListProperty =
            DependencyProperty.Register
            (
                "SelectedItemsList", typeof(IList), typeof(DataGridExtended), new PropertyMetadata(null)
            );
        #endregion

        /************************************************************************/

        #region Public properties

        /// <summary>
        /// Gets or sets a command to be executed when a context menu associated with the data grid is opening as defined by <see cref="ContextMenuOpeningCommandProperty"/>.
        /// </summary>
        public ICommand ContextMenuOpeningCommand
        {
            get { return GetValue(ContextMenuOpeningCommandProperty) as ICommand; }
            set { SetValue(ContextMenuOpeningCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets a command to be executed when the data grid is sorting as defined by <see cref="SortingCommandProperty"/>.
        /// </summary>
        public ICommand SortingCommand
        {
            get { return GetValue(SortingCommandProperty) as ICommand; }
            set { SetValue(CustomSortProperty, value); }
        }

        /// <summary>
        /// Gets or sets the selected items list as defined by <see cref="SelectedItemsListProperty"/>.
        /// </summary>
        public IList SelectedItemsList
        {
            get { return (IList)GetValue(SelectedItemsListProperty); }
            set { SetValue(SelectedItemsListProperty, value); }
        }
        #endregion

        /************************************************************************/

        #region Public methods (Set / Get DoubleClickCommand)
        /// <summary>
        /// Gets the <see cref="DoubleClickCommandProperty"/> for the specified element.
        /// </summary>
        /// <param name="obj">The dependency object to get the property for.</param>
        /// <returns>The attached property, or null if none.</returns>
        public static ICommand GetDoubleClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DoubleClickCommandProperty);
        }

        /// <summary>
        /// Sets the <see cref="DoubleClickCommandProperty"/> on the specified element.
        /// </summary>
        /// <param name="obj">The dependency object to set the property on.</param>
        /// <param name="value">The property to set.</param>
        public static void SetDoubleClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DoubleClickCommandProperty, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        #pragma warning disable 1591
        public DataGridExtended()
        {
            // nothing yet. May need something here later.
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Occurs when the selected item changes, raises the SelectionChanged event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            SelectedItemsList = SelectedItems;
        }

        /// <summary>
        /// Occurs when the <see cref="DataGridExtended"/> is sorting, raises the Sorting event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnSorting(DataGridSortingEventArgs e)
        {
            base.OnSorting(e);
            var view = CollectionViewSource.GetDefaultView(ItemsSource);
            // think this is unlikely
            if (view == null) return;

            // Get the custom sort.
            var colSort = e.Column.GetValue(CustomSortProperty) as DataGridColumnSortSpec;

            // If we have either a custom sort or a sorting command, clear the sort descriptions.
            if (colSort != null || SortingCommand != null)
            {
                view.SortDescriptions.Clear();
            }

            if (SortingCommand != null)
            {
                SortingCommand.Execute(e.Column);
                e.Handled = view.SortDescriptions.Count > 0;
            }

            
            if (colSort != null)
            {
                ListSortDirection primaryDirection = (e.Column.SortDirection == ListSortDirection.Ascending) ? ListSortDirection.Ascending : ListSortDirection.Descending;

                string primaryPath = e.Column.SortMemberPath;
                if (!String.IsNullOrEmpty(colSort.Column1))
                {
                    primaryPath = colSort.Column1;
                }
                view.SortDescriptions.Add(new SortDescription(primaryPath, primaryDirection));

                ListSortDirection secondaryDirection = primaryDirection;
                switch (colSort.Behavior)
                {
                    case DataGridColumnSortBehavior.AlwaysAscending:
                        secondaryDirection = ListSortDirection.Ascending;
                        break;
                    case DataGridColumnSortBehavior.AlwaysDescending:
                        secondaryDirection = ListSortDirection.Descending;
                        break;
                    case DataGridColumnSortBehavior.ReverseFollowPrimary:
                        secondaryDirection = (primaryDirection == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending;
                        break;
                    default:
                        secondaryDirection = primaryDirection;
                        break;
                }
                view.SortDescriptions.Add(new SortDescription(colSort.Column2, secondaryDirection));
                e.Handled = true;
            }
        }

        /// <summary>
        /// Called when a context menu associated with this data grid is opening.
        /// </summary>
        /// <param name="e">The event arguments</param>
        /// <remarks>
        /// If the <see cref="ContextMenuOpeningCommand"/> has been set, this method calls the command that was established,
        /// passing event arguments <paramref name="e"/> to the command handler.
        /// </remarks>
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            base.OnContextMenuOpening(e);
            ICommand cmd = ContextMenuOpeningCommand;
            if (cmd != null && cmd.CanExecute(e))
            {
                cmd.Execute(e);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private static void DoubleClickPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = d as DataGridRow;
            if (element == null) return;

            if (e.NewValue != null)
            {
                element.AddHandler(DataGridRow.MouseDoubleClickEvent, new RoutedEventHandler(DataGridMouseDoubleClick));
            }
            else
            {
                element.RemoveHandler(DataGridRow.MouseDoubleClickEvent, new RoutedEventHandler(DataGridMouseDoubleClick));
            }
        }

        private static void DataGridMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            var element = sender as DataGridRow;
            if (element != null)
            {
                var cmd = GetDoubleClickCommand(element);
                if (cmd.CanExecute(element.Item))
                {
                    cmd.Execute(element.Item);
                }
            }
        }
        #endregion
    }
}
