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
using System.Threading.Tasks;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Provides an enumeration of the modes in which the color pallete may be sorted.
    /// </summary>
    public enum ColorSortingMode
    {
        /// <summary>
        /// Alphabetical by color name.
        /// </summary>
        Alpha,
        /// <summary>
        /// Hue, then saturation, then brightness.
        /// </summary>
        HSB
    }
}