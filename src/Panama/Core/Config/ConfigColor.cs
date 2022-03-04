/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Mvvm;
using System;
using System.Data;
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
        private readonly string id;
        private Color defaultFore;
        private Color defaultBack;
        #endregion

        /************************************************************************/

        #region ColorType enumeration
        /// <summary>
        /// Provides an enumeration that describes the color types.
        /// </summary>
        public enum ColorType
        {
            /// <summary>
            /// Specifies operation for the foreground color.
            /// </summary>
            Foreground,
            /// <summary>
            /// Specifies operation for the background color.
            /// </summary>
            Background
        }
        #endregion

        /************************************************************************/

        #region Public Properties
        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        public Color Foreground
        {
            get => GetColor(ColorType.Foreground);
            set => SetColor(ColorType.Foreground, value);
        }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public Color Background
        {
            get => GetColor(ColorType.Background);
            set => SetColor(ColorType.Background, value);
        }

        /// <summary>
        /// Gets the foreground brush.
        /// </summary>
        public Brush ForegroundBrush
        {
            get => new SolidColorBrush(Foreground);
        }

        /// <summary>
        /// Gets the background brush.
        /// </summary>
        public Brush BackgroundBrush
        {
            get =>  new SolidColorBrush(Background);
        }

        /// <summary>
        /// Gets a boolean value that indicates if <see cref="Foreground"/> has its alpha channel set to a value greater than zero.
        /// </summary>
        public bool HasForeground
        {
            get => Foreground.A > 0;
        }

        /// <summary>
        /// Gets a boolean value that indicates if <see cref="Background"/> has its alpha channel set to a value greater than zero.
        /// </summary>
        public bool HasBackground
        {
            get => Background.A > 0;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigColor"/> class.
        /// </summary>
        /// <param name="id">The id associated with this color.</param>
        /// <param name="defaultFore">The default value for the foreground color.</param>
        /// <param name="defaultBack">The default value for the background color.</param>
        public ConfigColor(string id, Color defaultFore, Color defaultBack)
        {
            this.id = id ?? throw new ArgumentNullException(nameof(id));
            this.defaultFore = defaultFore;
            this.defaultBack = defaultBack;
            colorTable = DatabaseController.Instance.GetTable<ColorTable>();
            colorRow = colorTable.GetConfigurationRow(id, defaultFore, defaultBack);
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Resets this configuration color to its default value.
        /// </summary>
        public void ResetToDefault()
        {
            Foreground = defaultFore;
            Background = defaultBack;
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private Color GetColor(ColorType colorType)
        {
            string value = GetColorStringFromRow(colorType);
            try
            {
                return (Color)ColorConverter.ConvertFromString(value);
            }
            catch
            {
                return SystemColors.Transparent;
            }
        }

        private void SetColor(ColorType colorType, Color value)
        {
            if (value != GetColor(colorType))
            {
                if (colorType == ColorType.Foreground)
                {
                    colorRow[ColorTable.Defs.Columns.Foreground] = value.ToString();
                    OnPropertyChanged(nameof(Foreground));
                    OnPropertyChanged(nameof(ForegroundBrush));
                    OnPropertyChanged(nameof(HasForeground));
                }
                else
                {
                    colorRow[ColorTable.Defs.Columns.Background] = value.ToString();
                    OnPropertyChanged(nameof(Background));
                    OnPropertyChanged(nameof(BackgroundBrush));
                    OnPropertyChanged(nameof(HasBackground));
                }
            }
        }

        private string GetColorStringFromRow(ColorType colorType)
        {
            if (colorType == ColorType.Foreground)
                return colorRow[ColorTable.Defs.Columns.Foreground].ToString();
            else
                return colorRow[ColorTable.Defs.Columns.Background].ToString();

        }
        #endregion
    }
}