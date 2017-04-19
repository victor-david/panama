using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides enumeration values that describe the types of submission documents that may be created.
    /// </summary>
    public enum SubmissionDocumentCreateType
    {
        /// <summary>
        /// No selection
        /// </summary>
        None,
        /// <summary>
        /// Create a document
        /// </summary>
        CreateDocX,
        /// <summary>
        /// Create a placeholder entry
        /// </summary>
        CreatePlaceholder,
    }

}
