using Restless.Panama.Controls;
using Restless.Panama.ViewModel;
using Restless.Toolkit.Controls;
using System;
using System.Collections.ObjectModel;
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
        private const int StatusCollapsed = 10;
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

        public void AddHeader(string title, int id = 0)
        {
            Add(new TitledSeparator() { Title = title, Margin = separatorMargin, Id = id });
            AdjustNavigatorHeader();
        }

        public void SetHeaderVisibility(int headerId, bool isVisible)
        {
            ForEachSeparator(item =>
            {
                if (item.Id == headerId)
                {
                    item.Status = isVisible ? 0 : StatusCollapsed;
                    if (Header != NavigatorHeader.None)
                    {
                        SetItemVisibility(item, isVisible);
                    }
                }
            });

        }

        public void SetVisibility<T>(bool isVisible) where T : ApplicationViewModel
        {
            foreach (NavigatorItem item in this.OfType<NavigatorItem>())
            {
                if (item.TargetType == typeof(T))
                {
                    SetItemVisibility(item, isVisible);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
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
                    AdjustNavigatorHeader(false, false);
                    break;
                case NavigatorHeader.Simple:
                    AdjustNavigatorHeader(true, false);
                    break;
                case NavigatorHeader.Titled:
                    AdjustNavigatorHeader(true, true);
                    break;
                default:
                    AdjustNavigatorHeader(false, false);
                    break;
            }
        }

        private void AdjustNavigatorHeader(bool isVisible, bool isTitleVisible)
        {
            ForEachSeparator(item =>
            {
                bool isVisibleOverride = isVisible;
                if (isVisible && item.Status == StatusCollapsed)
                {
                    isVisibleOverride = false;
                }

                SetItemVisibility(item, isVisibleOverride);
                item.DisplayTitle = isTitleVisible;
            });
        }

        private void ForEachSeparator(Action<TitledSeparator> callback)
        {
            foreach (TitledSeparator item in this.OfType<TitledSeparator>())
            {
                callback(item);
            }
        }

        private void SetItemVisibility(Control item, bool isVisible)
        {
            item.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion
    }
}