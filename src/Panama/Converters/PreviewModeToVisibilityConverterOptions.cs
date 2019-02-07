/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace Restless.App.Panama.Converters
{
    /// <summary>
    /// Provides option values that may be passed to <see cref="PreviewModeToVisibilityConverter"/>
    /// that affect how the converter evaluates the passed PreviewMode value.
    /// </summary>
    public enum PreviewModeToVisibilityConverterOptions
    {
        /// <summary>
        /// If the value is PreviewMode.Text, return <see cref="Visibility.Visible"/>; otherwise, <see cref="Visibility.Collapsed"/>.
        /// </summary>
        TextToVisibility,

        /// <summary>
        /// If the value is PreviewMode.Image, return <see cref="Visibility.Visible"/>; otherwise, <see cref="Visibility.Collapsed"/>.
        /// </summary>
        ImageToVisibility,

        /// <summary>
        /// If the value is PreviewMode.Unsupported, return <see cref="Visibility.Visible"/>; otherwise, <see cref="Visibility.Collapsed"/>.
        /// </summary>
        UnsupportedToVisibility,
    }
}