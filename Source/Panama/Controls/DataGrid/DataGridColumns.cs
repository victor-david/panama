using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Restless.App.Panama.Controls;
using Restless.App.Panama.ViewModel;
using System.Diagnostics;
using System.Reflection;
using Restless.Tools.Utility;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Provides an attached dependency property that enables the consumer to manipulate the data columns from a view model.
    /// </summary>
    /// <remarks>
    /// See:
    /// http://stackoverflow.com/questions/3065758/wpf-mvvm-datagrid-dynamic-columns
    /// </remarks>
    public static class DataGridColumns
    {
        #region Dependency properties
        /// <summary>
        /// Defines an attached dependency property that enables the consumer to manipulate the data columns from a view model.
        /// </summary>
        /// <remarks>
        /// See:
        /// http://stackoverflow.com/questions/3065758/wpf-mvvm-datagrid-dynamic-columns
        /// </remarks>
        /// <AttachedPropertyComments>
        /// <summary>
        /// This attached property provides access to the data columns.
        /// </summary>
        /// </AttachedPropertyComments>
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.RegisterAttached
            (
                "Columns", typeof(ObservableCollection<DataGridColumn>), typeof(DataGridColumns),
                new UIPropertyMetadata(new ObservableCollection<DataGridColumn>(), OnDataGridColumnsPropertyChanged)
            );
        #endregion

        /************************************************************************/

        #region Public methods (Get / Set ColumnsProperty)
        /// <summary>
        /// Gets the <see cref="ColumnsProperty"/> for the specified element.
        /// </summary>
        /// <param name="obj">The dependency object to get the property for.</param>
        /// <returns>The attached property, or null if none.</returns>
        public static ObservableCollection<DataGridColumn> GetColumns(DependencyObject obj)
        {
            return (ObservableCollection<DataGridColumn>)obj.GetValue(ColumnsProperty);
        }

        /// <summary>
        /// Sets the <see cref="ColumnsProperty"/> on the specified element.
        /// </summary>
        /// <param name="obj">The dependency object to set the property on.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetColumns(DependencyObject obj, ObservableCollection<DataGridColumn> value)
        {
            obj.SetValue(ColumnsProperty, value);
        }
        #endregion

        /************************************************************************/

        #region Private methods

        private static void OnDataGridColumnsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d.GetType() == typeof(DataGrid) || d.GetType() == typeof(DataGridExtended))
            {
                DataGrid myGrid = d as DataGrid;

                ObservableCollection<DataGridColumn> Columns = (ObservableCollection<DataGridColumn>)e.NewValue;
                var prop = typeof(DataGridColumn).GetProperty("DataGridOwner", BindingFlags.Instance | BindingFlags.NonPublic);
                if (Columns != null)
                {
                    myGrid.Columns.Clear();

                    if (Columns != null && Columns.Count > 0)
                    {
                        foreach (DataGridColumn col in Columns)
                        {
                            if (prop != null)
                            {
                                prop.SetValue(col, null, null);
                            }
                            myGrid.Columns.Add(col);
                        }
                    }


                    Columns.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs args)
                    {
                        if (args.NewItems != null)
                        {
                            foreach (DataGridColumn column in args.NewItems.Cast<DataGridColumn>())
                            {
                                myGrid.Columns.Add(column);
                            }
                        }

                        if (args.OldItems != null)
                        {
                            foreach (DataGridColumn column in args.OldItems.Cast<DataGridColumn>())
                            {
                                myGrid.Columns.Remove(column);
                            }
                        }
                    };
                }
            }
        }

        #endregion

    }
}
