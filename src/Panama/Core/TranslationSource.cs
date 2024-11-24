using Restless.Panama.Resources;
using Restless.Toolkit.Core;
using Restless.Toolkit.Mvvm;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Windows.Data;

namespace Restless.Panama.Core
{
    public class TranslationSource : ObservableObject, ITranslator
    {
        #region Private
        private readonly Dictionary<string, ResourceManager> resourceManagers;
        private CultureInfo currentCulture = null;
        #endregion

        /************************************************************************/

        #region Constructor
        public static TranslationSource Instance { get; } = new TranslationSource();
        private TranslationSource()
        {
            resourceManagers = new Dictionary<string, ResourceManager>()
            {
                { nameof(Confirm), Confirm.ResourceManager },
                { nameof(Data), Data.ResourceManager },
                { nameof(Detail), Detail.ResourceManager },
                { nameof(Error), Error.ResourceManager },
                { nameof(Header), Header.ResourceManager },
                { nameof(Language), Language.ResourceManager },
                { nameof(Menu), Menu.ResourceManager },
                { nameof(Settings), Settings.ResourceManager },
                { nameof(Text), Text.ResourceManager },
                { nameof(ToolTip), ToolTip.ResourceManager }
            };
        }
        #endregion

        /************************************************************************/

        #region Public methods
        public string this[string key] => GetString(key);

        public CultureInfo CurrentCulture
        {
            get => currentCulture;
            set => SetProperty(ref currentCulture, value);
        }

        public string GetString(string key)
        {
            string place = "None";
            string[] parts = key.Split('.');
            if (parts.Length > 1)
            {
                place = parts[0];
                key = parts[1];
            }

            return GetString(place, key);
        }

        public string GetString(string place, string key)
        {
            if (resourceManagers.ContainsKey(place))
            {
                string str = resourceManagers[place].GetString(key, currentCulture);
                if (!string.IsNullOrEmpty(str))
                {
                    return str;
                }
            }

            return $"[{place}.{key}]";
        }
        #endregion
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