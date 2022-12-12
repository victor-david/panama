using Restless.Toolkit.Core.Database.SQLite;
using System.ComponentModel;
using System.Data;

namespace Restless.Panama.Database.Tables
{
    public abstract class DateRowObject<T> : RowObjectBase<T>, INotifyPropertyChanged where T: TableBase
    {
        #region Properties
        /// <summary>
        /// Gets the date format.
        /// </summary>
        protected string DateFormat
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DateRowObject{T}"/> class.
        /// </summary>
        /// <param name="row"></param>
        public DateRowObject(DataRow row) : base(row)
        {
            DateFormat = "MMM dd, yyyy";
        }
        #endregion

        /************************************************************************/

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Invokes property changed for the specified property name
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        protected void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Sets the date format.
        /// </summary>
        /// <param name="value">The format value</param>
        public void SetDateFormat(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                DateFormat = value;
            }
        }
        #endregion
    }
}