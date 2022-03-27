using Restless.Toolkit.Core.Database.SQLite;
using Restless.Toolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.Panama.ViewModel
{
    public abstract class BaseController<T1, T2> : DataGridViewModel<T2> where T1 : ViewModelBase where T2 : TableBase
    {
        protected new T1 Owner
        {
            get;
        }

        /// <summary>
        /// Gets a header value that may be bound to a UI element.
        /// </summary>
        public virtual string Header1 => null;

        /// <summary>
        /// Gets a header value that may be bound to a UI element.
        /// </summary>
        public virtual string Header2 => null;

        protected BaseController(T1 owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }
    }
}
