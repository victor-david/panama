/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/

using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using System.Linq;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains schema information.
    /// </summary>
    public class SchemaTable : Core.ApplicationTableBase
    {
        #region Public properties
        /// <summary>
        /// Provides static definitions for table properties such as column names and relation names.
        /// </summary>
        public static class Defs
        {
            /// <summary>
            /// Specifies the name of this table.
            /// </summary>
            public const string TableName = "schema";

            /// <summary>
            /// Provides static column names for this table.
            /// </summary>
            public static class Columns
            {
                /// <summary>
                /// The name of the id column. This is the table's primary key.
                /// </summary>
                public const string Id = DefaultPrimaryKeyName;

                /// <summary>
                /// Table name
                /// </summary>
                public const string TableName = "name";

                /// <summary>
                /// Schema version.
                /// </summary>
                public const string SchemaVersion = "schemaversion";

                /// <summary>
                /// Data version
                /// </summary>
                public const string DataVersion = "dataversion";

                /// <summary>
                /// Updated date / time
                /// </summary>
                public const string Updated = "updated";

                /// <summary>
                /// Note about schema update
                /// </summary>
                public const string Note = "note";
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaTable"/> class.
        /// </summary>
        public SchemaTable() : base(Defs.TableName)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Loads the data from the database into the Rows collection for this table.
        /// </summary>
        public override void Load()
        {
            Load(null, null);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Gets the column definitions for this table.
        /// </summary>
        /// <returns>A <see cref="ColumnDefinitionCollection"/>.</returns>
        protected override ColumnDefinitionCollection GetColumnDefinitions()
        {
            return new ColumnDefinitionCollection()
            {
                { Defs.Columns.Id, ColumnType.Integer, true },
                { Defs.Columns.TableName, ColumnType.Text },
                { Defs.Columns.SchemaVersion, ColumnType.Integer },
                { Defs.Columns.DataVersion, ColumnType.Integer },
                { Defs.Columns.Updated, ColumnType.Timestamp },
                { Defs.Columns.Note, ColumnType.Text }
            };
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        internal bool HaveSchemaRecord(string tableName, long schemaVersion, long dataVersion)
        {
            DataRow[] rows =
                Select($"{Defs.Columns.TableName}='{tableName}' and {Defs.Columns.SchemaVersion}= {schemaVersion} and {Defs.Columns.DataVersion}={dataVersion}");
            return rows.Length != 0;
        }

        internal void AddSchemaRecord(string tableName, long schemaVersion, long dataVersion, string note)
        {
            DataRow row = NewRow();
            row[Defs.Columns.TableName] = tableName;
            row[Defs.Columns.SchemaVersion] = schemaVersion;
            row[Defs.Columns.DataVersion] = dataVersion;
            row[Defs.Columns.Note] = note;
            row[Defs.Columns.Updated] = DateTime.UtcNow;
            Rows.Add(row);
        }

        internal void RegisterSchema(long schemaVersion)
        {
            foreach (Core.ApplicationTableBase table in Controller.DataSet.Tables.OfType<Core.ApplicationTableBase>())
            {
                DataRow[] rows = Select($"{Defs.Columns.TableName}='{table.TableName}'");
                if (rows.Length == 0)
                {
                    AddSchemaRecord(table.TableName, schemaVersion, 1, "---Init");
                }
            }
            Save();
        }
        #endregion
    }
}