using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace Restless.App.Panama
{
    /// <summary>
    /// Represents the base class from which other classes that need to implement INotifyPropertyChanged can derive.
    /// This class must be inherited.
    /// </summary>
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        #region Constructor
        #pragma warning disable 1591
        protected NotifyPropertyChangedBase()
        {
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/
        
        #region INotifyPropertyChanged Members
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion

        /************************************************************************/

        #region Debugging Aides
        /// <summary>
        /// Warns the developer if this object does not have a public property with the specified name
        /// by emitting a message in the debug output window. This method does not exist in a Release build.
        /// </summary>
        /// <param name="propertyName">The name of the property to verify</param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = string.Format("Invalid property name: {0}", propertyName);

                if (ThrowOnInvalidPropertyName)
                {
                    throw new Exception(msg);
                }
                else
                {
                    Debug.WriteLine(msg);
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether an exception is thrown, or if a Debug.WriteLine() is used
        /// when an invalid property name is passed to the <see cref="VerifyPropertyName"/> method. The default value is false,
        /// but derived classes used by unit tests might override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName
        {
            get;
            private set;
        }

        #endregion

        /************************************************************************/
    }
}
