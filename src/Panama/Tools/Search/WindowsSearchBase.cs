using Restless.Toolkit.Core.Utility;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Text;
using PropSystem = Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using SysProps = Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Represents the base class for Windows Search. This class must be inherited.
    /// </summary>
    public abstract class WindowsSearchBase
    {
        #region Private
        private const string ConnectionStr = "Provider=Search.CollatorDSO;Extended Properties=\"Application=Windows\"";
        private readonly List<PropSystem.PropertyKey> keys;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a list of search scopes. You must add at least one entry before performing a search.
        /// </summary>
        public List<string> Scopes
        {
            get;
        }

        /// <summary>
        /// Gets a dictionary of property keys and sort direction that determines how to order the results.
        /// </summary>
        public Dictionary<PropSystem.PropertyKey, ListSortDirection> OrderBy
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsSearchBase"/> class.
        /// </summary>
        protected WindowsSearchBase()
        {
            Scopes = new List<string>();
            OrderBy = new Dictionary<PropSystem.PropertyKey, ListSortDirection>();
            keys = new List<PropSystem.PropertyKey>();
            PopulatePropertyKeys();
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// When implemented in a derived class, gets the search results.
        /// </summary>
        /// <param name="expression">The expression to search on.</param>
        /// <returns>A collection of search results</returns>
        public abstract WindowsSearchResultCollection GetSearchResults(string expression);

        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Gets the collection of results, based on the specified sql.
        /// </summary>
        /// <param name="sql">The sql string</param>
        /// <returns>WindowsSearchResultCollection</returns>
        protected WindowsSearchResultCollection GetCollection(string sql)
        {
            WindowsSearchResultCollection collection = new();

            using (OleDbConnection conn = new(ConnectionStr))
            {
                conn.Open();
                OleDbCommand command = new(sql, conn);
                OleDbDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    WindowsSearchResult result = new();
                    foreach (PropSystem.PropertyKey key in keys)
                    {
                        Execution.TryCatchSwallow(() => 
                        {
                            result.Values.SetProperty(key, reader[GetPropName(key)]);
                        });
                    }
                    collection.Add(result);
                }
                conn.Close();
            }

            return collection;
        }

        /// <summary>
        /// Gets the list of select fields
        /// </summary>
        /// <returns>A list of all select fields ready to use in the SELECT statement.</returns>
        protected string GetSelectFieldList()
        {
            StringBuilder sql = new(256);
            foreach (PropSystem.PropertyKey key in keys)
            {
                sql.Append($"{GetPropName(key)},");
            }

            sql.Remove(sql.Length - 1, 1);
            return sql.ToString();
        }

        /// <summary>
        /// Gets the canonical name of the specified property.
        /// </summary>
        /// <param name="key">The property key</param>
        /// <returns>The canonical name</returns>
        protected string GetPropName(PropSystem.PropertyKey key)
        {
            return SysProps.GetPropertyDescription(key).CanonicalName;
        }

        /// <summary>
        /// Appends the order by clause to the specified sql.
        /// </summary>
        /// <param name="sql">The sql string builder object.</param>
        protected void AppendOrderBy(StringBuilder sql)
        {
            StringBuilder orderBy = new();
            if (OrderBy.Count > 0)
            {
                orderBy.Append(" ORDER BY ");
                int k = 0;
                foreach (KeyValuePair<PropSystem.PropertyKey, ListSortDirection> spec in OrderBy)
                {
                    orderBy.Append(GetPropName(spec.Key));
                    orderBy.Append((spec.Value == ListSortDirection.Ascending) ? " ASC" : " DESC");
                    orderBy.Append(k < OrderBy.Count - 1 ? "," : string.Empty);
                    k++;
                }
            }
            sql.Append(orderBy);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        /// <summary>
        /// Populates our list of keys with the property keys that we're interested in.
        /// </summary>
        private void PopulatePropertyKeys()
        {
            keys.Add(SysProps.System.Author);
            keys.Add(SysProps.System.Comment);
            keys.Add(SysProps.System.Company);
            keys.Add(SysProps.System.ContentType);
            keys.Add(SysProps.System.DateCreated);
            keys.Add(SysProps.System.DateModified);
            keys.Add(SysProps.System.FileDescription);
            keys.Add(SysProps.System.FileName);
            keys.Add(SysProps.System.ItemFolderNameDisplay);
            keys.Add(SysProps.System.ItemFolderPathDisplay);
            keys.Add(SysProps.System.ItemName);
            keys.Add(SysProps.System.ItemNameDisplay);
            keys.Add(SysProps.System.ItemPathDisplay);
            keys.Add(SysProps.System.ItemNamePrefix);
            keys.Add(SysProps.System.ItemType);
            keys.Add(SysProps.System.ItemTypeText);
            keys.Add(SysProps.System.ItemUrl);
            keys.Add(SysProps.System.Keywords);
            //keys.Add(SysProps.System.Kind);
            //keys.Add(SysProps.System.KindText);
            keys.Add(SysProps.System.Language);
            keys.Add(SysProps.System.Size);
            keys.Add(SysProps.System.Status);
            keys.Add(SysProps.System.Subject);
            keys.Add(SysProps.System.Title);
            keys.Add(SysProps.System.Status);
            keys.Add(SysProps.System.Document.DateCreated);
            keys.Add(SysProps.System.Document.DateSaved);
            keys.Add(SysProps.System.Message.DateReceived);
            keys.Add(SysProps.System.Message.DateSent);
            keys.Add(SysProps.System.Message.FromAddress);
            keys.Add(SysProps.System.Message.FromName);
            keys.Add(SysProps.System.Message.SenderAddress);
            keys.Add(SysProps.System.Message.SenderName);
            keys.Add(SysProps.System.Message.Store);
            keys.Add(SysProps.System.Message.ToAddress);
            keys.Add(SysProps.System.Message.ToName);
        }
        #endregion
    }
}
