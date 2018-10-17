using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Restless.App.Panama
{
    /// <summary>
    /// Represents a general purpose helper class that can be used when binding.
    /// </summary>
    /// <remarks>
    /// This class provide various properties that may be used in a variety of ways
    /// for binding purposes.
    /// </remarks>
    public class GeneralOption
    {
        #region Private
        private bool isSelected;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets the name of the option.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a string value associated with this option.
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an integer value associated with this option.
        /// </summary>
        public long IntValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an arbitrary object value associated with this option.
        /// </summary>
        public object ObjectValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates if this option is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    IsSelectedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets a command associated with this general option.
        /// </summary>
        public ICommand Command
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a command option associated with this general option.
        /// </summary>
        public object CommandParm
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the item index.
        /// This is used by <see cref="GeneralOptionList"/> when adding an item via
        /// the <see cref="GeneralOptionList.AddObservable(GeneralOption)"/> method.
        /// </summary>
        public int Index
        {
            get;
            set;
        }
        #endregion

        /************************************************************************/

        #region Public events
        /// <summary>
        /// Occurs when the <see cref="IsSelected"/> property changes.
        /// </summary>
        public event EventHandler IsSelectedChanged;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralOption"/> class.
        /// </summary>
        public GeneralOption()
        {

        }
        #endregion
    }

    /// <summary>
    /// Represents a list of <see cref="GeneralOption"/> objects.
    /// </summary>
    public class GeneralOptionList : List<GeneralOption>
    {
        #region Public events
        /// <summary>
        /// Occurs when an item of the collection has changed its <see cref="GeneralOption.IsSelected"/> property.
        /// You must use the <see cref="AddObservable(GeneralOption)"/> method for each item added to the collection 
        /// to enable this event.
        /// </summary>
        public event EventHandler IsSelectedChanged;
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Adds an item to the collection and hooks up its <see cref="GeneralOption.IsSelectedChanged"/> event.
        /// </summary>
        /// <param name="item"></param>
        public void AddObservable(GeneralOption item)
        {
            Add(item);
            if (item != null)
            {
                item.Index = Count - 1;
                item.IsSelectedChanged += ItemIsSelectedChanged;
            }
        }
        #endregion

        /************************************************************************/

        #region Private events
        private void ItemIsSelectedChanged(object sender, EventArgs e)
        {
            IsSelectedChanged?.Invoke(sender, e);
        }
        #endregion
    }

    /// <summary>
    /// Represents an observable collection list of <see cref="GeneralOption"/> objects.
    /// </summary>
    public class GeneralOptionObservableCollection : ObservableCollection<GeneralOption>
    {
        /// <summary>
        /// Gets a boolean value that indicates the collection contains a <see cref="GeneralOption"/> object
        /// with its <see cref="GeneralOption.IntValue"/> property set to <paramref name="intValue"/>.
        /// </summary>
        /// <param name="intValue">The value to check.</param>
        /// <returns>true or false.</returns>
        public bool Contains(long intValue)
        {
            foreach (GeneralOption op in this)
            {
                if (op.IntValue == intValue)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
