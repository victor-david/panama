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
    [System.ComponentModel.DesignerCategory("foo")]
    public class AuthorTable : TableBase
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
                public const string Id = "id";
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
        }

        /// <summary>
        /// Gets the column name of the primary key.
        /// </summary>
        public override string PrimaryKeyName
        {
            get => Defs.Columns.Id;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorTable"/> class.
        /// </summary>
        public AuthorTable() : base(DatabaseController.Instance, Defs.TableName)
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
        /// <returns>A <see cref="RowObject"/></returns>
        public IEnumerable<RowObject> EnumerateAuthors()
        {
            DataRow[] rows = Select(null, $"{Defs.Columns.Name} ASC");
            foreach (DataRow row in rows)
            {
                yield return new RowObject(row);
            }
            yield break;
        }

        /// <summary>
        /// Gets the first author marked as default, or String.Empty if none.
        /// </summary>
        /// <returns>The author name, or an empty string if none marked as default.</returns>
        public string GetDefaultAuthorName()
        {
            DataRow[] rows = Select($"{Defs.Columns.IsDefault}=1", Defs.Columns.Id);
            if (rows.Length > 0)
            {
                return rows[0][Defs.Columns.Name].ToString();
            }
            return string.Empty;
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Gets the DDL needed to create this table.
        /// </summary>
        /// <returns>A SQL string that describes how to create this table.</returns>
        protected override string GetDdl()
        {
            return Resources.Create.Author;
        }

        /// <summary>
        /// Gets the SQL needed to populate this table with its default values.
        /// </summary>
        /// <returns>A SQL string to insert the default data.</returns>
        protected override string GetPopulateSql()
        {
            return Resources.Data.Author;
        }

        /// <summary>
        /// Sets extended properties on certain columns. See the base implemntation <see cref="TableBase.SetColumnProperties"/> for more information.
        /// </summary>
        protected override void SetColumnProperties()
        {
            SetColumnProperty(Columns[Defs.Columns.Id], DataColumnPropertyKey.ExcludeFromInsert, DataColumnPropertyKey.ExcludeFromUpdate, DataColumnPropertyKey.ReceiveInsertedId);
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
        protected override void PopulateDefaultRow(System.Data.DataRow row)
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
            DataRow[] rows = Select(null, Defs.Columns.Id);
            foreach (DataRow row in rows)
            {
                var obj = new RowObject(row);
                if (firstId == -1) firstId = obj.Id;
                if (obj.IsDefault) return obj.Id;
            }
            return firstId;
        }
        #endregion

        /************************************************************************/

        #region Row Object
        /// <summary>
        /// Encapsulates a single row from the <see cref="AuthorTable"/>.
        /// </summary>
        public class RowObject : RowObjectBase<AuthorTable>
        {
            #region Public properties
            /// <summary>
            /// Gets the id for this row object.
            /// </summary>
            public long Id
            {
                get => GetInt64(Defs.Columns.Id);
            }

            /// <summary>
            /// Gets or sets the author name.
            /// </summary>
            public string Name
            {
                get => GetString(Defs.Columns.Name);
                set => SetValue(Defs.Columns.Name, value);
            }

            /// <summary>
            /// Gets or sets the IsDefault flag.
            /// </summary>
            public bool IsDefault
            {
                get => GetBoolean(Defs.Columns.IsDefault);
                set => SetValue(Defs.Columns.IsDefault, value);
            }
            #endregion

            /************************************************************************/

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="RowObject"/> class.
            /// </summary>
            /// <param name="row">The data row</param>
            public RowObject(DataRow row)
                : base(row)
            {
            }
            #endregion

            /************************************************************************/

            #region Public methods
            /// <summary>
            /// Gets the string representation of this object.
            /// </summary>
            /// <returns>The <see cref="Name"/></returns>
            public override string ToString()
            {
                return Name;
            }
            #endregion
        }
        #endregion

    }
}