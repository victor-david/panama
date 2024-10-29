using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using System;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents a single title that is being submitted
    /// </summary>
    public class TitleSubmission
    {
        private readonly TitleRow titleRow;

        /// <summary>
        /// Gets the title id
        /// </summary>
        public long Id => titleRow.Id;

        /// <summary>
        /// Get the title
        /// </summary>
        public string Title => titleRow.Title;

        /// <summary>
        /// Gets the status
        /// </summary>
        public TitleSubmissionStatus Status
        {
            get;
        }

        public string StatusString => GetStatusString();

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleSubmission"/> class
        /// </summary>
        /// <param name="titleRow"></param>
        /// <param name="status"></param>
        public TitleSubmission(TitleRow titleRow, TitleSubmissionStatus status)
        {
            this.titleRow = titleRow ?? throw new ArgumentNullException(nameof(titleRow));
            Status = status;
        }

        private string GetStatusString()
        {
            return Status switch
            {
                TitleSubmissionStatus.Okay => Strings.TitleStatusOkay,
                TitleSubmissionStatus.Exclusive => Strings.TitleStatusSubmittedToExclusive,
                TitleSubmissionStatus.SamePublisher => Strings.TitleStatusPreviousToPublisher,
                _ => Strings.TitleStatusUnknown
            };
        }
    }
}