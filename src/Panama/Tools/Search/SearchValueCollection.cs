using System.Collections.Generic;
using System.Text;
using PropSystem = Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using SysProps = Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Represents a key value pair of propetry keys and their associated values.
    /// </summary>
    public class SearchValueCollection
    {
        #region Private
        private readonly Dictionary<string, object> values;
        #endregion

        /************************************************************************/

        #region Public indexers
        /// <summary>
        /// Gets the property value by its property key.
        /// </summary>
        /// <param name="index">The property key index.</param>
        /// <returns>The value associated with <paramref name="index"/>, or null if not present.</returns>
        public object this[PropSystem.PropertyKey index]
        {
            get
            {
                var desc = SysProps.GetPropertyDescription(index);
                return this[desc.CanonicalName];
            }
        }

        /// <summary>
        /// Gets the property value by its property name.
        /// </summary>
        /// <param name="index">The property name index.</param>
        /// <returns>The value associated with <paramref name="index"/>, or null if not present.</returns>
        public object this[string index]
        {
            get 
            {
                if (values.ContainsKey(index))
                {
                    return values[index];
                }
                return null;
            }
        }
        #endregion

        /************************************************************************/
        
        #region Constructor (internal)
        internal SearchValueCollection()
        {
            values = new Dictionary<string, object>();
        }
        #endregion

        /************************************************************************/
        
        #region Public methods
        /// <summary>
        /// Gets a string representation of all the key / value pairs in the collection.
        /// </summary>
        /// <returns>a string representation of all the key / value pairs in the collection.</returns>
        public override string ToString()
        {
            StringBuilder b = new();
            b.AppendLine("SearchValueCollection");
            b.AppendLine("=====================");
            foreach (string key in values.Keys)
            {
                b.AppendLine(string.Format("{0} -> {1}", key, values[key].ToString()));
            }
            return b.ToString();
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        internal void SetProperty(PropSystem.PropertyKey key, object value)
        {
            var desc = SysProps.GetPropertyDescription(key);
            var formattedValue = GetValue(value);
            string index = desc.CanonicalName;
            if (values.ContainsKey(index))
            {
                values[index] = formattedValue;
            }
            else
            {
                values.Add(index, formattedValue);
            }
        }
        #endregion

        /************************************************************************/
        
        #region Private methods
        private object GetValue(object value)
        {
            if (value is string[] && ((string[])value).Length > 0)
            {
                string retValue = string.Empty;
                foreach (string a in (string[])value)
                {
                    retValue += string.Format("{0}, ", a);
                }
                retValue = retValue[0..^2];
                return retValue;
            }
            return value;
        }
        #endregion
    }
}
