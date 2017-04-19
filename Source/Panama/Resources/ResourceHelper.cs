using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Restless.App.Panama.Resources
{
    /// <summary>
    /// Provides static convienance methods to assist with application resources.
    /// </summary>
    public static class ResourceHelper
    {
        /// <summary>
        /// Gets a resource by its name
        /// </summary>
        /// <param name="name">The name of the resource</param>
        /// <returns>The resource object, or null if no such resource.</returns>
        public static object Get(string name)
        {
            return Application.Current.TryFindResource(name);
        }
    }
}
