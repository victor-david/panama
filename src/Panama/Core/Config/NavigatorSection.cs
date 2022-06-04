namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides a simple class used to identify a navigator section
    /// </summary>
    public class NavigatorSection
    {
        /// <summary>
        /// Gets the name of the section
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Gets the section id
        /// </summary>
        public long Id
        {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatorSection"/> class
        /// </summary>
        /// <param name="name">Name of the section</param>
        /// <param name="id">Section id</param>
        public NavigatorSection(string name, long id)
        {
            Name = name;
            Id = id;
        }
    }
}