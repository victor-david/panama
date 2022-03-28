/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Handles submission dates
    /// </summary>
    public class SubmissionDateController : BaseController<SubmissionViewModel, DummyTable>
    {
        private ResponseTable ResponseTable => DatabaseController.Instance.GetTable<ResponseTable>();

        #region Public properties
        /// <summary>
        /// Gets a the header for the submitted date
        /// </summary>
        public override string Header1 => $"{Strings.TextSubmitted}: {GetDateString(Owner.SelectedBatch?.Submitted)}";

        /// <summary>
        /// Gets or sets the submitted date.
        /// </summary>
        public DateTime? SubmittedDate
        {
            get => Owner.SelectedBatch?.Submitted ?? null;
            set
            {
                if (Owner.SelectedBatch != null && value is DateTime date)
                {
                    Owner.SelectedBatch.Submitted = date;
                    OnSubmittedPropertiesChanged();
                }
            }
        }

        /// <summary>
        /// Gets a the header for the response date
        /// </summary>
        public override string Header2 => $"{Strings.TextResponse}: {GetDateString(Owner.SelectedBatch?.Response)}";


        public DateTime? ResponseDate
        {
            get => Owner.SelectedBatch?.Response;
            set
            {
                if (Owner.SelectedBatch != null)
                {
                    Owner.SelectedBatch.Response = value.HasValue ? value : null;
                    OnResponsePropertiesChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates if the selected row contains a response date
        /// </summary>
        public bool HaveResponseDate => ResponseDate is DateTime;

        /// <summary>
        /// Gets or sets the response type
        /// </summary>
        public long ResponseType
        {
            get => Owner.SelectedBatch?.ResponseType ?? ResponseTable.Defs.Values.NoResponse;
            set => Owner.SelectedBatch?.SetResponseType(value);
        }

        public IEnumerable<ResponseRow> Responses => ResponseTable.EnumerateResponses();

        public ICommand ClearResponseCommand
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionDateController"/> class.
        /// </summary>
        /// <param name="owner">The owner of this controller.</param>
        public SubmissionDateController(SubmissionViewModel owner) : base(owner)
        {
            ClearResponseCommand = RelayCommand.Create(p => ResponseDate = null, p => HaveResponseDate);
        }
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
            OnResponsePropertiesChanged();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private string GetDateString(object parm)
        {
            return parm is DateTime date ? date.ToLocalTime().ToString(Config.Instance.DateFormat, CultureInfo.InvariantCulture) : Strings.TextNone;
        }

        private void OnSubmittedPropertiesChanged()
        {
            OnPropertyChanged(nameof(Header1));
            OnPropertyChanged(nameof(Header2));
            OnPropertyChanged(nameof(SubmittedDate));
            Owner.SetSubmissionHeader();
        }

        private void OnResponsePropertiesChanged()
        {
            OnPropertyChanged(nameof(HaveResponseDate));
            OnPropertyChanged(nameof(ResponseDate));
            OnPropertyChanged(nameof(Header2));
            OnPropertyChanged(nameof(ResponseType));
        }


        #endregion
    }
}