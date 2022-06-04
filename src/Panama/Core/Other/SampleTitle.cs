using System;
using System.Globalization;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Helper class to display sample titles
    /// </summary>
    public class SampleTitle
    {
        /// <summary>
        /// Gets the id of the sample title.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the date written.
        /// </summary>
        public string Written { get; }

        /// <summary>
        /// Gets a value that indicates if the title is published.
        /// </summary>
        public bool CalcIsPublished { get; }

        /// <summary>
        /// Gets a value that indicates if the title is published.
        /// </summary>
        public bool CalcIsSelfPublished { get; }

        /// <summary>
        /// Gets a value that indicates if the title is submitted.
        /// </summary>
        public bool CalcIsSubmitted { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleTitle"/> class.
        /// </summary>
        /// <param name="id">The sample id.</param>
        /// <param name="title">The title.</param>
        /// <param name="written">Number of days from today that the title was written (usually a negative number)</param>
        /// <param name="isPublished">A boolean value that indicates if the title is published.</param>
        /// <param name="isSelfPublished">A boolean value that indicates if the title is self published.</param>
        /// <param name="isSubmitted">A boolean value that indicates if the title is submitted.</param>
        public SampleTitle(long id, string title, int written, bool isPublished, bool isSelfPublished, bool isSubmitted)
        {
            Id = id;
            Title = title;
            Written = DateTime.Now.AddDays(written).ToString(Config.Instance.DateFormat, CultureInfo.InvariantCulture);
            CalcIsPublished = isPublished;
            CalcIsSelfPublished = isSelfPublished;
            CalcIsSubmitted = isSubmitted;
        }
    }
}