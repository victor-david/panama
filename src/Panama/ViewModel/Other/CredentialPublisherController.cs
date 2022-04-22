/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Controls;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides logic to display publishers that are related to a credential.
    /// </summary>
    public class CredentialPublisherController : BaseController<CredentialViewModel, CredentialTable>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialPublisherController"/> class.
        /// </summary>
        /// <param name="owner">The owner of this controller.</param>
        public CredentialPublisherController(CredentialViewModel owner) : base(owner)
        {
            Columns.Create("Id", PublisherTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.W042);
            Columns.Create("Publisher", PublisherTable.Defs.Columns.Name).MakeInitialSortAscending();
            Columns.Create("Added", PublisherTable.Defs.Columns.Added).MakeDate();
            Columns.Create("Last Sub", PublisherTable.Defs.Columns.Calculated.LastSub).MakeDate();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        /// Called by the <see cref=" ControllerBase{VM,T}.Owner"/> of this controller
        protected override void OnUpdate()
        {
            // TODO
            //long credentialId = GetOwnerSelectedPrimaryId();
            //MainView.RowFilter = string.Format("{0}={1}", PublisherTable.Defs.Columns.CredentialId, credentialId);
        }
        #endregion
    }
}