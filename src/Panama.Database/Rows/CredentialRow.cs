using Restless.Toolkit.Core.Database.SQLite;
using System.Data;
using Columns = Restless.Panama.Database.Tables.CredentialTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="CredentialTable"/>
    /// </summary>
    public class CredentialRow : RowObjectBase<CredentialTable>
    {
        #region Properties
        /// <summary>
        /// Gets the default credential value.
        /// </summary>
        public const string DefaultCredential = "(none)";

        /// <summary>
        /// Gets the record id.
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets or sets the credential name.
        /// </summary>
        public string Name
        {
            get => GetString(Columns.Name);
            set => SetValue(Columns.Name, value.ToDefaultValue(DefaultCredential));
        }

        /// <summary>
        /// Gets or sets the login id
        /// </summary>
        public string LoginId
        {
            get => GetString(Columns.LoginId);
            set => SetValue(Columns.LoginId, value);
        }

        /// <summary>
        /// Gets or sets the password
        /// </summary>
        public string Password
        {
            get => GetString(Columns.Password);
            set => SetValue(Columns.Password, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialRow"/> class
        /// </summary>
        /// <param name="row">The data row</param>
        public CredentialRow(DataRow row) : base(row)
        {
        }

        /// <summary>
        /// Creates a new <see cref="CredentialRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new row, or null.</returns>
        public static CredentialRow Create(DataRow row)
        {
            return row != null ? new CredentialRow(row) : null;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return Name;
        }

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
        #endregion
    }
}