/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Core.Database.SQLite;
using System.Collections.Generic;
using System.Data;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the author table.
    /// </summary>
    public class AuthorTable : Core.ApplicationTableBase
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
            public const string TableName = "author";

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
                /// The name of the author name  column.
                /// </summary>
                public const string Name = "name";
                /// <summary>
                /// The name of the IsDefault author column.
                /// </summary>
                public const string IsDefault = "isdefault";
            }

            /// <summary>
            /// Provides static relation names.
            /// </summary>
            public static class Relations
            {
                /// <summary>
                /// The name of the relation that relates the <see cref="AuthorTable"/> to the  <see cref="TitleTable"/>.
                /// </summary>
                public const string ToTitle = "AuthorToTitle";
            }

            /// <summary>
            /// Provides static values
            /// </summary>
            public static class Values
            {
                /// <summary>
                /// The system supplied author id
                /// </summary>
                public const long SystemAuthorId = 1;
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorTable"/> class.
        /// </summary>
        public AuthorTable() : base(Defs.TableName)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Loads the data from the database into the Data collection for this table.
        /// </summary>
        public override void Load()
        {
            Load(null, Defs.Columns.Name);
        }

        /// <summary>
        /// Provides an enumerable that gets all authors in order of name ASC.
        /// </summary>
        /// <returns>An enumerable that gets all authors</returns>
        public IEnumerable<AuthorRow> EnumerateAuthors()
        {
            foreach (DataRow row in EnumerateRows(null, Defs.Columns.Id))
            {
                yield return new AuthorRow(row);
            }
            yield break;
        }

        /// <summary>
        /// Gets the first author marked as default, or String.Empty if none.
        /// </summary>
        /// <returns>The author name, or an empty string if none marked as default.</returns>
        public string GetDefaultAuthorName()
        {
            foreach (AuthorRow author in EnumerateAuthors())
            {
                if (author.IsDefault)
                {
                    return author.Name;
                }
            }
            return string.Empty;
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
                { Defs.Columns.Name, ColumnType.Text },
                { Defs.Columns.IsDefault, ColumnType.Boolean, false, false, 0 }
            };
        }

        /// <summary>
        /// Gets a list of column names to use in subsequent initial insert operations.
        /// These are used only when the table is empty, i.e. upon first creation.
        /// </summary>
        /// <returns>A list of column names</returns>
        protected override List<string> GetPopulateColumnList()
        {
            return new List<string>() { Defs.Columns.Id, Defs.Columns.Name, Defs.Columns.IsDefault };
        }

        /// <summary>
        /// Provides an enumerable that returns values for each row to be populated.
        /// </summary>
        /// <returns>An IEnumerable</returns>
        protected override IEnumerable<object[]> EnumeratePopulateValues()
        {
            yield return new object[] { Defs.Values.SystemAuthorId, "Your author name", true };
        }

        /// <summary>
        /// Establishes parent / child relationships with other tables.
        /// </summary>
        protected override void SetDataRelations()
        {
            CreateParentChildRelation<TitleTable>(Defs.Relations.ToTitle, Defs.Columns.Id, TitleTable.Defs.Columns.AuthorId);
        }

        /// <summary>
        /// Populates a new row with default (starter) values
        /// </summary>
        /// <param name="row">The freshly created DataRow to poulate</param>
        protected override void PopulateDefaultRow(DataRow row)
        {
            row[Defs.Columns.Name] = "(new author)";
            row[Defs.Columns.IsDefault] = false;
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Gets the first author id that is marked as default.
        /// If none are marked as default, returns the first id.
        /// </summary>
        /// <returns>The default id</returns>
        internal long GetDefaultAuthorId()
        {
            long firstId = -1;
            
            foreach (AuthorRow author in EnumerateAuthors())
            {
                if (firstId == -1)
                {
                    firstId = author.Id;
                }
                if (author.IsDefault)
                {
                    return author.Id;
                }
            }
            return firstId;
        }
        #endregion
    }
}