/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Core;
using Restless.Toolkit.Core.Database.SQLite;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents a dummy table used in certain classes where a table type must particpate in the inheritance but is not used.
    /// </summary>
    public class DummyTable : MemoryTable
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
            public const string TableName = "dummy";
        }

        /// <summary>
        /// Gets the column name of the primary key.
        /// </summary>
        public override string PrimaryKeyName
        {
            get => null;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DummyTable"/> class.
        /// </summary>
        public DummyTable() : base(DatabaseController.Instance, Defs.TableName)
        {
        }
        #endregion
    }
}