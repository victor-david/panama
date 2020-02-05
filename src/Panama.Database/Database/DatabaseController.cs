/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.Database.Tables;
using Restless.Tools.Database.SQLite;
using Restless.Tools.Utility;
using System;
using System.IO;

namespace Restless.App.Panama.Database
{
    /// <summary>
    /// A singleton class to provide high level database management via SQLite
    /// </summary>
    public sealed class DatabaseController : DatabaseControllerBase
    {
        #region Private
        private static DatabaseController instance;

        private const string DefaultDbDirectory = "db";
#if DEBUG
        private const string DefaultDbFileName = "PanamaDebug.sqlite";
#else
        private const string DefaultDbFileName = "PanamaLive.sqlite";
#endif
        #endregion

        /************************************************************************/

        #region Public Properties
        /// <summary>
        /// Gets the name of the folder where shema files are held.
        /// </summary>
        public const string SchemaFolder = "Schema";

        /// <summary>
        /// Gets the current schema version number.
        /// </summary>
        public const int SchemaVersion = 100;
        #endregion

        /************************************************************************/
        
        #region Singleton access and constructor
        /// <summary>
        /// Gets the singleton instance of this class
        /// </summary>
        public static DatabaseController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseController();
                }
                return instance;
            }
        }

        /// <summary>
        /// Constructor (private)
        /// </summary>
        private DatabaseController() : base()
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Initializes the database controller by creating and registering 
        /// all of the tables for the application.
        /// </summary>
        /// <param name="installationFolder">The installation folder for the application.</param>
        /// <param name="fileName">The name of the database file name, or null to use the default.</param>
        public void Init(string installationFolder, string fileName)
        {
            Validations.ValidateNullEmpty(installationFolder, "Init.InstallationFolder");
            base.CreateAndOpen(GetDatabaseFileName(installationFolder, fileName));
            CreateAndRegisterTable<AlertTable>();
            CreateAndRegisterTable<AuthorTable>();
            CreateAndRegisterTable<ColorTable>();
            CreateAndRegisterTable<ConfigTable>();
            CreateAndRegisterTable<CredentialTable>();
            CreateAndRegisterTable<DocumentTypeTable>();
            CreateAndRegisterTable<DummyTable>();
            CreateAndRegisterTable<LanguageTable>();
            CreateAndRegisterTable<LinkTable>();
            CreateAndRegisterTable<PublishedTable>();
            CreateAndRegisterTable<PublisherTable>();
            CreateAndRegisterTable<ResponseTable>();
            CreateAndRegisterTable<SubmissionTable>();
            CreateAndRegisterTable<SubmissionBatchTable>();
            CreateAndRegisterTable<SubmissionDocumentTable>();
            CreateAndRegisterTable<SubmissionMessageTable>();
            CreateAndRegisterTable<SubmissionMessageAttachmentTable>();
            CreateAndRegisterTable<SubmissionPeriodTable>();
            CreateAndRegisterTable<SchemaTable>();
            CreateAndRegisterTable<SelfPublishedTable>();
            CreateAndRegisterTable<SelfPublisherTable>();
            CreateAndRegisterTable<TableTable>();
            CreateAndRegisterTable<TagTable>();
            CreateAndRegisterTable<TitleTable>();
            CreateAndRegisterTable<TitleTagTable>();
            CreateAndRegisterTable<TitleVersionTable>();
            CreateAndRegisterTable<UserNoteTable>();
            TableRegistrationComplete();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        #endregion

        /************************************************************************/

        #region Private methods

        private string GetDatabaseFileName(string root, string databaseFileName)
        {
            if (string.IsNullOrWhiteSpace(databaseFileName))
            {
                // Note: DefaultDbFileName (a constant) is different in DEBUG and RELEASE modes
                databaseFileName = DefaultDbFileName;
            }
            return Path.Combine(root, DefaultDbDirectory, databaseFileName);
        }
        #endregion


    }
}