using Restless.Toolkit.Mvvm;
using System;
using System.Data;
using System.Windows.Data;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents the base class for a data row filter. This class must be inherited.
    /// </summary>
    public abstract class RowFilter : ObservableObject
    {
        #region Private
        private long id;
        private string text;
        private int applyFilterSuspendLevel;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the count of currently filtered records
        /// </summary>
        public int RecordCount => ListView?.Count ?? 0;

        /// <summary>
        /// Gets the list view associated with the filter
        /// </summary>
        protected ListCollectionView ListView
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a boolean value that determines if filtering by id is supported
        /// </summary>
        protected virtual bool IsIdFilterSupported => false;

        /// <summary>
        /// Gets a boolean value that determines if filtering by text is supported
        /// </summary>
        protected virtual bool IsTextFilterSupported => false;

        /// <summary>
        /// Gets a boolean value that indicates if any filter is active.
        /// </summary>
        /// <remarks>
        /// When you override this property in a derived class, always call the base implementation also
        /// to check the base filter properties.
        /// </remarks>
        public virtual bool IsAnyFilterActive => id != -1 || !string.IsNullOrEmpty(Text);

        /// <summary>
        /// Gets or sets a text value.
        /// How this value is used depends on the class that extends <see cref="RowFilter"/>.
        /// </summary>
        public string Text
        {
            get => text;
            set
            {
                if (IsTextFilterSupported && SetProperty(ref text, value))
                {
                    ApplyFilter();
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="RowFilter"/> class
        /// </summary>
        protected RowFilter()
        {
            id = -1;
        }
        #endregion

        /************************************************************************/

        #region Methods
        /// <summary>
        /// Sets the <see cref="ListView"/> property
        /// </summary>
        /// <param name="listView"></param>
        public void SetListView(ListCollectionView listView)
        {
            ListView = listView ?? throw new ArgumentNullException(nameof(listView));
        }

        /// <summary>
        /// Sets the <see cref="Id"/> filter property and calls <see cref="OnSetIdFilter"/>
        /// </summary>
        /// <param name="id">The id</param>
        /// <remarks>
        /// The id property is used in certain circumstances to filter to a single record;
        /// it is not meant to be directly bound.
        /// </remarks>
        public void SetIdFilter(long id)
        {
            if (IsIdFilterSupported)
            {
                IncreaseSuspendLevel();
                ClearAll();
                DecreaseSuspendLevel();
                this.id = id;
                ApplyFilter();
            }
        }

        /// <summary>
        /// Gets the current value of the id filter
        /// </summary>
        /// <returns>The value of id</returns>
        public long GetIdFilter()
        {
            return id;
        }

        /// <summary>
        /// Applies the filter if not suspended
        /// </summary>
        public void ApplyFilter()
        {
            if (applyFilterSuspendLevel == 0)
            {
                ListView?.Refresh();
                OnPropertyChanged(nameof(RecordCount));
                OnPropertyChanged(nameof(IsAnyFilterActive));
            }
        }

        /// <summary>
        /// Clears all filters.
        /// Override in a derived class as needed, and always call the base method
        /// </summary>
        public virtual void ClearAll()
        {
            IncreaseSuspendLevel();
            id = -1;
            Text = null;
            DecreaseSuspendLevel();
        }

        /// <summary>
        /// Evaluates the specified data row
        /// </summary>
        /// <param name="item">The data row</param>
        /// <returns>true the data row passes the filter; otherwise, false</returns>
        public abstract bool OnDataRowFilter(DataRow item);

        /// <summary>
        /// Increases the <see cref="ApplyFilter"/> method suspension level
        /// </summary>
        protected void IncreaseSuspendLevel()
        {
            applyFilterSuspendLevel++;
        }

        /// <summary>
        /// Decreases the <see cref="ApplyFilter"/> method suspension level.
        /// If level reaches zero, the filter is applied.
        /// </summary>
        protected void DecreaseSuspendLevel()
        {
            applyFilterSuspendLevel--;
            ApplyFilter();
        }

        /// <summary>
        /// Sets state according to <paramref name="setter"/>
        /// within a suspended state of filter application
        /// </summary>
        /// <param name="setter">The callback setter</param>
        /// <remarks>
        /// Suspends filter application (i.e. refresh of the list view),
        /// clears all filters, calls the callback, and resumes from the suspension.
        /// </remarks>
        protected void SetCustomPropertyState(Action setter)
        {
            IncreaseSuspendLevel();
            ClearAll();
            setter?.Invoke();
            DecreaseSuspendLevel();
        }
        #endregion
    }
}