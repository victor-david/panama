/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.Tools.Database.SQLite;
using System.Data;
using Restless.Tools.Utility;

namespace Restless.App.Panama.Database
{
    /// <summary>
    /// Represents an OLEDB to SQLIte importer. Import is only available in DEBUG mode
    /// </summary>
    public class DatabaseImporter
    {
        #region Private
        private static DatabaseImporter instance;
        private System.Data.OleDb.OleDbConnection connection;
        //private bool IsEnabled = false;
        #endregion

        /************************************************************************/
        
        #region Static singleton instance and constructor

        /// <summary>
        /// Gets the singleton instance of this object
        /// </summary>
        public static DatabaseImporter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseImporter();
                }
                return instance;
            }
        }

        /// <summary>
        /// Creates a new instance of this object (private)
        /// </summary>
        private DatabaseImporter()
        {
            IsEnabled = false;
            // The  SetEnabled() method is marked conditional DEBUG. The compiler will omit the call to the method
            // if the DEBUG symbol is not defined. SetEnabled() calls SetEnabled2() which is marked conditional IMPORT.
            // The compiler will omit the call to SetEnabled2() if the IMPORT symbol is not defined. This means that
            // the importer will be enabled only in debug mode and with the IMPORT symbol defined.
            SetEnabled();
            if (IsEnabled)
            {
                var builder = new System.Data.OleDb.OleDbConnectionStringBuilder();
                builder.Provider = "Microsoft.ACE.OLEDB.12.0";
                builder.DataSource = @"D:\Writing\Title_Manager\db\title_manager_live_main.accdb";
                connection = new System.Data.OleDb.OleDbConnection(builder.ConnectionString);
            }
        }
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a boolean value that indicates if import operations are enabled
        /// </summary>
        public bool IsEnabled
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/
        
        #region Public Methods
        /// <summary>
        /// Imports a table
        /// </summary>
        /// <param name="dest">The destination table</param>
        /// <param name="importer">The import object that implements the IImporter interface</param>
        /// <param name="sourceName">The name of the table in the source, or null to use the same name as the destination</param>
        /// <returns>true if the table was successfuly imported; otherwise, false.</returns>
        public bool ImportTable(TableBase dest, IColumnRowImporter importer, string sourceName = null)
        {
            if (!IsEnabled) return false;

            Validations.ValidateNull(importer, "ImportTable.Importer");
            // if no name passed, use same name as destination
            if (string.IsNullOrEmpty(sourceName))
            {
                sourceName = dest.TableName;
            }

            dest.Rows.Clear();
            dest.Columns.Clear();
            connection.Open();

            using (var temp = new DataTable(sourceName))
            {
                string sql = string.Format("SELECT * FROM [{0}]", temp.TableName);
                var adapter = new System.Data.OleDb.OleDbDataAdapter(sql, connection);
                adapter.Fill(temp);

                foreach (DataColumn col in temp.Columns)
                {
                    if (importer.IncludeColumn(col.ColumnName))
                    {
                        dest.Columns.Add(new DataColumn(importer.GetColumnName(col.ColumnName), col.DataType));
                    }
                }

                foreach (DataRow row in temp.Rows)
                {
                    if (importer.GetRowConfirmation(row))
                    {
                        DataRow newRow = dest.NewRow();
                        int colIdx = 0;
                        foreach (DataColumn col in temp.Columns)
                        {
                            if (importer.IncludeColumn(col.ColumnName))
                            {
                                newRow[colIdx] = row[col];
                                colIdx++;
                            }
                        }
                        dest.Rows.Add(newRow);
                    }
                }
            }

            dest.Save();
            dest.Rows.Clear();
            dest.Columns.Clear();
            connection.Close();
            return true;
        }
        #endregion

        /************************************************************************/

        #region Private methods
        [System.Diagnostics.Conditional("DEBUG")]
        private void SetEnabled()
        {
            SetEnabled2();
        }

        [System.Diagnostics.Conditional("IMPORT")]
        private void SetEnabled2()
        {
            IsEnabled = true;
        }
        #endregion
    }
}