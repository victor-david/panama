using Restless.Panama.Controls;
using Restless.Panama.ViewModel;
using Restless.Toolkit.Controls;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using IconKind = MahApps.Metro.IconPacks.PackIconMaterialKind;

namespace Restless.Panama.Core
{
    public class NavigatorItemCollection : ObservableCollection<Control>
    {
        #region Private
        private NavigatorHeader header;
        private readonly Thickness navigatorPadding;
        private readonly Thickness separatorMargin;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets or sets the navigator header style
        /// </summary>
        public NavigatorHeader Header
        {
            get => header;
            set => SetNavigatorHeader(value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigatorItemCollection"/> class
        /// </summary>
        /// <param name="header">The navigator header type</param>
        /// <param name="navIndent">The amount to indent a navigator</param>
        /// <param name="navVertical">The amount of vertical space (top and bottom) for a navigator</param>
        public NavigatorItemCollection(NavigatorHeader header, double navIndent, double navVertical)
        {
            Header = header;
            navigatorPadding = new Thickness(navIndent, navVertical, 0, navVertical);
            separatorMargin = new Thickness(0, 8, 5, 5);
        }
        #endregion

        /************************************************************************/

        #region Public methods

        public void AddNavigator<T>(string title, IconKind iconKind) where T : ApplicationViewModel
        {
            NavigatorItem item = new(0, typeof(T), 0)
            {
                Title = title,
                Icon = Icons.Get(iconKind),
                Padding = navigatorPadding
            };

            Add(item);
        }

        public void AddHeader(string title)
        {
            Add(new Separator() { Margin = separatorMargin });
            Add(new TitledSeparator() { Title = title, Margin = separatorMargin });
            AdjustNavigatorHeader();
        }

        public void SetVisibility<T>(bool isVisible) where T : ApplicationViewModel
        {
            foreach (NavigatorItem item in this.OfType<NavigatorItem>())
            {
                if (item.TargetType == typeof(T))
                {
                    item.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }
        #endregion

        private void SetNavigatorHeader(NavigatorHeader value)
        {
            header = value;
            AdjustNavigatorHeader();
        }

        private void AdjustNavigatorHeader()
        {
            switch (header)
            {
                case NavigatorHeader.None:
                    SetControlVisibility<Separator>(false);
                    break;
                case NavigatorHeader.Simple:
                    SetControlVisibility<Separator>(true);
                    SetControlVisibility<TitledSeparator>(false);
                    break;
                case NavigatorHeader.Titled:
                    SetControlVisibility<Separator>(false);
                    SetControlVisibility<TitledSeparator>(true);
                    break;
                default:
                    SetControlVisibility<Separator>(false);
                    break;
            }
        }

        private void SetControlVisibility<T>(bool isVisible) where T : Control
        {
            foreach (Control control in this.OfType<T>())
            {
                control.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}