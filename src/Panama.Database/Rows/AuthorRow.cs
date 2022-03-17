using Restless.Toolkit.Core.Database.SQLite;
using System.Data;
using Columns = Restless.Panama.Database.Tables.AuthorTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="AuthorTable"/>
    /// </summary>
    public class AuthorRow : RowObjectBase<AuthorTable>
    {
        #region Properties
        /// <summary>
        /// Gets the default author value.
        /// </summary>
        public const string DefaultAuthor = "(none)";

        /// <summary>
        /// Gets the record id.
        /// </summary>
        public long Id
        {
            get => GetInt64(Columns.Id);
        }

        /// <summary>
        /// Gets or sets the author name.
        /// </summary>
        public string Name
        {
            get => GetString(Columns.Name);
            set => SetValue(Columns.Name, value.ToDefaultValue(DefaultAuthor));
        }

        /// <summary>
        /// Gets or sets the IsDefault flag.
        /// </summary>
        public bool IsDefault
        {
            get => GetBoolean(Columns.IsDefault);
            set => SetValue(Columns.IsDefault, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorRow"/> class
        /// </summary>
        /// <param name="row">The data row</param>
        public AuthorRow(DataRow row) : base(row)
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
        public static AuthorRow Create(DataRow row)
        {
            return row != null ? new AuthorRow(row) : null;
        }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
}