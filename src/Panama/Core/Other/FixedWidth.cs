/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/

namespace Restless.App.Panama.Core
{
    /// <summary>
    /// Provides standard fixed width values used when creating DataGrid columns
    /// </summary>
    public class FixedWidth
    {
        /// <summary>
        /// Standard fixed width for an id column, 42
        /// </summary>
        public const int Standard = 42;

        /// <summary>
        /// Fixed width for short string, 48
        /// </summary>
        public const int ShortString = 48;

        /// <summary>
        /// Fixed width for medium string, 96
        /// </summary>
        public const int MediumString = 96;

        /// <summary>
        /// Fixed width for a column that displays a bit longer numeric data, 52.
        /// </summary>
        public const int MediumNumeric = 52;

        /// <summary>
        /// Fixed width for a column that displays longer numeric data, 76.
        /// </summary>
        public const int LongerNumeric = 76;

        /// <summary>
        /// Fixed width for a column that displays longer data, 180
        /// </summary>
        public const int LongString = 180;
    }
}