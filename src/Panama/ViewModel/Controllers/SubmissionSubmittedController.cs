/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.Core;
using Restless.App.Panama.Resources;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using System;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Submission submitted controller. Handles the submission submitted date
    /// </summary>
    public class SubmissionSubmittedController : SubmissionController
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets the submitted date. The UI binds to this property.
        /// </summary>
        public object SubmittedDate
        {
            get
            {
                if (Owner.IsSelectedRowAccessible)
                {
                    return Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Submitted];
                }
                return null;
            }
            set
            {
                if (Owner.IsSelectedRowAccessible)
                {
                    Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Submitted] = value;
                    OnSubmittedPropertiesChanged();
                }
            }
        }

        /// <summary>
        /// Gets a string value that displays the submitted date
        /// </summary>
        public override string Header
        {
            get
            {
                string dateStr = Strings.TextNone;
                if (Owner.IsSelectedRowAccessible && Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Submitted] is DateTime dt)
                {
                    dateStr = dt.ToLocalTime().ToString(Config.Instance.DateFormat);
                }
                return $"{Strings.TextSubmitted}: {dateStr}";
            }
        }
        #endregion

        /************************************************************************/

        #region Protected properties
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionSubmittedController"/> class.
        /// </summary>
        /// <param name="owner">The owner of this controller.</param>
        public SubmissionSubmittedController(SubmissionViewModel owner)
            : base(owner)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called by the <see cref=" ControllerBase{VM,T}.Owner"/> of this controller
        /// in order to update the controller values.
        /// </summary>
        protected override void OnUpdate()
        {
            OnSubmittedPropertiesChanged();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void OnSubmittedPropertiesChanged()
        {
            OnPropertyChanged(nameof(SubmittedDate));
            OnPropertyChanged(nameof(Header));
            Owner.SetSubmissionHeader();
        }
        #endregion

    }
}