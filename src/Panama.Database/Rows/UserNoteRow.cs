using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.UserNoteTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="UserNoteTable"/>
    /// </summary>
    public class UserNoteRow : RowObjectBase<UserNoteTable>
    {
        #region Properties
        /// <summary>
        /// Gets the default author value.
        /// </summary>
        public const string DefaultTitle = "(none)";

        /// <summary>
        /// Gets the record id.
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string Title
        {
            get => GetString(Columns.Title);
            set => SetValue(Columns.Title, value.ToDefaultValue(DefaultTitle));
        }

        /// <summary>
        /// Gets or sets the note
        /// </summary>
        public string Note
        {
            get => GetString(Columns.Note);
            set => SetValue(Columns.Note, value);
        }

        /// <summary>
        /// Gets the date/time record created
        /// </summary>
        public DateTime Created => GetDateTime(Columns.Created);
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UserNoteRow"/> class
        /// </summary>
        /// <param name="row">The data row</param>
        public UserNoteRow(DataRow row) : base(row)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Creates a new <see cref="AuthorRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new tag row, or null.</returns>
        public static UserNoteRow Create(DataRow row)
        {
            return row != null ? new UserNoteRow(row) : null;
        }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return Title;
        }
        #endregion
    }
}