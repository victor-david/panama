using System.Collections.Generic;
using System.Linq;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents a list of language items
    /// </summary>
    public class LanguageItemCollection : List<LanguageItem>
    {
        /// <summary>
        /// Gets the language item with the specified id
        /// </summary>
        /// <param name="id">The id</param>
        /// <returns>The item, or null if not found</returns>
        public LanguageItem GetLanguageItem(string id) => this.FirstOrDefault(item => item.Id == id);


        //public void SetDisplayLanguage(LanguageItem item)
        //{
        //    ForEach(lang => lang.SetDisplayLanguage(item));
        //}
    }
}