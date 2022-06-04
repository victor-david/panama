using System;
using System.Globalization;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Helper class to display sample publishers
    /// </summary>
    public class SamplePublisher
    {
        /// <summary>
        /// Gets the id of the sample publisher.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Gets the name of the publisher.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the date this publisher was added.
        /// </summary>
        public string Added { get; }

        /// <summary>
        /// Get the last submission date to this publisher.
        /// </summary>
        public string LastSub { get; }

        /// <summary>
        /// Gets a value that indicates if the publisher is within their submission period.
        /// </summary>
        public bool CalcInPeriod { get; }

        /// <summary>
        /// Gets a value that indicates if the publisher has been flagged as a goner.
        /// </summary>
        public bool CalcGoner { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SamplePublisher"/> class.
        /// </summary>
        /// <param name="id">The sample id.</param>
        /// <param name="name">The name of the publisher.</param>
        /// <param name="added">Number of days from today when the publisher was added (usually a negative number).</param>
        /// <param name="lastSub">Number of days from today for the last submission (usually a negative number).</param>
        /// <param name="isInPeriod">A boolean value that indicates if the publisher is within their submission period.</param>
        /// <param name="isGoner">A boolean value that indicates if the publisher has been flagged as a goner.</param>
        public SamplePublisher(long id, string name, int added, int lastSub, bool isInPeriod, bool isGoner)
        {
            Id = id;
            Name = name;
            Added = DateTime.Now.AddDays(added).ToString(Config.Instance.DateFormat, CultureInfo.InvariantCulture);
            LastSub = DateTime.Now.AddDays(lastSub).ToString(Config.Instance.DateFormat, CultureInfo.InvariantCulture);
            CalcInPeriod = isInPeriod;
            CalcGoner = isGoner;
        }
    }
}