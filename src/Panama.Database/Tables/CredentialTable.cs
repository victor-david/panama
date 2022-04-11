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
    /// Represents the table that contains credential for accessing various systems such as submission systems.
    /// </summary>
    public class CredentialTable : Core.ApplicationTableBase
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
                public const string Id = DefaultPrimaryKeyName;

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
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialTable"/> class.
        /// </summary>
        public CredentialTable() : base(Defs.TableName)
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
        /// Provides an enumerable that gets all credentials in order of name ASC.
        /// </summary>
        /// <returns>An enumerable</returns>
        public IEnumerable<CredentialRow> EnumerateAll()
        {
            DataRow zeroRow = NewRow();
            zeroRow[Defs.Columns.Id] = 0;
            zeroRow[Defs.Columns.Name] = "None";
            zeroRow[Defs.Columns.LoginId] = "***";
            zeroRow[Defs.Columns.Password] = "***";

            yield return new CredentialRow(zeroRow);
            foreach (DataRow row in EnumerateRows(null, Defs.Columns.Name))
            {
                yield return new CredentialRow(row);
            }
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
                { Defs.Columns.LoginId, ColumnType.Text },
                { Defs.Columns.Password, ColumnType.Text },
            };
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


        //    /************************************************************************/
            
        //    #region Public methods
        //    /// <summary>
        //    /// Gets the string representation of this object.
        //    /// </summary>
        //    /// <returns>A string with the name and login id concatenated.</returns>
        //    public override string ToString()
        //    {
        //        string loginId = string.Empty;
        //        if (Id > 0)
        //        {
        //            loginId = string.Format(" ({0})", LoginId);
        //        }
        //        return string.Format("{0}{1}", Name, loginId);
        //    }
        //    #endregion
        //}
        //#endregion
    }
}