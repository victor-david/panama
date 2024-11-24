using Restless.Panama.Database.Tables;
using System;
using PropSystem = Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using SysProps = Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Represents a single result from a Windows search operation
    /// </summary>
    public class WindowsSearchResult
    {
        #region Public properties
        /// <summary>
        /// Gets the values for this search result.
        /// </summary>
        public SearchValueCollection Values
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor (internal)
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsSearchResult"/> class.
        /// </summary>
        internal WindowsSearchResult()
        {
            Values = new SearchValueCollection();
        }
        #endregion

        /************************************************************************/
        
        #region Public methods
        /// <summary>
        /// Creates a <see cref="SearchRowItem"/> from current values
        /// </summary>
        /// <param name="versionExists">true if version exists</param>
        /// <returns>A <see cref="SearchRowItem"/> to pass to the table inserter</returns>
        public SearchRowItem ToSearchTableItem(bool versionExists)
        {
            return new SearchRowItem()
            {
                Type = Values.GetValue<string>(SysProps.System.ItemType),
                File = Values.GetValue<string>(SysProps.System.ItemPathDisplay),
                Title = Values.GetValue<string>(SysProps.System.Title),
                Author = Values.GetValue<string>(SysProps.System.Author),
                Company = Values.GetValue<string>(SysProps.System.Company),
                Size = (int)Values.GetValue<decimal>(SysProps.System.Size),
                IsVersion = versionExists,
                Created = Values.GetValue<DateTime>(SysProps.System.DateCreated),
                Modified = Values.GetValue<DateTime>(SysProps.System.DateModified),
            };
        }

        /// <summary>
        /// Sets the System.ItemPathDisplay property
        /// </summary>
        /// <param name="value">The value to set</param>
        public void SetItemPathDisplay(string value)
        {
            Values.SetProperty(SysProps.System.ItemPathDisplay, value);
        }
        #endregion
    }
}