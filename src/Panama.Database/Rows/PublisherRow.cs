using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.PublisherTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="PublisherTable"/>.
    /// </summary>
    public class PublisherRow : RowObjectBase<PublisherTable>
    {
        #region Public properties
        /// <summary>
        /// Gets the default name value
        /// </summary>
        public const string DefaultName = "(none)";

        /// <summary>
        /// Gets the id.
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets or sets the publisher name
        /// </summary>
        public string Name
        {
            get => GetString(Columns.Name);
            set => SetValue(Columns.Name, value.Trim().ToDefaultValue(DefaultName));
        }

        /// <summary>
        /// Gets or sets publisher address 1
        /// </summary>
        public string Address1
        {
            get => GetString(Columns.Address1);
            set => SetValue(Columns.Address1, value);
        }

        /// <summary>
        /// Gets or sets publisher address 2
        /// </summary>
        public string Address2
        {
            get => GetString(Columns.Address2);
            set => SetValue(Columns.Address2, value);
        }

        /// <summary>
        /// Gets or sets publisher address 3
        /// </summary>
        public string Address3
        {
            get => GetString(Columns.Address3);
            set => SetValue(Columns.Address3, value);
        }

        /// <summary>
        /// Gets or sets publisher city
        /// </summary>
        public string City
        {
            get => GetString(Columns.City);
            set => SetValue(Columns.City, value);
        }

        /// <summary>
        /// Gets or sets publisher state
        /// </summary>
        public string State
        {
            get => GetString(Columns.State);
            set => SetValue(Columns.State, value);
        }

        /// <summary>
        /// Gets or sets publisher zip
        /// </summary>
        public string Zip
        {
            get => GetString(Columns.Zip);
            set => SetValue(Columns.Zip, value);
        }

        /// <summary>
        /// Gets or sets publisher url
        /// </summary>
        public string Url
        {
            get => GetString(Columns.Url);
            set => SetValue(Columns.Url, value);
        }

        /// <summary>
        /// Gets or sets publisher email
        /// </summary>
        public string Email
        {
            get => GetString(Columns.Email);
            set => SetValue(Columns.Email, value);
        }

        /// <summary>
        /// Gets or sets publisher options
        /// </summary>
        public long Options
        {
            get => GetInt64(Columns.Options);
            set => SetValue(Columns.Options, value);
        }

        /// <summary>
        /// Gets or sets the exclusive status, i.e. the publisher does not accept simultaneous submissions.
        /// </summary>
        public bool Exclusive
        {
            get => GetBoolean(Columns.Exclusive);
            set => SetValue(Columns.Exclusive, value);
        }

        /// <summary>
        /// Gets or sets whether the publisher is a paying market
        /// </summary>
        public bool Paying
        {
            get => GetBoolean(Columns.Paying);
            set => SetValue(Columns.Paying, value);
        }

        /// <summary>
        /// Gets or sets whether the publisher is flagged for follow up
        /// </summary>
        public bool FollowUp
        {
            get => GetBoolean(Columns.Followup);
            set => SetValue(Columns.Followup, value);
        }

        /// <summary>
        /// Gets or sets whether the publisher is a goner
        /// </summary>
        public bool Goner
        {
            get => GetBoolean(Columns.Goner);
            set => SetValue(Columns.Goner, value);
        }

        /// <summary>
        /// Gets or sets the credential id associated with this publisher
        /// </summary>
        public long CredentialId
        {
            get => GetInt64(Columns.CredentialId);
            set => SetValue(Columns.CredentialId, value);
        }

        /// <summary>
        /// Gets or sets publisher notes
        /// </summary>
        public string Notes
        {
            get => GetString(Columns.Notes);
            set => SetValue(Columns.Notes, value);
        }

        /// <summary>
        /// Gets the date / time publisher added
        /// </summary>
        public DateTime Added => GetDateTime(Columns.Added);
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherRow"/> class.
        /// </summary>
        /// <param name="row">The data row</param>
        public PublisherRow(DataRow row) : base(row)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Creates a new <see cref="PublisherRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new tag row, or null.</returns>
        public static PublisherRow Create(DataRow row)
        {
            return row != null ? new PublisherRow(row) : null;
        }

        /// <summary>
        /// Gets a boolean value that indicates if <see cref="Url"/> is populated.
        /// </summary>
        /// <returns></returns>
        public bool HasUrl()
        {
            return !string.IsNullOrEmpty(Url);
        }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return $"{nameof(PublisherRow)} {Id} {Name}";
        }
        #endregion
    }
}