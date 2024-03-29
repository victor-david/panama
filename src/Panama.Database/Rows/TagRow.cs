﻿using Restless.Toolkit.Core.Database.SQLite;
using System.Data;
using Columns = Restless.Panama.Database.Tables.TagTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="TagTable"/>.
    /// </summary>
    public class TagRow : RowObjectBase<TagTable>
    {
        #region Properties
        /// <summary>
        /// Gets the record id.
        /// </summary>
        public long Id
        {
            get => GetInt64(Columns.Id);
        }

        /// <summary>
        /// Gets or sets the tag name.
        /// </summary>
        public string Tag
        {
            get => GetString(Columns.Tag);
            set => SetValue(Columns.Tag, value);
        }

        /// <summary>
        /// Gets or sets the tag description.
        /// </summary>
        public string Description
        {
            get => GetString(Columns.Description);
            set => SetValue(Columns.Description, value);
        }

        /// <summary>
        /// Gets the usage count.
        /// </summary>
        public long UsageCount
        {
            get => GetInt64(Columns.Calculated.UsageCount);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TagRow"/> class.
        /// </summary>
        /// <param name="row">The data row</param>
        public TagRow(DataRow row) : base(row)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Creates a new <see cref="TagRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new tag row, or null.</returns>
        public static TagRow Create(DataRow row)
        {
            return row != null ? new TagRow(row) : null;
        }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return Tag;
        }
        #endregion
    }
}