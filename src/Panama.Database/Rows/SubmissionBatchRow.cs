using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.SubmissionBatchTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="SubmissionBatchTable"/>
    /// </summary>
    public class SubmissionBatchRow : RowObjectBase<SubmissionBatchTable>
    {
        #region Properties
        /// <summary>
        /// Gets the record id.
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets the publisher id.
        /// </summary>
        public long PublisherId
        { 
            get => GetInt64(Columns.PublisherId);
            private set => SetValue(Columns.PublisherId, value);
        }

        /// <summary>
        /// Gets or sets the submission fee
        /// </summary>
        public decimal Fee
        {
            get => GetDecimal(Columns.Fee);
            set => SetValue(Columns.Fee, value);
        }

        /// <summary>
        /// Gets or sets the submission award
        /// </summary>
        public decimal Award
        {
            get => GetDecimal(Columns.Award);
            set => SetValue(Columns.Award, value);
        }

        /// <summary>
        /// Gets or sets whether an online submission
        /// </summary>
        public bool IsOnline
        {
            get => GetBoolean(Columns.Online);
            set => SetValue(Columns.Online, value);
        }

        /// <summary>
        /// Gets or sets whether a contest submission
        /// </summary>
        public bool IsContest
        {
            get => GetBoolean(Columns.Contest);
            set => SetValue(Columns.Contest, value);
        }

        /// <summary>
        /// Gets or sets whether submission is locked
        /// </summary>
        public bool IsLocked
        {
            get => GetBoolean(Columns.Locked);
            set => SetValue(Columns.Locked, value);
        }

        /// <summary>
        /// Gets or sets the submission date
        /// </summary>
        public DateTime Submitted
        {
            get => GetDateTime(Columns.Submitted);
            set => SetValue(Columns.Submitted, value);
        }

        /// <summary>
        /// Gets or sets the response date
        /// </summary>
        public DateTime? Response
        {
            get => GetNullableDateTime(Columns.Response);
            set => SetValue(Columns.Response, value);
        }

        /// <summary>
        /// Gets or sets the response type
        /// </summary>
        public long ResponseType
        {
            get => GetInt64(Columns.ResponseType);
            set => SetValue(Columns.ResponseType, value);
        }

        /// <summary>
        /// Gets or sets submission notes
        /// </summary>
        public string Notes
        {
            get => GetString(Columns.Notes);
            set => SetValue(Columns.Notes, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionBatchRow"/> class
        /// </summary>
        /// <param name="row">The data row</param>
        private SubmissionBatchRow(DataRow row) : base(row)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Creates a new <see cref="SubmissionBatchRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new tag row, or null.</returns>
        public static SubmissionBatchRow Create(DataRow row)
        {
            return row != null ? new SubmissionBatchRow(row) : null;
        }

        /// <summary>
        /// Creates a new <see cref="SubmissionBatchRow"/> object for a new submission
        /// </summary>
        /// <param name="newRow">The new row</param>
        /// <param name="publisherId">The published id</param>
        /// <returns></returns>
        internal static SubmissionBatchRow Create(DataRow newRow, long publisherId)
        {
            return new SubmissionBatchRow(newRow)
            {
                PublisherId = publisherId,
                Fee = 0,
                Award = 0,
                IsOnline = false,
                IsContest = false,
                IsLocked = false,
                Submitted = DateTime.UtcNow,
                Response = null,
                ResponseType = ResponseTable.Defs.Values.NoResponse,
                Notes = null
            };
        }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return $"{nameof(SubmissionBatchRow)} Id: {Id} Publisher: {PublisherId}";
        }
        #endregion
    }
}