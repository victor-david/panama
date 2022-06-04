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

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides option values that may be passed to <see cref="BooleanToVisibilityMultiConverter"/>
    /// that affect how the converter evaluates the passed boolean values.
    /// </summary>
    public enum BooleanToVisibilityMultiConverterOptions
    {
        /// <summary>
        /// If the first boolean value is true, or the second one is true, returns <see cref="Visibility.Collapsed"/>;
        /// otherwise, <see cref="Visibility.Visible"/>.
        /// </summary>
        OneTrueOrTwoTrue,

        /// <summary>
        /// If the first boolean value is true, and the second one is true, returns <see cref="Visibility.Collapsed"/>;
        /// otherwise, <see cref="Visibility.Visible"/>.
        /// </summary>
        OneTrueAndTwoTrue,

        /// <summary>
        /// If the first boolean value is false, or the second one is true, returns <see cref="Visibility.Collapsed"/>;
        /// otherwise, <see cref="Visibility.Visible"/>.
        /// </summary>
        OneFalseOrTwoTrue

    }
}