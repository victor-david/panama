using System.ComponentModel;

namespace Restless.Panama.Core
{
    public class LanguageItem : INotifyPropertyChanged
    {
        private readonly string displayName;

        // private readonly LanguageItemCollection alternates;

        public string Id { get; }
        // public string DisplayName => GetDisplayName();

        public string DisplayName
        {
            get;
        }


        public LanguageItem DisplayLanguage { get; private set; }

        public LanguageItem(string id, string displayName) //, params LanguageItem[] items)
        {
            Id = id;
            DisplayName = displayName;
            // this.displayName = displayName;
            // alternates = new LanguageItemCollection();
            // alternates.AddRange(items);
        }

        //public void SetDisplayLanguage(LanguageItem item)
        //{
        //    DisplayLanguage = item;
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        //private string GetDisplayName()
        //{
        //    if (DisplayLanguage == null || DisplayLanguage.Id == Id)
        //    {
        //        return displayName;
        //    }

        //    if (alternates.GetLanguageItem(DisplayLanguage.Id) is LanguageItem alt)
        //    {
        //        return alt.displayName;
        //    }
        //    return displayName;
        //}
    }
}