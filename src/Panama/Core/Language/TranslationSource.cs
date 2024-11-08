using Restless.Panama.Resources;
using Restless.Toolkit.Mvvm;
using System.Globalization;
using System.Resources;
using System.Windows.Data;

namespace Restless.Panama.Core
{
    public class TranslationSource : ObservableObject
    {
        #region Private
        private readonly ResourceManager resManager = Menu.ResourceManager;
        private CultureInfo currentCulture = null;
        #endregion

        /************************************************************************/

        #region Constructor
        public static TranslationSource Instance { get; } = new TranslationSource();
        private TranslationSource()
        {
        }
        #endregion

        public string this[string key]
        {
            get
            {
                string res = resManager?.GetString(key, currentCulture);
                return !string.IsNullOrWhiteSpace(res) ? res : $"[{key}]";
            }
        }

        public CultureInfo CurrentCulture
        {
            get => currentCulture;
            set => SetProperty(ref currentCulture, value);
        }
    }

    public class LocExtension : Binding
    {
        public LocExtension(string name) : base("[" + name + "]")
        {
            Mode = BindingMode.OneWay;
            Source = TranslationSource.Instance;
        }
    }
}