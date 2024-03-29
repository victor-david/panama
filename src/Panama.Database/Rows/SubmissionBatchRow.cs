﻿using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using System.Globalization;
using Columns = Restless.Panama.Database.Tables.SubmissionBatchTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="SubmissionBatchTable"/>
    /// </summary>
    public class SubmissionBatchRow : RowObjectBase<SubmissionBatchTable>
    {
        #region Private
        private string dateFormat;
        #endregion

        /************************************************************************/

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
        /// Gets a formatted value for <see cref="Submitted"/> converted to local time.
        /// </summary>
        public string SubmittedLocal => Submitted.ToLocalTime().ToString(dateFormat, CultureInfo.InvariantCulture);

        /// <summary>
        /// Gets a formatted value for <see cref="Response"/> converted to local time.
        /// </summary>
        public string ResponseLocal => Response?.ToLocalTime().ToString(dateFormat, CultureInfo.InvariantCulture) ?? "--";

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
            dateFormat = "MMM dd, yyyy";
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
                Submitted = Utility.GetUtcNowZero(),
                Response = null,
                ResponseType = ResponseTable.Defs.Values.NoResponse,
                Notes = null
            };
        }

        /// <summary>
        /// Sets the date format used for <see cref="DateLocal"/>
        /// </summary>
        /// <param name="value"></param>
        public void SetDateFormat(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                dateFormat = value;
            }
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