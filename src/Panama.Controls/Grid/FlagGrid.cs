using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Restless.Panama.Controls
{
    /// <summary>
    /// Provides a specialized grid control to display a series of flags
    /// </summary>
    public class FlagGrid : Grid
    {
        #region Private
        private readonly int flagSize;
        private int flagCount;
        private FlagGridColumnCollection columns;
        #endregion

        /************************************************************************/

        #region Constructors
        private FlagGrid()
        {
            Margin = new Thickness(2, 0, 2, 0);
            flagSize = 12;
        }

        public static FlagGrid Create()
        {
            return new FlagGrid();
        }
        #endregion

        /************************************************************************/

        #region Public methods

        public FlagGrid SetColumns(FlagGridColumnCollection columns)
        {
            this.columns = columns;
            return this;
        }

        public FlagGrid CreateFlags(object[] flags)
        {
            flagCount = flags?.Length ?? throw new ArgumentNullException(nameof(flags));
            CreateColumns();
            CreateFlagsPrivate(flags);
            return this;
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void CreateColumns()
        {
            ColumnDefinitions.Clear();
            for (int idx = 0; idx < flagCount; idx++)
            {
                ColumnDefinitions.Add(new ColumnDefinition() 
                {
                    Width = new GridLength(flagSize)
                });
            }
        }

        private void CreateFlagsPrivate(object[] flags)
        {
            for (int col = 0; col < flagCount; col++)
            {
                if (flags[col] is bool value && value)
                {
                    Children.Add(CreateFlag(col));
                }
            }
        }

        private Border CreateFlag(int column)
        {
            Border border = new Border()
            {
                Margin = new Thickness(1),
                Height = flagSize - 2,
                Width = flagSize - 2,
                CornerRadius = new CornerRadius(1),
                Background = GetBrush(column)
            };

            SetColumn(border, column);
            return border;
        }

        private Brush GetBrush(int column)
        {
            return columns != null && column < columns.Count - 1 ? columns[column].Brush : Brushes.Transparent;
        }
        #endregion
    }
}