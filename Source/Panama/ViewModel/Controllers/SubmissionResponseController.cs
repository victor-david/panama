using Restless.App.Panama.Configuration;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using System;
using System.Data;
using System.Windows.Input;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Submission response controller. Handles response tasks such as response date and response type.
    /// </summary>
    public class SubmissionResponseController : SubmissionController
    {
        #region Private
        //private DocumentTypeTable documentTypeTable;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a value that indicates if the selected row contains a response date
        /// </summary>
        public bool HaveResponseDate
        {
            get
            {
                return (Owner.SelectedRow != null && Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Response] != DBNull.Value);
            }
        }

        /// <summary>
        /// Gets or sets the response date
        /// </summary>
        public object ResponseDate
        {
            get
            {
                if (Owner.SelectedRow != null)
                {
                    return Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Response];
                }
                return null;
            }
            set
            {
                if (Owner.SelectedRow != null)
                {
                    if (value != null)
                    {
                        object prevResponseDate = Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Response];
                        Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Response] = value;
                        if (prevResponseDate == DBNull.Value)
                        {
                            ResponseType = ResponseTable.Defs.Values.ResponseNotSpecified;
                        }
                    }
                    else
                    {
                        Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Response] = DBNull.Value;
                        ResponseType = ResponseTable.Defs.Values.NoResponse;
                    }
                    OnResponsePropertiesChanged();
                }
            }
        }

        /// <summary>
        /// Gets a string value that displays the response date
        /// </summary>
        public override string Header
        {
            get
            {
                string dateStr = Strings.TextNone;
                if (Owner.SelectedRow != null && Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Response] != DBNull.Value)
                {
                    dateStr = ((DateTime)Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Response]).ToString(Config.Instance.DateFormat);
                }
                return string.Format("{0}: {1}", Strings.TextResponse, dateStr);
            }
        }

        /// <summary>
        /// Gets or sets the response type
        /// </summary>
        public long ResponseType
        {
            get
            {
                if (Owner.SelectedRow != null)
                {
                    return (long)Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.ResponseType];
                }
                return ResponseTable.Defs.Values.NoResponse;
            }
            set
            {
                if (Owner.SelectedRow != null)
                {
                    Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.ResponseType] = value;
                    OnResponsePropertiesChanged();
                }
            }
        }

        /// <summary>
        /// Gets the command that clears the response.
        /// </summary>
        public ICommand ClearResponseCommand
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Protected properties
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionResponseController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public SubmissionResponseController(SubmissionViewModel owner)
            : base(owner)
        {
            AssignDataViewFrom(DatabaseController.Instance.GetTable<ResponseTable>());
            DataView.Sort = ResponseTable.Defs.Columns.Id;
            DataView.RowFilter = string.Format("{0} <> {1}", ResponseTable.Defs.Columns.Id, ResponseTable.Defs.Values.NoResponse);
            ClearResponseCommand = new RelayCommand(RunClearResponseCommand, CanClearResponseCommandRun);
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
            OnResponsePropertiesChanged();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void OnResponsePropertiesChanged()
        {
            OnPropertyChanged(nameof(HaveResponseDate));
            OnPropertyChanged(nameof(ResponseDate));
            OnPropertyChanged(nameof(Header));
            OnPropertyChanged(nameof(ResponseType));
        }

        private void RunClearResponseCommand(object o)
        {
            ResponseDate = null;
        }

        private bool CanClearResponseCommandRun(object o)
        {
            return (Owner.SelectedRow != null && Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Response] != DBNull.Value);
        }
        #endregion
    }
}
