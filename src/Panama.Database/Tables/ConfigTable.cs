/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains the application configuration.
    /// </summary>
    public class ConfigTable : Core.ApplicationTableBase
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
            public const string TableName = "config";

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
                /// The name of the value column.
                /// </summary>
                public const string Value = "value";
            }

            /// <summary>
            /// Provides static names of values used to describe the configuration type.
            /// </summary>
            public static class Types
            {
                /// <summary>
                /// The name that identifies the type as a boolean value.
                /// </summary>
                public const string Boolean = "bool";

                /// <summary>
                /// The name that identifies the type as a string value.
                /// </summary>
                public const string String = "string";

                /// <summary>
                /// The name that identifies the type as a string value that can span multiple lines.
                /// </summary>
                public const string MultiString = "mstring";

                /// <summary>
                /// The name that identifies the type as a path value.
                /// </summary>
                public const string Path = "path";

                /// <summary>
                /// The name that identifies the type as a mapi path value.
                /// </summary>
                public const string Mapi = "mapi";

                /// <summary>
                /// The name that identifies the type as an integer value.
                /// </summary>
                public const string Integer = "int";

                /// <summary>
                /// The name that identifies the type as a color value.
                /// </summary>
                public const string Color = "color";

                /// <summary>
                /// The name that identifies the type as an object value. These values are XML serialized.
                /// </summary>
                public const string Object = "object";
            }

            /// <summary>
            /// Provides static names of primary key id values.
            /// </summary>
            public static class FieldIds
            {
                /// <summary>
                /// The id value that identifies the title root folder.
                /// </summary>
                public const string FolderTitleRoot = "FolderTitleRoot";

                /// <summary>
                /// The id value that identifies the submission document folder.
                /// </summary>
                public const string FolderSubmissionDocument = "FolderSubmissionDocument";

                /// <summary>
                /// The id value that identifies the submission message attachment folder.
                /// </summary>
                public const string FolderSubmissionMessageAttachment = "FolderSubmissionMessageAttachment";

            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigTable"/> class.
        /// </summary>
        public ConfigTable() : base(Defs.TableName)
        {
            IsDeleteRestricted = true;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Loads the data from the database into the Data collection for this table.
        /// </summary>
        public override void Load()
        {
            Load(null, Defs.Columns.Id);
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
                { Defs.Columns.Id, ColumnType.Text, true },
                { Defs.Columns.Value, ColumnType.Text, false, true },
            };
        }

        /// <summary>
        /// Sets extended properties on certain columns. See the base implemntation <see cref="TableBase.SetColumnProperties"/> for more information.
        /// </summary>
        protected override void SetColumnProperties()
        {
            SetColumnProperty(Columns[Defs.Columns.Id], DataColumnPropertyKey.ExcludeFromUpdate);
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// From within this assembly, gets a configuration value specified by id.
        /// </summary>
        /// <param name="id">The id</param>
        /// <returns>The value</returns>
        internal string GetRowValue(string id)
        {
            DataRow[] rows = Select(string.Format("{0}='{1}'", Defs.Columns.Id, id));
            if (rows.Length == 1)
            {
                return rows[0][Defs.Columns.Value].ToString();
            }

            throw new IndexOutOfRangeException();
        }
        #endregion
  
    }
}