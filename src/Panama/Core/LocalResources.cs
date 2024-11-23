using System.Windows;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides static convienance methods to assist with application resources.
    /// </summary>
    public static class LocalResources
    {
        /// <summary>
        /// Gets a resource by its key, or null if not found
        /// </summary>
        /// <param name="key">The resource key</param>
        /// <returns>The resource object, or null if no such resource.</returns>
        public static object Get(object key)
        {
            return key is not null ? Application.Current.TryFindResource(key) : null;
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