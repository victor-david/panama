namespace Restless.Panama.Controls
{
    /// <summary>
    /// Represents a month name/value selection
    /// </summary>
    public class Month
    {
        /// <summary>
        /// Gets the month name
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Gets the month value
        /// </summary>
        public long Value
        {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Month"/> class
        /// </summary>
        /// <param name="value">Month value</param>
        /// <param name="name">Month name</param>
        internal Month(long value, string name)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}