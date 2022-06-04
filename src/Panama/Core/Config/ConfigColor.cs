/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Mvvm;
using System.Data;
using System.Globalization;
using System.Windows.Media;
using SystemColors = System.Windows.Media.Colors;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents the foreground and background colors for a single configuration color item.
    /// </summary>
    public class ConfigColor : ObservableObject
    {
        #region Private
        private readonly ColorTable colorTable;
        private readonly DataRow colorRow;
        private Color defaultColor;
        #endregion

        /************************************************************************/

        #region Public Properties
        /// <summary>
        /// Gets or sets the color.
        /// </summary>
        public Color Color
        {
            get => GetColor();
            set => SetColor(value);
        }

        /// <summary>
        /// Gets a brush that corresponds to <see cref="Color"/>
        /// </summary>
        public Brush ColorBrush => new SolidColorBrush(Color);

        /// <summary>
        /// Gets a boolean value that indicates if <see cref="Color"/> has its alpha channel set to a value greater than zero.
        /// </summary>
        public bool HasColor => Color.A > 0;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigColor"/> class.
        /// </summary>
        /// <param name="id">The id associated with this color.</param>
        /// <param name="defaultColor">The default value for the foreground color.</param>
        public ConfigColor(string id, Color defaultColor)
        {
            this.defaultColor = defaultColor;
            colorTable = DatabaseController.Instance.GetTable<ColorTable>();
            colorRow = colorTable.GetConfigurationRow(id, defaultColor);
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Resets this configuration color to its default value.
        /// </summary>
        public void ResetToDefault()
        {
            Color = defaultColor;
        }

        /// <summary>
        /// Gets the binding path to <see cref="ColorBrush"/>
        /// </summary>
        /// <returns>A binding path string</returns>
        public string ToBindingPath()
        {
            return $"{nameof(Config)}.{nameof(Config.Colors)}.{colorRow[ColorTable.Defs.Columns.Id]}.{nameof(ColorBrush)}";
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private Color GetColor()
        {
            string value = GetColorStringFromRow();
            try
            {
                return (Color)ColorConverter.ConvertFromString(value);
            }
            catch
            {
                return SystemColors.Transparent;
            }
        }

        private void SetColor(Color value)
        {
            if (value != GetColor())
            {
                colorRow[ColorTable.Defs.Columns.Color] = value.ToString(CultureInfo.InvariantCulture);
                OnPropertyChanged(nameof(Color));
                OnPropertyChanged(nameof(ColorBrush));
                OnPropertyChanged(nameof(HasColor));
            }
        }

        private string GetColorStringFromRow()
        {
            return colorRow[ColorTable.Defs.Columns.Color].ToString();
        }
        #endregion
    }
}