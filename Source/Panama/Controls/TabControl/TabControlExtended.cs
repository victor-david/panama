using System;
using System.Collections.Generic;
using System.Collections.Specialized;
//using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Restless.Tools.Controls.DragDrop;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Extends TabControl to prevent content unloading during tab switch and to provide tab reordering via drag and drop.
    /// </summary>
    /// <remarks>
    /// See:
    /// http://stackoverflow.com/questions/9794151/stop-tabcontrol-from-recreating-its-children
    /// </remarks>
    [TemplatePart(Name = "PART_ItemsHolder", Type = typeof(Panel))]
    public class TabControlExtended : TabControl
    {
        #region Private
        private Panel ItemsHolderPanel = null;
        private const string Part = "PART_ItemsHolder";
        private Window dragCursor;
        private Point startPoint;
        private bool dragging = false;
        #endregion

        /************************************************************************/
        
        #region Public properties
        /// <summary>
        /// Gets the tab item torn from the control.
        /// This property is set when a tab has been torn from the control.
        /// </summary>
        public TabItem TornItem
        {
            get { return (TabItem)GetValue(TornItemProperty); }
            set { SetValue(TornItemProperty, value); }
        }

        /// <summary>
        /// Defines a dependency property that describes the <see cref="TornItem"/> property.
        /// </summary>
        public static readonly DependencyProperty TornItemProperty = DependencyProperty.Register
            (
                "TornItem", typeof(TabItem), typeof(TabControlExtended), new FrameworkPropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets a value that indicates if the tabs of this control can be reordered using drag and drop.
        /// </summary>
        public bool AllowTabReorder
        {
            get { return (bool)GetValue(AllowTabReorderProperty); }
            set { SetValue(AllowTabReorderProperty, value); }
        }

        /// <summary>
        /// Defines a dependency property that describes the <see cref="AllowTabReorder"/> property.
        /// </summary>
        public static readonly DependencyProperty AllowTabReorderProperty = DependencyProperty.Register
            (
                "AllowTabReorder", typeof(bool), typeof(TabControlExtended), new UIPropertyMetadata(false)
            );

        /// <summary>
        /// Gets or sets a command to be executed after a drag / drop operation to perform the reordering.
        /// If this property is not set, the tabs can be reordered internally if the ItemsSource is bound
        /// to an ObservableCollection.
        /// </summary>
        public ICommand ReorderTabsCommand
        {
            get { return (ICommand)GetValue(ReorderTabsCommandProperty); }
            set { SetValue(ReorderTabsCommandProperty, value); }
        }

        /// <summary>
        /// Defines a dependency property that describes the <see cref="ReorderTabsCommand"/> property.
        /// </summary>
        public static readonly DependencyProperty ReorderTabsCommandProperty = DependencyProperty.Register
            (
                "ReorderTabsCommand", typeof(ICommand), typeof(TabControlExtended), new UIPropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets a value that indicates if the tabs of this control can be torn off into a new window using drag and drop.
        /// </summary>
        public bool AllowTabTear
        {
            get { return (bool)GetValue(AllowTabTearProperty); }
            set { SetValue(AllowTabTearProperty, value); }
        }

        /// <summary>
        /// Defines a  dependency property that describes the <see cref="AllowTabTear"/> property.
        /// </summary>
        public static readonly DependencyProperty AllowTabTearProperty = DependencyProperty.Register
            (
                "AllowTabTear", typeof(bool), typeof(TabControlExtended), new UIPropertyMetadata(false, AllowTabTearChanged)
            );
        #endregion

        /************************************************************************/

        #region Constructor
        #pragma warning disable 1591
        public TabControlExtended()
            : base()
        {
            // This is necessary so that we get the initial databound selected item
            ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Get the ItemsHolder and generate any children
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ItemsHolderPanel = GetTemplateChild(Part) as Panel;
            UpdateSelectedItem();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// When the items change we remove any generated panel children and add any new ones as necessary
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (ItemsHolderPanel == null)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    ItemsHolderPanel.Children.Clear();
                    break;

                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (AllowTabReorder)
                            {
                                RemoveEventHandlers(item);
                            }
                            ContentPresenter cp = FindChildContentPresenter(item);
                            if (cp != null)
                            {
                                ItemsHolderPanel.Children.Remove(cp);
                            }
                        }
                    }

                    // Don't do anything with new items because we don't want to
                    // create visuals that aren't being shown
                    if (AllowTabReorder && e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            AttachEventHandlers(item);
                        }
                    }
                    UpdateSelectedItem();
                    break;

                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException("Replace not implemented yet");
            }
        }

        /// <summary>
        /// Called when the selection on the control changes.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            UpdateSelectedItem();
        }

        /// <summary>
        /// Called when an item has been dropped on the control.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnDrop(DragEventArgs e)
        {
            var tabItemSource = e.Data.GetData(typeof(TabItem)) as TabItem;
            if (tabItemSource != null)
            {
                TornItem = tabItemSource;
                e.Handled = true;
            }
            base.OnDrop(e);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        /// <summary>
        /// If containers are done, generate the selected item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                ItemContainerGenerator.StatusChanged -= ItemContainerGenerator_StatusChanged;
                UpdateSelectedItem();
            }
        }

        private void UpdateSelectedItem()
        {
            if (ItemsHolderPanel == null)
            {
                return;
            }

            // Generate a ContentPresenter if necessary
            TabItem item = GetSelectedTabItem();
            if (item != null)
            {
                CreateChildContentPresenter(item);
            }

            // show the right child
            foreach (ContentPresenter child in ItemsHolderPanel.Children)
            {
                child.Visibility = ((child.Tag as TabItem).IsSelected) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private ContentPresenter CreateChildContentPresenter(object item)
        {
            if (item == null)
            {
                return null;
            }

            ContentPresenter cp = FindChildContentPresenter(item);

            if (cp != null)
            {
                return cp;
            }

            // the actual child to be added.  cp.Tag is a reference to the TabItem
            cp = new ContentPresenter();
            cp.Content = (item is TabItem) ? (item as TabItem).Content : item;
            cp.ContentTemplate = SelectedContentTemplate;
            cp.ContentTemplateSelector = SelectedContentTemplateSelector;
            cp.ContentStringFormat = SelectedContentStringFormat;
            cp.Visibility = Visibility.Collapsed;
            cp.Tag = (item is TabItem) ? item : (ItemContainerGenerator.ContainerFromItem(item));
            ItemsHolderPanel.Children.Add(cp);
            return cp;
        }

        private ContentPresenter FindChildContentPresenter(object data)
        {
            if (data is TabItem)
            {
                data = (data as TabItem).Content;
            }

            if (data == null || ItemsHolderPanel == null)
            {
                return null;
            }

            foreach (ContentPresenter cp in ItemsHolderPanel.Children)
            {
                if (cp.Content == data)
                {
                    return cp;
                }
            }

            return null;
        }

        private TabItem GetSelectedTabItem()
        {
            object selectedItem = base.SelectedItem;
            if (selectedItem == null)
            {
                return null;
            }

            TabItem item = selectedItem as TabItem;
            if (item == null)
            {
                item = base.ItemContainerGenerator.ContainerFromIndex(base.SelectedIndex) as TabItem;
            }

            return item;
        }

        private TabItem GetTabItem(object item)
        {
            TabItem tabItem = item as TabItem;
            if (tabItem == null)
            {
                tabItem = base.ItemContainerGenerator.ContainerFromItem(item) as TabItem;
            }

            return tabItem;
        }
        #endregion

        /************************************************************************/

        #region Private methods (drag and drop support)
        private void AttachEventHandlers(object item)
        {
            UIElement element = item as UIElement;
            TabItem tabItem = GetTabItem(item);
            if (tabItem != null)
            {
                tabItem.AllowDrop = true;
                tabItem.PreviewMouseLeftButtonDown += TabItemPreviewMouseLeftButtonDown;
                tabItem.PreviewMouseMove += TabItemPreviewMouseMove;
                tabItem.GiveFeedback += TabItemGiveFeedback;
                tabItem.Drop += TabItemDrop;
            }
        }

        private void RemoveEventHandlers(object item)
        {
            TabItem tabItem = GetTabItem(item);
            if (tabItem != null)
            {
                tabItem.PreviewMouseLeftButtonDown -= TabItemPreviewMouseLeftButtonDown;
                tabItem.PreviewMouseMove -= TabItemPreviewMouseMove;
                tabItem.GiveFeedback -= TabItemGiveFeedback;
                tabItem.Drop -= TabItemDrop;
            }
        }

        private void TabItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void TabItemPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !dragging)
            {
                Point pos = e.GetPosition(null);

                if (Math.Abs(pos.X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(pos.Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    var tabItem = e.Source as FrameworkElement;
                    tabItem.AllowDrop = false;
                    dragCursor = CreateDragCursor(tabItem);
                    dragCursor.Show();
                    dragging = true;
                    DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.Move);
                    tabItem.AllowDrop = true;
                    dragging = false;
                    dragCursor.Close();
                    dragCursor = null;
                }
            }            
        }

        private void TabItemGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            dragCursor.Left = w32Mouse.X + 8;
            dragCursor.Top = w32Mouse.Y - (dragCursor.ActualHeight / 2);
            dragCursor.Opacity = (e.Effects == DragDropEffects.Move) ? 1.0 : 0.35;
            e.Handled = true;
        }

        private void TabItemDrop(object sender, DragEventArgs e)
        {
            var tabItemTarget = e.Source as TabItem;

            var tabItemSource = e.Data.GetData(typeof(TabItem)) as TabItem;

            if (!tabItemTarget.Equals(tabItemSource))
            {
                if (ReorderTabsCommand != null)
                {
                    ReorderTabsCommand.Execute(new TabItemDragDrop(tabItemSource, tabItemTarget));
                }
                else
                {
                    var sourceType = ItemsSource.GetType();
                    if (sourceType.IsGenericType)
                    {
                        var sourceDefinition = sourceType.GetGenericTypeDefinition();

                        if (sourceDefinition == typeof(ObservableCollection<>))
                        {

                            int sourceIdx = base.ItemContainerGenerator.IndexFromContainer(tabItemSource);
                            int targetIdx = base.ItemContainerGenerator.IndexFromContainer(tabItemTarget);

                            //int sourceIdx = Workspaces.IndexOf(source);
                            //int targetIdx = Workspaces.IndexOf(target);

                            var method = sourceType.GetMethod("Move");
                            method.Invoke(ItemsSource, new object[] { sourceIdx, targetIdx });
                        }
                    }
                }

                //DragDropItems = new TabItemDragDrop(tabItemSource, tabItemTarget);
            }
            e.Handled = true;
        }

        private Window CreateDragCursor(FrameworkElement dragElement)
        {
            dragCursor = new Window()
            {
                Background = new SolidColorBrush(Colors.DarkGreen),
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Topmost = true,
                ShowInTaskbar = false,
                SizeToContent = SizeToContent.WidthAndHeight
            };

            Rectangle rect = new Rectangle();
            rect.Width = dragElement.ActualWidth;
            rect.Height = dragElement.ActualHeight;
            rect.Fill = new VisualBrush(dragElement);

            dragCursor.Content = rect;
            return dragCursor;
        }
        #endregion

        /************************************************************************/

        #region Externals (drag and drop support)
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public int X;
            public int Y;
        };
        #endregion

        /************************************************************************/

        #region Private methods (static)
        private static void AllowTabTearChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabControlExtended t = d as TabControlExtended;
            if (t != null)
            {
                t.AllowDrop = (bool)e.NewValue;
            }
        }
        #endregion


    }
}
