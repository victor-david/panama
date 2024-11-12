using Restless.Panama.Resources;
using System.Globalization;
using System.Threading;
using System.Windows.Markup;

namespace Restless.Panama.Core
{
    public class LanguageManager
    {
        #region Private
        private CultureInfo culture;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the default language id, en-us
        /// </summary>
        public const string DefaultLanguageId = "en-us";

        /// <summary>
        /// Gets the current language id
        /// </summary>
        public string CurrentLanguageId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current Xml langauge.
        /// </summary>
        public XmlLanguage GetCurrentXmlLanguage() => XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);

        /// <summary>
        /// Gets the list of supported languages.
        /// </summary>
        public LanguageItemCollection Languages { get; }
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Gets the singleton instance of this class
        /// </summary>
        public static LanguageManager Instance { get; } = new LanguageManager();

        private LanguageManager()
        {
            Languages = new LanguageItemCollection()
            {
                new LanguageItem(DefaultLanguageId, nameof(Language.English)),
                new LanguageItem("es", nameof(Language.Spanish))
            };

            CurrentLanguageId = DefaultLanguageId;
            SetLanguage(DefaultLanguageId);
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Sets language to the specified id
        /// </summary>
        /// <param name="languageId">The id</param>
        public void SetLanguage(string languageId)
        {
            if (Languages.GetLanguageItem(languageId) is LanguageItem item)
            {
                CurrentLanguageId = languageId;

                culture = new CultureInfo(item.Id);

                Confirm.Culture = culture;
                Detail.Culture = culture;
                Error.Culture = culture;
                Header.Culture = culture;
                Language.Culture = culture;
                Menu.Culture = culture;
                Settings.Culture = culture;
                Text.Culture = culture;
                ToolTip.Culture = culture;

                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                TranslationSource.Instance.CurrentCulture = culture;

                Languages.ForEach(item => item.UpdateDisplayName(culture));
            }
        }
        #endregion
    }
}