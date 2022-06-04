using System;

namespace Restless.Panama.ViewModel
{
    public abstract class TableBaseController<T> : DataViewModel<T> where T : class
    {
        protected new TableViewModel Owner
        {
            get;
        }

        protected TableBaseController(TableViewModel owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }
    }
}