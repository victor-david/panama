using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Resources;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Provides extension methods for DataGridColumn
    /// </summary>
    public static class DataGridColumnExtensions
    {
        /// <summary>
        /// The default fixed width to be applied when using the MakeDate() extension
        /// </summary>
        public const int DefaultWidth = 100;

        /// <summary>
        /// Formats the column to display a date and makes the column fixed width.
        /// </summary>
        /// <param name="col">The column.</param>
        /// <param name="dateFormat">The desired date format. Null (the default) uses the application default format</param>
        /// <param name="width">The desired width. Default is <see cref="DefaultWidth"/>.</param>
        /// <returns>The column</returns>
        public static DataGridBoundColumn MakeDate(this DataGridBoundColumn col, string dateFormat = null, int width = DefaultWidth)
        {
            if (String.IsNullOrEmpty(dateFormat))
            {
                dateFormat = Config.Instance.DateFormat;
            }
            col.Binding.StringFormat = dateFormat;
            col.MakeFixedWidth(width);
            return col;
        }

        /// <summary>
        /// Formats the column to display a number and makes the column fixed width.
        /// </summary>
        /// <param name="col">The column</param>
        /// <param name="numericFormat">The desired date format. Null (the default) uses a standard numeric format.</param>
        /// <param name="width">The desired width. Default is <see cref="DefaultWidth"/>.</param>
        /// <returns>The column</returns>
        public static DataGridBoundColumn MakeNumeric(this DataGridBoundColumn col, string numericFormat = null, int width = DefaultWidth)
        {
            if (String.IsNullOrEmpty(numericFormat))
            {
                numericFormat = "N0";
            }
            col.Binding.StringFormat = numericFormat;
            col.MakeFixedWidth(width);
            return col;
        }

        /// <summary>
        /// Sets the column's ElementStyle property to the specified style.
        /// </summary>
        /// <param name="col">The column.</param>
        /// <param name="style">The style to set.</param>
        /// <returns>The column</returns>
        public static DataGridBoundColumn AddCellStyle(this DataGridBoundColumn col, Style style)
        {
            if (style != null)
            {
                col.ElementStyle = style;
            }
            return col;
        }

        /// <summary>
        /// Sets the column's HeaderStyle property to the specified style.
        /// </summary>
        /// <param name="col">The column.</param>
        /// <param name="style">The style to set.</param>
        /// <returns>The column.</returns>
        public static DataGridBoundColumn AddHeaderStyle(this DataGridBoundColumn col, Style style)
        {

            if (style != null && style.TargetType == typeof(DataGridColumnHeader))
            {
                col.HeaderStyle = style;
            }
            return col;
        }

        /// <summary>
        /// Makes the column fixed width, unable to resize
        /// </summary>
        /// <param name="col">The column</param>
        /// <param name="width">The desired width. Default is <see cref="DefaultWidth"/>.</param>
        /// <returns>The column</returns>
        public static DataGridColumn MakeFixedWidth(this DataGridColumn col, int width = DefaultWidth)
        {
            col.Width = width;
            col.CanUserResize = false;
            return col;
        }

        /// <summary>
        /// Makes the column flexible width.
        /// </summary>
        /// <param name="col">The column</param>
        /// <param name="width">The flex width factor, 1 is standard, 2 is double, etc.</param>
        /// <returns>The column</returns>
        public static DataGridBoundColumn MakeFlexWidth(this DataGridBoundColumn col, double width)
        {
            col.Width = new DataGridLength(width, DataGridLengthUnitType.Star);
            return col;
        }

        /// <summary>
        /// Makes the column single line display by attaching a converter that removes EOL.
        /// </summary>
        /// <param name="col">The column</param>
        /// <returns>The column</returns>
        public static DataGridBoundColumn MakeSingleLine(this DataGridBoundColumn col)
        {
            ((System.Windows.Data.Binding)col.Binding).Converter = new StringToCleanStringConverter();
            return col;
        }

        /// <summary>
        /// Makes the column a masked display by attaching a converter.
        /// </summary>
        /// <param name="col">The column</param>
        /// <returns>The column</returns>
        public static DataGridBoundColumn MakeMasked(this DataGridBoundColumn col)
        {
            ((System.Windows.Data.Binding)col.Binding).Converter = new StringToMaskedStringConverter();
            return col;
        }
        

        /// <summary>
        /// Adds the specified tooltip text to the column's header
        /// </summary>
        /// <param name="col">The column</param>
        /// <param name="toolTip">The text of the tool tip</param>
        /// <returns>The column</returns>
        public static DataGridColumn AddToolTip(this DataGridColumn col, string toolTip)
        {
            if (!String.IsNullOrEmpty(toolTip))
            {
                if (col.HeaderStyle == null)
                {
                    col.HeaderStyle = new Style(typeof(DataGridColumnHeader), (Style)ResourceHelper.Get("DataGridHeaderDefault"));
                }
                col.HeaderStyle.Setters.Add(new Setter(ToolTipService.ToolTipProperty, toolTip));
            }
            return col;
        }

        /// <summary>
        /// Adds a sort specification to the column
        /// </summary>
        /// <param name="col">The column</param>
        /// <param name="column1">The name of the column to act as primary sort, or null to use the <paramref name="col"/></param>
        /// <param name="column2">The name of the column to act as a secondary sort.</param>
        /// <param name="behavior">The behavior of the secondary column when sorting.</param>
        /// <returns>The column</returns>
        /// <remarks>
        /// This extension adds a <see cref="DataGridColumnSortSpec"/> property to the column, creating
        /// a secondary sort on this column.
        /// </remarks>
        public static DataGridColumn AddSort(this DataGridColumn col, string column1, string column2, DataGridColumnSortBehavior behavior)
        {
            col.SetValue(DataGridExtended.CustomSortProperty, new DataGridColumnSortSpec(column1, column2, behavior));
            return col;
        }

        //private static DataGridColumn AddToolTipPrivate()
        //{
        //}
    }
}
