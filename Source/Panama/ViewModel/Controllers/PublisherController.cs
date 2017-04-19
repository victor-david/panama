using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.Tools.Database.SQLite;
using System.ComponentModel;
using Restless.App.Panama.Database.Tables;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Base class for publisher supplemental controllers.
    /// Currently, this class provides no additional functionality
    /// </summary>
    public abstract class PublisherController : TitleLatestVersionController<PublisherViewModel, PublisherTable>
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties
        #endregion

        /************************************************************************/

        #region Protected properties
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        protected PublisherController(PublisherViewModel owner)
            : base(owner)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region Protected methods
        #endregion

        /************************************************************************/

        #region Private methods
        #endregion
    }
}
