using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Database.SQLite;
using Restless.Tools.Utility;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Submission submitted controller. Handles the submission submitted date
    /// </summary>
    public class SubmissionSubmittedController : SubmissionController
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets the submitted date
        /// </summary>
        public object SubmittedDate
        {
            get
            {
                if (Owner.SelectedRow != null)
                {
                    return Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Submitted];
                }
                return null;
            }
            set
            {
                if (Owner.SelectedRow != null)
                {
                    Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Submitted] = value;
                    OnSubmittedPropertiesChanged();
                }
            }
        }

        /// <summary>
        /// Gets the submitted date display name. Used to bind to the edit view
        /// Can't bind directly DisplayDate="{Binding  SelectedRow[submitted]}"
        /// because when there isn't yet a response date, the calendar shows January, 0001
        /// </summary>
        public DateTime DisplayDate
        {
            get
            {
                DateTime displayDate = DateTime.UtcNow;
                if (Owner.SelectedRow != null && Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Submitted] != DBNull.Value)
                {
                    displayDate = (DateTime)Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Submitted];
                }
                return displayDate;
            }
        }

        /// <summary>
        /// Gets a string value that displays the submitted date
        /// </summary>
        public override string Header
        {
            get
            {
                string dateStr = Strings.TextNone;
                if (Owner.SelectedRow != null && Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Submitted] != DBNull.Value)
                {
                    dateStr = ((DateTime)Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Submitted]).ToString(Config.Instance.DateFormat);
                }
                return String.Format("{0}: {1}", Strings.TextSubmitted, dateStr);
            }
        }
        #endregion

        /************************************************************************/

        #region Protected properties
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionSubmittedController"/> class.
        /// </summary>
        /// <param name="owner">The owner of this controller.</param>
        public SubmissionSubmittedController(SubmissionViewModel owner)
            : base(owner)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called by the <see cref=" ControllerBase{VM,T}.Owner"/> of this controller
        /// in order to update the controller values.
        /// </summary>
        protected override void OnUpdate()
        {
            OnSubmittedPropertiesChanged();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void OnSubmittedPropertiesChanged()
        {
            OnPropertyChanged("SubmittedDate");
            OnPropertyChanged("DisplayDate");
            OnPropertyChanged("Header");
        }
        #endregion

    }
}
