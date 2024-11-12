using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.SubmissionBatchTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="SubmissionBatchTable"/>
    /// </summary>
    public class SubmissionBatchRow : DateRowObject<SubmissionBatchTable>
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
        /// Gets or sets the submission date.
        /// When set, invokes property changed on <see cref="SubmittedFormatted"/>.
        /// </summary>
        public DateTime Submitted
        {
            get => GetDateTime(Columns.Submitted);
            set => SetDateValue(Columns.Submitted, value, nameof(SubmittedFormatted));
        }

        /// <summary>
        /// Gets or sets the response date.
        /// When set, invokes property changed on <see cref="ResponseFormatted"/>.
        /// </summary>
        public DateTime? Response
        {
            get => GetNullableDateTime(Columns.Response);
            set => SetDateValue(Columns.Response, value, nameof(ResponseFormatted));
        }

        /// <summary>
        /// Gets or sets the response type
        /// </summary>
        public long ResponseType
        {
            get => GetInt64(Columns.ResponseType);
            private set => SetValue(Columns.ResponseType, value);
        }

        /// <summary>
        /// Gets or sets submission notes
        /// </summary>
        public string Notes
        {
            get => GetString(Columns.Notes);
            set => SetValue(Columns.Notes, value);
        }

        /// <summary>
        /// Gets a boolean value that indicates if <see cref="Response"/> has a value
        /// </summary>
        public bool HasResponseDate => Response != null;

        /// <summary>
        /// Gets a formatted value for <see cref="Submitted"/>.
        /// </summary>
        public string SubmittedFormatted => GetFormattedDate(Submitted);

        /// <summary>
        /// Gets a formatted value for <see cref="Response"/>.
        /// </summary>
        public string ResponseFormatted => GetFormattedDate(Response);

        /// <summary>
        /// Gets the response type descriptive name
        /// </summary>
        public string ResponseTypeName => GetString(Columns.Joined.ResponseTypeName);

        /// <summary>
        /// Gets the publisher name
        /// </summary>
        public string PublisherName => GetString(Columns.Joined.Publisher);

        /// <summary>
        /// Gets the publisher url
        /// </summary>
        public string PublisherUrl => GetString(Columns.Joined.PublisherUrl);

        /// <summary>
        /// Gets a boolean value that indicates whether the publisher has a url
        /// </summary>
        public bool HasPublisherUrl => !string.IsNullOrWhiteSpace(PublisherUrl);
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

        /// <summary>
        /// Creates a new <see cref="SubmissionBatchRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new row, or null.</returns>
        public static SubmissionBatchRow Create(DataRow row)
        {
            return row != null ? new SubmissionBatchRow(row) : null;
        }
        #endregion

        /************************************************************************/

        #region Methods
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
                Submitted = Utility.GetNowZero(),
                Response = null,
                ResponseType = ResponseTable.Defs.Values.NoResponse,
                Notes = null
            };
        }

        /// <summary>
        /// Sets <see cref="ResponseType"/> to the specified value.
        /// </summary>
        /// <param name="value">The value to set</param>
        public void SetResponseType(long value)
        {
            ResponseType = value;
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