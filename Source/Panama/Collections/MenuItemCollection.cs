using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

namespace Restless.App.Panama.Collections
{
    /// <summary>
    /// Represents a bindable collection of menu items with convienence methods for adding items.
    /// </summary>
    public class MenuItemCollection : ObservableCollection<Control>
    {
        /// <summary>
        /// Adds a new menu item to the collection.
        /// </summary>
        /// <param name="header">The item header, that which is displayed in the UI</param>
        /// <param name="command">The command associated with this item.</param>
        /// <param name="imageResource">The name of the image resource to use on this item, or null if none.</param>
        /// <param name="tag">An arbitrary object to attach to the menu item.</param>
        public void AddItem(string header, ICommand command, string imageResource = null, object tag = null)
        {
            var item = new MenuItem();
            item.Header = header;
            item.Command = command;
            item.Tag = tag;
            item.HorizontalContentAlignment = HorizontalAlignment.Left;
            item.VerticalContentAlignment = VerticalAlignment.Center;
            if (!String.IsNullOrEmpty(imageResource))
            {
                item.Icon = Application.Current.TryFindResource(imageResource);
            }
            Add(item);
        }

        /// <summary>
        /// Adds a menu separator to the collection.
        /// </summary>
        public void AddSeparator()
        {
            Add(new Separator());
        }
    }
}
