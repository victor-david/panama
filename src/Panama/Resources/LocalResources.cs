/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.Windows;

namespace Restless.Panama.Resources
{
    /// <summary>
    /// Provides static convienance methods to assist with application resources.
    /// </summary>
    public static class LocalResources
    {
        /// <summary>
        /// Defines the name of a style that may be applied to DataGridColumnHeader in order to center it.
        /// </summary>
        public const string StyleDataGridHeaderCenter = "DataGridHeaderCenter";

        /// <summary>
        /// Defines the name of a style that may be applied to TextBlock in order to center it.
        /// </summary>
        public const string StyleTextBlockCenter = "TextBlockCenter";

        /// <summary>
        /// Gets a resource by its key, or null if not found
        /// </summary>
        /// <param name="key">The resource key</param>
        /// <returns>The resource object, or null if no such resource.</returns>
        public static object Get(object key)
        {
            return Application.Current.TryFindResource(key);
        }

        /// <summary>
        /// Gets a resource by its name
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="name">The name of the resource</param>
        /// <returns>The resource, or null if not found</returns>
        public static T Get<T>(object key) where T : class
        {
            return Get(key) as T;
        }
    }
}