/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Text.RegularExpressions;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides a converter that accepts a string value and returns a string value that has been cleaned up.
    /// </summary>
    public static class StringClean
    {
        #region Public methods
        /// <summary>
        /// Cleans up the string according to the specified options.
        /// </summary>
        /// <param name="str">The string value</param>
        /// <param name="options">Options</param>
        /// <returns>The cleaned string</returns>
        public static string Clean(string str, StringCleanOptions options)
        {
            if (options.HasFlag(StringCleanOptions.RemoveHtml))
            {
                int startStyle = str.IndexOf("<style", StringComparison.OrdinalIgnoreCase);
                int endStyle = str.IndexOf("</style>", StringComparison.OrdinalIgnoreCase);
                if (startStyle != -1 && endStyle != -1)
                {
                    str = $"{str.Substring(0, startStyle)}{str[(endStyle + 8)..]}";
                }
                str =  Regex.Replace(str, "\\<[^\\>]*\\>", string.Empty);
            }

            if (options.HasFlag(StringCleanOptions.TrimInterior))
            {
                // this replaces all double white space with a single space
                // str = Regex.Replace(str, @"\s+", " ");
                // this replaces are double white space with the same type of white space.
                str = Regex.Replace(str, @"(\s)\s+", "$1");
            }
            return str.Trim();
        }
        #endregion
    }
}