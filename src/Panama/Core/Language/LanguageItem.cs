using Restless.Panama.Resources;
using System.Globalization;

namespace Restless.Panama.Core
{
    public class LanguageItem
    {
        private readonly string resourceId;

        public string Id { get; }

        public string DisplayName
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageItem"/> class
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resourceId"></param>
        public LanguageItem(string id, string resourceId)
        {
            Id = id;
            this.resourceId = resourceId;
            DisplayName = Language.ResourceManager.GetString(resourceId);
        }

        /// <summary>
        /// Updates the display name of this item so it shows in the new language
        /// </summary>
        /// <param name="culture">The culture to use</param>
        public void UpdateDisplayName(CultureInfo culture)
        {
            DisplayName = Language.ResourceManager.GetString(resourceId, culture);
        }

        public override string ToString() => $"{DisplayName} [{Id}]";
    }
}