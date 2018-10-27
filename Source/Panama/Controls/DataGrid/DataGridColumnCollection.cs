using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Restless.Tools.Utility;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Restless.App.Panama.Converters;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Represents a bindable collection of data grid columns with convienence methods for adding items
    /// </summary>
    public class DataGridColumnCollection : ObservableCollection<DataGridColumn>
    {
        #region Private Vars
        private DataGridColumn defaultSortColumn;
        private ListSortDirection? defaultSortDirection;
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Creates a text column and adds it to the collection
        /// </summary>
        /// <param name="header">The header for the column</param>
        /// <param name="bindingName">The name that the column should bind to.</param>
        /// <returns>The newly created column</returns>
        public DataGridBoundColumn Create(string header, string bindingName)
        {
            Validations.ValidateNullEmpty(header, nameof(header));
            Validations.ValidateNullEmpty(bindingName, nameof(bindingName));
            DataGridTextColumn col = new DataGridTextColumn
            {
                Header = MakeTextBlockHeader(header),
                Binding = new Binding(bindingName)
                {
                    TargetNullValue = "--"
                }
            };
            Add(col);
            return col;
        }

        /// <summary>
        /// Creates a text column that uses a IValueConverter to get its values
        /// </summary>
        /// <typeparam name="T">The converter type</typeparam>
        /// <param name="header">The header for the column</param>
        /// <param name="bindingName">The name that the column should bind to.</param>
        /// <returns>The newly created column.</returns>
        public DataGridBoundColumn Create<T>(string header, string bindingName) where T: IValueConverter, new()
        {
            Validations.ValidateNullEmpty(header, nameof(header));
            Validations.ValidateNullEmpty(bindingName, nameof(bindingName));
            DataGridTextColumn col = new DataGridTextColumn
            {
                Header = MakeTextBlockHeader(header),
                Binding = new Binding(bindingName)
                {
                    Converter = new T(),
                    TargetNullValue = "--",
                }
            };
            Add(col);
            return col;
        }

        /// <summary>
        /// Creates a text column that uses a IMultiValueConverter to get its values
        /// </summary>
        /// <typeparam name="T">The converter type</typeparam>
        /// <param name="header">The header for the column</param>
        /// <param name="bindingNames">The names that the column should bind to.</param>
        /// <returns>The newly created column.</returns>
        public DataGridBoundColumn Create<T>(string header, params string[] bindingNames) where T: IMultiValueConverter, new()
        {
            DataGridTextColumn col = new DataGridTextColumn
            {
                Header = MakeTextBlockHeader(header)
            };

            MultiBinding multiBinding = new MultiBinding
            {
                Converter = new T()
            };
            foreach (string name in bindingNames)
            {
                multiBinding.Bindings.Add(new Binding(name));
            }
            col.Binding = multiBinding;
            col.Binding.TargetNullValue = "--";
            Add(col);
            return col;
        }

        /// <summary>
        /// Creates an image column that displays an image according to the specified converter.
        /// </summary>
        /// <typeparam name="T">The converter type</typeparam>
        /// <param name="header">The column header string</param>
        /// <param name="bindingName">The binding name, i.e the column name within the table</param>
        /// <param name="converterParm">A paramater to pass to the converter</param>
        /// <param name="imageXY">The X and Y size of the image (default 12.0)</param>
        /// <param name="isVisible">true (default) if the column should be visible</param>
        /// <returns>The newly created column.</returns>
        public DataGridTemplateColumn CreateImage<T>(string header, string bindingName, object converterParm = null, double imageXY = 12.0, bool isVisible = true) where T : IValueConverter, new()
        {
            DataGridTemplateColumn col = new DataGridTemplateColumn
            {
                Header = MakeTextBlockHeader(header),
                CanUserResize = false,
                Width = new DataGridLength(imageXY * 1.55, DataGridLengthUnitType.Pixel)
            };

            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(Image));
            Binding binding = new Binding(bindingName)
            {
                Mode = BindingMode.OneWay,
                Converter = new T(),
                ConverterParameter = converterParm
            };
            factory.SetValue(Image.SourceProperty, binding);
            factory.SetValue(Image.WidthProperty, imageXY);
            factory.SetValue(Image.HeightProperty, imageXY);
            DataTemplate cellTemplate = new DataTemplate();
            cellTemplate.VisualTree = factory;
            col.CellTemplate = cellTemplate;
            Add(col);
            return col;
        }

        /// <summary>
        /// Sets the default sort column
        /// </summary>
        /// <param name="col">The column</param>
        /// <param name="sortDirection">The sort direction</param>
        public void SetDefaultSort(DataGridColumn col, ListSortDirection? sortDirection)
        {
            defaultSortColumn = col;
            defaultSortDirection = sortDirection;
            if (defaultSortColumn != null)
            {
                ClearColumnSortDirections();
                defaultSortColumn.SortDirection = defaultSortDirection;
            }
        }

        /// <summary>
        /// Restores the default sort. Must have called SetDefaultSortColumn() prior
        /// </summary>
        public void RestoreDefaultSort()
        {
            if (defaultSortColumn != null)
            {
                ClearColumnSortDirections();
                defaultSortColumn.SortDirection = defaultSortDirection;
            }
        }
        #endregion

        /************************************************************************/

        #region Private Methods

        private TextBlock MakeTextBlockHeader(string text)
        {
            return new TextBlock()
            {
                Text = text
            };
        }

        private void ClearColumnSortDirections()
        {
            foreach (var c in this)
            {
                c.SortDirection = null;
            }
        }

        #endregion
    }
}
