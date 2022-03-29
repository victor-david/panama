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
    /// Represents the table that contains information on the type of responses that a submssion batch may have.
    /// This is a lookup table.
    /// </summary>
    public class ResponseTable : Core.ApplicationTableBase
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
            public const string TableName = "response";

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
                /// The name of the name column. This column holds the name of the response type.
                /// </summary>
                public const string Name = "name";

                /// <summary>
                /// The name of the description column. This column holds a longer description of the response type.
                /// </summary>
                public const string Description = "description";
            }

            /// <summary>
            /// Provides static relation names.
            /// </summary>
            public static class Relations
            {
                /// <summary>
                /// The name of the relation that relates the <see cref="ResponseTable"/> to the <see cref="SubmissionBatchTable"/>.
                /// </summary>
                public const string ToSubmissionBatch = "ResponseToSubBatch";
            }

            /// <summary>
            /// Provides static reponse values.
            /// </summary>
            public static class Values
            {
                /// <summary>
                /// The value used when a submission has not yet received a response.
                /// </summary>
                public const long NoResponse = 0;

                /// <summary>
                /// The value used when the response is of an unspecified type.
                /// </summary>
                public const long ResponseNotSpecified = 1;

                /// <summary>
                /// Response is a bfn
                /// </summary>
                public const long ResponseBfn = 2;

                /// <summary>
                /// Response is a try again
                /// </summary>
                public const long ResponseTryAgain = 3;

                /// <summary>
                /// Response contains something personal
                /// </summary>
                public const long ResponsePersonal = 4;

                /// <summary>
                /// Response is a general blast
                /// </summary>
                public const long ResponseGeneralBlast = 10;

                /// <summary>
                /// Response is flakey
                /// </summary>
                public const long ResponseFlakey = 11;

                /// <summary>
                /// The value used when the response is an acceptance.
                /// </summary>
                public const long ResponseAccepted = 255;

            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseTable"/> class.
        /// </summary>
        public ResponseTable() : base(Defs.TableName)
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
            Load(null, Defs.Columns.Id);
        }

        /// <summary>
        /// Provides an enumerable that enumerates all valid responses,
        /// excludes <see cref="Defs.Values.NoResponse"/>
        /// </summary>
        /// <returns>An enumerable</returns>
        public IEnumerable<ResponseRow> EnumerateResponses()
        {
            foreach (DataRow row in EnumerateRows($"{Defs.Columns.Id} <> {Defs.Values.NoResponse}", Defs.Columns.Id))
            {
                yield return new ResponseRow(row);
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
                { Defs.Columns.Name, ColumnType.Text, false, true },
                { Defs.Columns.Description, ColumnType.Text },
            };
        }

        /// <summary>
        /// Gets a list of column names to use in subsequent initial insert operations.
        /// These are used only when the table is empty, i.e. upon first creation.
        /// </summary>
        /// <returns>A list of column names</returns>
        protected override List<string> GetPopulateColumnList()
        {
            return new List<string>() { Defs.Columns.Id, Defs.Columns.Name, Defs.Columns.Description };
        }

        /// <summary>
        /// Provides an enumerable that returns values for each row to be populated.
        /// </summary>
        /// <returns>An IEnumerable</returns>
        protected override IEnumerable<object[]> EnumeratePopulateValues()
        {
            yield return new object[] { Defs.Values.NoResponse, null, "None. The submission has not yet received a response." };
            yield return new object[] { Defs.Values.ResponseNotSpecified, "(not specified)", "Not Specified. The response type has not been specified." };
            yield return new object[] { Defs.Values.ResponseBfn, "BFN", "Form Rejection. Standard BFN note. No personalization. No encouragement." };
            yield return new object[] { Defs.Values.ResponseTryAgain, "Try Again", "A rejection with at least a hint of encouragement to submit again." };
            yield return new object[] { Defs.Values.ResponsePersonal, "Personal Note", "Received a personal note." };
            yield return new object[] { Defs.Values.ResponseGeneralBlast, "General Blast", "Sent a general email announcing new issue / contest winners, or just posted on web site." };
            yield return new object[] { Defs.Values.ResponseFlakey, "Flakey", "Flakey. No response." };
            yield return new object[] { Defs.Values.ResponseAccepted, "Accepted", "Acceptance of one or more pieces in the submission." };
        }

        /// <summary>
        /// Establishes parent / child relationships with other tables.
        /// </summary>
        protected override void SetDataRelations()
        {
            CreateParentChildRelation<SubmissionBatchTable>(Defs.Relations.ToSubmissionBatch, Defs.Columns.Id, SubmissionBatchTable.Defs.Columns.ResponseType);
        }
        #endregion
    }
}