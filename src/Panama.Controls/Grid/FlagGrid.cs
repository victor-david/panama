using System;
using System.Windows;
using System.Windows.Controls;

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
            for (int colIdx = 0; colIdx < flagCount; colIdx++)
            {
                if (flags[colIdx] is bool value && value)
                {
                    Children.Add(CreateFlag(colIdx));
                }
            }
        }

        private Border CreateFlag(int colIdx)
        {
            Border border = new Border()
            {
                Margin = new Thickness(1),
                Height = flagSize - 2,
                Width = flagSize - 2,
                CornerRadius = new CornerRadius(1),
                DataContext = columns.DataContext
            };

            border.SetBinding(Border.BackgroundProperty, columns[colIdx].BindingPath);
            SetColumn(border, colIdx);
            return border;
        }
        #endregion
    }
}