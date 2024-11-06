using Restless.Panama.Core;
using Restless.Toolkit.Core.Database.SQLite;
using Restless.Toolkit.Mvvm;
using System;
using SubmissionValues = Restless.Panama.Database.Tables.SubmissionTable.Defs.Values;

namespace Restless.Panama.ViewModel
{
    public abstract class BaseController<T1, T2> : DataRowViewModel<T2> where T1 : ViewModelBase where T2 : TableBase
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

        /// <summary>
        /// Gets the accepted icon, used in the tool tip
        /// </summary>
        public object AcceptedIcon => ResourceKeys.Icon.GetTitleStatusIcon(SubmissionValues.StatusAccepted);

        /// <summary>
        /// Gets the withdrawn icon, used in the tool tip
        /// </summary>
        public object WithdrawnIcon => ResourceKeys.Icon.GetTitleStatusIcon(SubmissionValues.StatusWithdrawn);
    }
}
