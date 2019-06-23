/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.View;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the selection logic for <see cref="SubmissionDocumentSelectWindow"/>. 
    /// Used when the user wants to create a submission document.
    /// </summary>
    public class SubmissionDocumentSelectWindowViewModel : WindowViewModel
    {
        #region Public properties
        /// <summary>
        /// Gets the submission document creation type as selected by the user.
        /// </summary>
        public SubmissionDocumentCreateType CreateType
        {
            get;
            private set;
        }
        #endregion
        
        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionDocumentSelectWindowViewModel"/> class.
        /// </summary>
        public SubmissionDocumentSelectWindowViewModel()
        {
            CreateType = SubmissionDocumentCreateType.None;
            Commands.Add("CreateDocx", (p) =>
                {
                    CreateType = SubmissionDocumentCreateType.CreateDocX;
                    CloseWindowCommand.Execute(null);
                });

            Commands.Add("CreatePlaceholder", (p) =>
            {
                CreateType = SubmissionDocumentCreateType.CreatePlaceholder;
                CloseWindowCommand.Execute(null);
            });
        }
        #endregion
    }
}