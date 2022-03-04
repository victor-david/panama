/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Core.Utility;
using Restless.Toolkit.Mvvm;
using System;
using System.IO;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents a single title version item for which to rename its associated file
    /// to be consistent with the title, version, and language that belongs to the version.
    /// </summary>
    public class TitleVersionRenameItem : ObservableObject
    {
        #region Private
        private readonly TitleVersionTable.RowObject ver;
        private string status;
        #endregion

        /************************************************************************/

        #region Public fields
        /// <summary>
        /// Provides static property name values to be used for binding.
        /// </summary>
        public static class Properties
        {
            /// <summary>
            /// The name of the <see cref="TitleVersionRenameItem.Status"/> property.
            /// </summary>
            public const string Status = nameof(Status);

            /// <summary>
            /// The name of the <see cref="TitleVersionRenameItem.Version"/> property.
            /// </summary>
            public const string Version = nameof(Version);

            /// <summary>
            /// The name of the <see cref="TitleVersionRenameItem.RevisionChar"/> property.
            /// </summary>
            public const string RevisionChar = nameof(RevisionChar);

            /// <summary>
            /// The name of the <see cref="TitleVersionRenameItem.OriginalNameDisplay"/> property.
            /// </summary>
            public const string OriginalNameDisplay = nameof(OriginalNameDisplay);

            /// <summary>
            /// The name of the <see cref="TitleVersionRenameItem.NewNameDisplay"/> property.
            /// </summary>
            public const string NewNameDisplay = nameof(NewNameDisplay);
        }
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the status message for this rename item.
        /// </summary>
        public string Status
        {
            get => status;
            private set => SetProperty(ref status, value);
        }

        /// <summary>
        /// Gets a boolean value indicating if the original file name and the new file name are the same, i.e. already renamed.
        /// </summary>
        public bool Same
        {
            get => OriginalName == NewName;
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public long Version
        {
            get => ver.Version;
        }

        /// <summary>
        /// Gets the revision.
        /// </summary>
        public long Revision
        {
            get => ver.Revision;
        }

        /// <summary>
        /// Gets the revision character.
        /// </summary>
        public char RevisionChar
        {
            get => (char)ver.Revision;
        }

        /// <summary>
        /// Gets the original file name.
        /// </summary>
        public string OriginalName
        {
            get;
            //private set;
        }

        /// <summary>
        /// Gets the original file name without any path part.
        /// </summary>
        public string OriginalNameDisplay
        {
            get;
            //private set;
        }

        /// <summary>
        /// Gets a boolean value that indicates if the original file exists.
        /// </summary>
        public bool OriginalExists
        {
            get => File.Exists(OriginalName);
        }

        /// <summary>
        /// Gets the proposed new name.
        /// </summary>
        public string NewName
        {
            get;
        }

        /// <summary>
        /// Gets the proposed new name without any path part.
        /// </summary>
        public string NewNameDisplay
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleVersionRenameItem"/> class.
        /// </summary>
        /// <param name="ver">The title version row object</param>
        /// <param name="title">The title</param>
        public TitleVersionRenameItem(TitleVersionTable.RowObject ver, string title)
        {
            this.ver = ver ?? throw new ArgumentNullException(nameof(ver));
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            OriginalName = Paths.Title.WithRoot(ver.FileName);
            OriginalNameDisplay = Path.GetFileName(OriginalName);

            /*
             * Examples of new file name
             * The Title Of This Piece_v3.A.en-us.docx
             * The Title Of This Piece_v3.B.es-mx.docx
             */
            string newNameWithoutPath =
                string.Format("{0}_v{1}.{2}.{3}{4}",
                    Format.ValidFileName(title),
                    Version,
                    RevisionChar,
                    ver.LanguageId,
                    Path.GetExtension(OriginalName));

            NewName = Path.Combine(Path.GetDirectoryName(OriginalName), newNameWithoutPath);
            NewNameDisplay = Path.GetFileName(NewName);

            if (!OriginalExists)
                Status = "Missing";
            else if (Same)
                Status = "Already renamed";
            else
                Status = "Ready to rename";
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Performs the rename operation on this item.
        /// </summary>
        /// <remarks>
        /// This method performs the rename operation for this item
        /// if <see cref="Same"/> is false, and <see cref="OriginalExists"/> is true.
        /// </remarks>
        public void Rename()
        {
            if (!Same && OriginalExists)
            {
                File.Move(OriginalName, NewName);
                ver.FileName = Paths.Title.WithoutRoot(NewName);
                Status = "Rename successful";
            }
        }
        #endregion
    }
}