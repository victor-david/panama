/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tools.Database.SQLite;
using System;
using System.Collections.Generic;
using System.Data;

namespace Restless.App.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains credential for accessing various systems such as submission systems.
    /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class CredentialTable : TableBase
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
            public const string TableName = "credential";

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
                /// The name of the name column.
                /// </summary>
                public const string Name = "name";

                /// <summary>
                /// The name of the login id column.
                /// </summary>
                public const string LoginId = "loginid";

                /// <summary>
                /// The name of the password column.
                /// </summary>
                public const string Password = "password";

            }
        }

        /// <summary>
        /// Gets the column name of the primary key.
        /// </summary>
        public override string PrimaryKeyName
        {
            get { return Defs.Columns.Id; }
        }
        #endregion

        /************************************************************************/
        
        #region Constructor
        #pragma warning disable 1591
        public CredentialTable() : base(DatabaseController.Instance, Defs.TableName)
        {
        }
        #pragma warning restore 1591
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
        /// Provides an enumerable that gets all credentials in order of name ASC.
        /// </summary>
        /// <returns>A <see cref="RowObject"/></returns>
        public IEnumerable<RowObject> EnumerateCredentials()
        {
            DataRow zeroRow = NewRow();
            zeroRow[Defs.Columns.Id] = 0;
            zeroRow[Defs.Columns.Name] = "None";
            zeroRow[Defs.Columns.LoginId] = "***";
            zeroRow[Defs.Columns.Password] = "***";

            yield return new RowObject(zeroRow);

            DataRow[] rows = Select(null, $"{Defs.Columns.Name} ASC");
            foreach (DataRow row in rows)
            {
                yield return new RowObject(row);
            }
            yield break;
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
            return Resources.Create.Credential;
        }

        /// <summary>
        /// Sets extended properties on certain columns. See the base implemntation <see cref="TableBase.SetColumnProperties"/> for more information.
        /// </summary>
        protected override void SetColumnProperties()
        {
            SetColumnProperty(Columns[Defs.Columns.Id], DataColumnPropertyKey.ExcludeFromInsert, DataColumnPropertyKey.ExcludeFromUpdate, DataColumnPropertyKey.ReceiveInsertedId);
        }

        /// <summary>
        /// Populates a new row with default (starter) values
        /// </summary>
        /// <param name="row">The freshly created DataRow to poulate</param>
        protected override void PopulateDefaultRow(System.Data.DataRow row)
        {
            row[Defs.Columns.Name] = "(new credential)";
            row[Defs.Columns.LoginId] = "(new login id)";
            row[Defs.Columns.Password] = "(new password)";
        }
        #endregion

        /************************************************************************/

        #region Row Object
        /// <summary>
        /// Encapsulates a single row from the <see cref="CredentialTable"/>.
        /// </summary>
        public class RowObject : RowObjectBase<CredentialTable>
        {
            #region Public properties
            /// <summary>
            /// Gets the id for this row object.
            /// </summary>
            public long Id
            {
                get { return GetInt64(Defs.Columns.Id); }
            }

            /// <summary>
            /// Gets or sets the name for this row object.
            /// </summary>
            public string Name
            {
                get { return GetString(Defs.Columns.Name); }
                set { SetValue(Defs.Columns.Name, value); }
            }
            /// <summary>
            /// Gets or sets the login id for this row object.
            /// </summary>
            public string LoginId
            {
                get { return GetString(Defs.Columns.LoginId); }
                set { SetValue(Defs.Columns.LoginId, value); }
            }

            /// <summary>
            /// Gets or sets the author id for this row object.
            /// </summary>
            public string Password
            {
                get { return GetString(Defs.Columns.Password); }
                set { SetValue(Defs.Columns.Password, value); }
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
            /// <returns>A string with the name and login id concatenated.</returns>
            public override string ToString()
            {
                string loginId = string.Empty;
                if (Id > 0)
                {
                    loginId = string.Format(" ({0})", LoginId);
                }
                return string.Format("{0}{1}", Name, loginId);
            }
            #endregion
        }
        #endregion

    }
}