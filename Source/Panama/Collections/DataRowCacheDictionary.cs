using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.Tools.Utility;
using System.Data;

namespace Restless.App.Panama.Collections
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
        #pragma warning disable 1591
        public DataRowCacheDictionary()
        {
            storage = new Dictionary<string, DataRow>();
        }
        #pragma warning restore 1591
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
            Validations.ValidateNullEmpty(key, "Add.Key");
            Validations.ValidateNull(row, "Add.Row");
            storage.Add(key, row);
        }

        /// <summary>
        /// Adds a data row if it doesn't already exist.
        /// </summary>
        /// <param name="key">The key for the data row</param>
        /// <param name="row">TheDataRow object</param>
        public void AddIf(string key, DataRow row)
        {
            Validations.ValidateNullEmpty(key, "AddIf.Key");
            Validations.ValidateNull(row, "AddIf.Row");
            if (!storage.ContainsKey(key))
            {
                storage.Add(key, row);
            }
        }
        #endregion
    }
}
