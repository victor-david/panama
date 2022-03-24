using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.LinkTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="LinkTable"/>
    /// </summary>
    public class LinkRow : RowObjectBase<LinkTable>
    {
        #region Properties
        /// <summary>
        /// Gets the default name value.
        /// </summary>
        public const string DefaultValue = "(none)";

        /// <summary>
        /// Gets the record id.
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets or sets the author name.
        /// </summary>
        public string Name
        {
            get => GetString(Columns.Name);
            set => SetValue(Columns.Name, value.ToDefaultValue(DefaultValue));
        }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public string Url
        {
            get => GetString(Columns.Url);
            set => SetValue(Columns.Url, value.ToDefaultValue(DefaultValue));
        }

        /// <summary>
        /// Gets or sets the notes.
        /// </summary>
        public string Notes
        {
            get => GetString(Columns.Notes);
            set => SetValue(Columns.Notes, value);
        }

        /// <summary>
        /// Gets the credential id.
        /// </summary>
        public long CredentialId => GetInt64(Columns.CredentialId);

        /// <summary>
        /// Gets the date/time record added.
        /// </summary>
        public DateTime Added => GetDateTime(Columns.Added);
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorRow"/> class
        /// </summary>
        /// <param name="row">The data row</param>
        public LinkRow(DataRow row) : base(row)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Creates a new <see cref="LinkRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new row, or null.</returns>
        public static LinkRow Create(DataRow row)
        {
            return row != null ? new LinkRow(row) : null;
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