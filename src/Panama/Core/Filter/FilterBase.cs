/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Mvvm;
using System;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents the base class for filters. This class must be inherited.
    /// </summary>
    public abstract class FilterBase : ObservableObject
    {
        #region Private
        private string text;
        private long? id;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a boolean value that indicates if any filter is active.
        /// </summary>
        /// <remarks>
        /// When you override this property in a derived class, always call the base implementation also
        /// to check the base filter properties.
        /// </remarks>
        public virtual bool IsAnyFilterActive
        {
            get
            {
                return !string.IsNullOrEmpty(Text) || Id != null;
            }
        }

        /// <summary>
        /// Gets or sets the filter's text option.
        /// How this is applied depends upon the class that extends <see cref="FilterBase"/>.
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                if (SetProperty(ref text, value))
                {
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter's id option.
        /// How this is applied depends upon the class that extends <see cref="FilterBase"/>.
        /// </summary>
        public long? Id
        {
            get { return id; }
            set
            {
                if (SetProperty(ref id, value))
                {
                    OnChanged();
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Gets or sets a boolean value that determines if the <see cref="Changed"/>
        /// event is suspended. A derived class can temporarily suspend the firing
        /// of the event while making multiple changes to filter properties, and then
        /// re-enable the event when all changes are completed.
        /// </summary>
        protected bool IsChangedEventSuspended
        {
            get;
            set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        #pragma warning disable 1591
        public FilterBase()
        {
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Public events
        /// <summary>
        /// Occurs when a filter property changes.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to receive notifications when some aspect of the filter has changed.
        /// </remarks>
        public event EventHandler Changed;
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Causes the filter to update by raising the <see cref="Changed"/> event.
        /// </summary>
        public void Update()
        {
            OnChanged();
        }

        /// <summary>
        /// Resets the base filter properties to their default values.
        /// If you override this method in a derived class, always call the the base method.
        /// </summary>
        public virtual void Reset()
        {
            Text = null;
            Id = null;
            OnChanged();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Raises the <see cref="Changed"/> event.
        /// </summary>
        /// <remarks>
        /// This method raises the <see cref="Changed"/> event unless <see cref="IsChangedEventSuspended"/>
        /// is set to true. When overriding in a derived class, always call the base implementation
        /// to signal the property change on the <see cref="IsAnyFilterActive"/> property. The property
        /// change will not be signaled if <see cref="IsChangedEventSuspended"/> is true.
        /// </remarks>
        protected virtual void OnChanged()
        {
            if (!IsChangedEventSuspended)
            {
                Changed?.Invoke(this, EventArgs.Empty);
                OnPropertyChanged(nameof(IsAnyFilterActive));
            }
        }
        #endregion
    }
}