using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Restless.App.Panama
{
    /// <summary>
    /// Helper class for tag assignment
    /// </summary>
    public class TagCache
    {
        #region Public properties
        /// <summary>
        /// Gets the tag id
        /// </summary>
        public Int64 Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the tag name
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the tag description
        /// </summary>
        public string Description
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/
        
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TagCache"/> class.
        /// </summary>
        /// <param name="id">The tag id.</param>
        /// <param name="name">The tag name.</param>
        /// <param name="description">The tag description.</param>
        public TagCache(Int64 id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
        #endregion
    }
}
