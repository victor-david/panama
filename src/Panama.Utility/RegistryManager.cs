using System;
using System.IO;
using Microsoft.Win32;

namespace Restless.Panama.Utility
{
    /// <summary>
    /// Provides static methods and properties to access the registry
    /// </summary>
    public static class RegistryManager
    {
        #region Private
        private const string RegistryKey = @"Software\RestlessAnimal\Panama";
        private const string AppDirectory = @"RestlessAnimal\Panama";
        private const string DatabaseDirectoryValue = "DatabaseDirectory";
        #endregion

        /************************************************************************/

        #region Public
        public static string AppDataDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDirectory);

        /// <summary>
        /// Gets the directory for the database.
        /// </summary>
        public static string DatabaseDirectory
        {
            get => Get<string>(DatabaseDirectoryValue, AppDataDirectory);
        }

        /// <summary>
        /// Initializes the registry manager.
        /// </summary>
        public static void Initialize()
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryKey))
            {
                if (key.GetValue(DatabaseDirectoryValue) == null)
                {
                    key.SetValue(DatabaseDirectoryValue, AppDataDirectory);
                }
            }
        }

        /// <summary>
        /// Sets the database directory to the specified value.
        /// </summary>
        /// <param name="value">The value to set.</param>
        public static void SetDatabaseDirectory(string value)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryKey))
            {
                key.SetValue(DatabaseDirectoryValue, value);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private static T Get<T>(string valueName, string defaultValue) where T :class
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryKey))
            {
                return key.GetValue(valueName, defaultValue) as T;
            }
        }
        #endregion

    }
}