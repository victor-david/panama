using Restless.Panama.Database.Tables;
using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.IO;
using System.Linq;

namespace Restless.Panama.Database.Core
{
    /// <summary>
    /// A singleton class to provide high level database management via SQLite
    /// </summary>
    public sealed class DatabaseController : DatabaseControllerBase
    {
        #region Private
        private const string MainFileNameV4 = "MAIN0-13D729DF-6EAC-40CD-946B-094685DA8638";
        private const string MainFileNameV5 = "panama.dat";

#if DEBUG
        private const string DataSetV4 = "SETD-V400-65E6-4964-8087-6B05";
        private const string DataSetV5 = "SETD-V500";
#else
        private const string DataSetV4 = "SETR-V400-65E6-4964-8087-6B05";
        private const string DataSetV5 = "SETR-V500";
#endif
        #endregion

        /************************************************************************/

        #region Public
        /// <summary>
        /// Gets the schema version
        /// </summary>
        public override long DefaultSchemaVersion => 500;

        /// <summary>
        /// Gets the database root location. This value is passed to the <see cref="Init(string)"/>
        /// method at application startup, and may be changed by the user (requires app restart)
        /// </summary>
        public string DatabaseRoot
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Internal
        /// <summary>
        /// Gets the name for the attached data schema. This schema holds all the main tables.
        /// </summary>
        internal const string MainAppSchemaName = "panama";

        /// <summary>
        /// Gets the name for the attached memory only schema
        /// </summary>
        internal const string MemorySchemaName = "mem";
        #endregion

        /************************************************************************/

        #region Singleton access and constructor
        /// <summary>
        /// Gets the singleton instance of this class
        /// </summary>
        public static DatabaseController Instance { get; } = new DatabaseController();

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
        /// <param name="databaseRoot">The database root folder.</param>
        public void Init(string databaseRoot)
        {
            ThrowIfEmpty(databaseRoot);
            DatabaseRoot = databaseRoot;
            CopyDatabaseIfNeeded();
            CreateAndOpen(MemoryDatabase);

            AttachMemorySchema();
            AttachMainSchema(DataSetV5, MainFileNameV5);

            TableRegistrationComplete(MainAppSchemaName);
            TableRegistrationComplete(MemorySchemaName);

            RegisterSchema();
            PerformTableUpdate();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void CopyDatabaseIfNeeded()
        {
            string fileNameV4 = GetFullFileName(DataSetV4, MainFileNameV4);
            string fileNameV5 = GetFullFileName(DataSetV5, MainFileNameV5);
            Directory.CreateDirectory(Path.GetDirectoryName(fileNameV5));
            if (!File.Exists(fileNameV5))
            {
                File.Copy(fileNameV4, fileNameV5);
            }
        }

        private void AttachMainSchema(string dataSet, string fileName)
        {
            // throws if either is empty
            string fullFileName =  GetFullFileName(dataSet, fileName);

            Attach(MainAppSchemaName, fullFileName, () =>
            {
                CreateAndRegisterTable<AlertTable>();
                CreateAndRegisterTable<AuthorTable>();
                CreateAndRegisterTable<ColorTable>();
                CreateAndRegisterTable<ConfigTable>();
                CreateAndRegisterTable<CredentialTable>();
                CreateAndRegisterTable<DocumentTypeTable>();
                CreateAndRegisterTable<DummyTable>();
                CreateAndRegisterTable<LanguageTable>();
                CreateAndRegisterTable<LinkTable>();
                CreateAndRegisterTable<LinkVerifyTable>();
                CreateAndRegisterTable<OrphanExclusionTable>();
                CreateAndRegisterTable<PublishedTable>();
                CreateAndRegisterTable<PublisherTable>();
                CreateAndRegisterTable<QueueTable>();
                CreateAndRegisterTable<QueueTitleTable>();
                CreateAndRegisterTable<QueueTitleStatusTable>();
                CreateAndRegisterTable<ResponseTable>();
                CreateAndRegisterTable<SchemaTable>();
                CreateAndRegisterTable<SubmissionTable>();
                CreateAndRegisterTable<SubmissionBatchTable>();
                CreateAndRegisterTable<SubmissionDocumentTable>();
                CreateAndRegisterTable<SubmissionMessageTable>();
                CreateAndRegisterTable<SubmissionMessageAttachmentTable>();
                CreateAndRegisterTable<SubmissionPeriodTable>();
                CreateAndRegisterTable<SelfPublishedTable>();
                CreateAndRegisterTable<SelfPublisherTable>();
                CreateAndRegisterTable<TagTable>();
                CreateAndRegisterTable<ThemeTable>();
                CreateAndRegisterTable<TitleTable>();
                CreateAndRegisterTable<TitleRelatedTable>();
                CreateAndRegisterTable<TitleTagTable>();
                CreateAndRegisterTable<TitleVersionTable>();
                CreateAndRegisterTable<UserNoteTable>();
            });
        }

        private string GetFullFileName(string dataSet, string fileName)
        {
            ThrowIfEmpty(dataSet);
            ThrowIfEmpty(fileName);

            if (fileName != MemoryDatabase)
            {
                return Path.Combine(DatabaseRoot, dataSet, fileName);
            }
            return fileName;
        }

        private static void ThrowIfEmpty(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
            {
                throw new ArgumentException("Argument cannot be empty");
            }
        }

        private void AttachMemorySchema()
        {
            Attach(MemorySchemaName, MemoryDatabase, () =>
            {
                CreateAndRegisterTable<PublishedAllTable>();
                CreateAndRegisterTable<SearchTable>();
            });
        }

        private void RegisterSchema()
        {
            GetTable<SchemaTable>().RegisterSchema(DefaultSchemaVersion);
        }

        /// <summary>
        /// Performs table schema / data updates if needed
        /// </summary>
        private void PerformTableUpdate()
        {
            foreach (ApplicationTableBase table in DataSet.Tables.OfType<ApplicationTableBase>())
            {
                table.PerformSchemaUpdate();
                table.PerformDataUpdate();
            }
        }
        #endregion
    }
}