using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;

namespace Restless.Panama.Database.Tables
{
    public abstract class DateRowObject<T> : RowObjectBase<T>, INotifyPropertyChanged where T: TableBase
    {
        #region Private
        private string dateFormat;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DateRowObject{T}"/> class.
        /// </summary>
        /// <param name="row"></param>
        public DateRowObject(DataRow row) : base(row)
        {
            dateFormat = "MMM dd, yyyy";
        }
        #endregion

        /************************************************************************/

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Gets a formatted date
        /// </summary>
        /// <param name="date">A date time</param>
        /// <returns></returns>
        protected string GetFormattedDate(DateTime date) => date.ToString(dateFormat);

        /// <summary>
        /// Gets a formatted date
        /// </summary>
        /// <param name="date">A nullable date time</param>
        /// <returns></returns>
        protected string GetFormattedDate(DateTime? date) => date?.ToString(dateFormat) ?? "--";

        /// <summary>
        /// Sets the specified date column to the specified value and optionaly invokes property changed.
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="value"></param>
        /// <param name="properyName">An optional property name</param>
        protected void SetDateValue(string colName, DateTime value, string properyName = null)
        {
            if (SetValue(colName, value) && !string.IsNullOrEmpty(properyName))
            {
                InvokePropertyChanged(properyName);
            }
        }

        /// <summary>
        /// Sets the specified date nullable column to the specified value and optionaly invokes property changed.
        /// </summary>
        /// <param name="colName"></param>
        /// <param name="value"></param>
        /// <param name="properyName">An optional property name</param>
        protected void SetDateValue(string colName, DateTime? value, string properyName = null)
        {
            if (SetValue(colName, value) && !string.IsNullOrEmpty(properyName))
            {
                InvokePropertyChanged(properyName);
            }
        }

        /// <summary>
        /// Invokes property changed for the specified property name
        /// </summary>
        /// <param name="propertyName">The name of the property</param>
        protected void InvokePropertyChanged([CallerMemberName] string propertyName = null)
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
                dateFormat = value;
            }
        }
        #endregion
    }
}