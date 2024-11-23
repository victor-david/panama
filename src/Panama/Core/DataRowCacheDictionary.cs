using System.Collections.Generic;
using System.Data;
using System;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents a dictionary of data rows indexed by a key value
    /// </summary>
    public class DataRowCacheDictionary
    {
        #region Private
        private Dictionary<string, DataRow> storage;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Acceses the dictionary value according to the string key
        /// </summary>
        /// <param name="key">The string key</param>
        /// <returns>The DataRow object, or null if not present</returns>
        public DataRow this [string key]
        {
            get
            {
                if (storage.ContainsKey(key))
                {
                    return storage[key];
                }
                return null;
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRowCacheDictionary"/> class.
        /// </summary>
        public DataRowCacheDictionary()
        {
            storage = new Dictionary<string, DataRow>();
        }

        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Adds a data row
        /// </summary>
        /// <param name="key">The key for the data row</param>
        /// <param name="row">TheDataRow object</param>
        public void Add(string key, DataRow row)
        {
            ValidateArgs(key, row);
            storage.Add(key, row);
        }

        /// <summary>
        /// Adds a data row if it doesn't already exist.
        /// </summary>
        /// <param name="key">The key for the data row</param>
        /// <param name="row">TheDataRow object</param>
        public void AddIf(string key, DataRow row)
        {
            ValidateArgs(key, row);
            if (!storage.ContainsKey(key))
            {
                storage.Add(key, row);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void ValidateArgs(string key, DataRow row)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }
        }
        #endregion
    }
}