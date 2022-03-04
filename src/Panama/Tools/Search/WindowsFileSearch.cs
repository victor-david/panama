using Restless.Panama.Resources;
using System.Collections.Generic;
using System.Text;
using System;
using SysProps = Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Extends <see cref="WindowsSearchBase"/> to provide file search capability.
    /// </summary>
    public class WindowsFileSearch : WindowsSearchBase
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a list of scopes to be excluded from the search. Only applies to file search
        /// </summary>
        public List<string> ExcludedScopes
        {
            get;
        }

        /// <summary>
        /// Gets a list of types to be excluded from the search. Only applies to file search.
        /// Default exclusions are "Directory", ".dll", ".exe", ".zip", ".cmd"
        /// </summary>
        public List<string> ExcludedTypes
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsFileSearch"/> class.
        /// </summary>
        public WindowsFileSearch()
        {
            ExcludedScopes = new List<string>();
            ExcludedTypes = new List<string>
            {
                "Directory",
                ".dll",
                ".exe",
                ".zip",
                ".cmd",
                ".pspimage"
            };
        }
        #endregion

        /************************************************************************/
        
        #region Public methods
        /// <summary>
        /// Gets the search results for the specified expression
        /// </summary>
        /// <param name="expression">The expression</param>
        /// <returns>A WindowsSearchResultCollection object</returns>
        public override WindowsSearchResultCollection GetSearchResults(string expression)
        {
            if (Scopes.Count == 0)
            {
                throw new InvalidOperationException(Strings.InvalidOperationNoSearchScope);
            }

            StringBuilder sql = new(512);
            sql.AppendFormat("SELECT {0}", GetSelectFieldList());

            sql.Append(" FROM SystemIndex");
            sql.Append(" WHERE (");
            for (int k = 0; k < Scopes.Count; k++)
            {
                sql.AppendFormat(" scope='file:{0}'", Scopes[k]);
                sql.Append(k < Scopes.Count - 1 ? " OR " : string.Empty);

            }
            sql.Append(")");

            foreach (string scope in ExcludedScopes)
            {
                if (!string.IsNullOrEmpty(scope))
                {
                    sql.AppendFormat(" AND NOT scope='file:{0}'", scope);
                }
            }

            sql.AppendFormat(" AND CONTAINS(\"ALL\",'\"{0}*\"')", expression);
            //sql.AppendFormat(" AND FREETEXT('{0}*')", expression);

            foreach (string type in ExcludedTypes)
            {
                sql.AppendFormat(" AND {0} != '{1}'", GetPropName(SysProps.System.ItemType), type);
            }

            AppendOrderBy(sql);
            return GetCollection(sql.ToString());
        }
        #endregion

    }
}
