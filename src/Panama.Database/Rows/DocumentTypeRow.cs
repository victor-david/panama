using Restless.Toolkit.Core.Database.SQLite;
using System.Data;
using Columns = Restless.Panama.Database.Tables.DocumentTypeTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="DocumentTypeTable"/>.
    /// </summary>
    public class DocumentTypeRow : RowObjectBase<DocumentTypeTable>
    {
        #region Public properties
        /// <summary>
        /// Gets the id for this row object.
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name => GetString(Columns.Name);

        /// <summary>
        /// Gets the extensions
        /// </summary>
        public string Extensions => GetString(Columns.Extensions);

        /// <summary>
        /// Gets the ordering.
        /// </summary>
        public long Ordering => GetInt64(Columns.Ordering);

        /// <summary>
        /// Gets whether the entry is supported.
        /// </summary>
        public bool Supported => GetBoolean(Columns.Supported);
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentTypeRow"/> class.
        /// </summary>
        /// <param name="row">The data row</param>
        public DocumentTypeRow(DataRow row) : base(row)
        {
        }
        #endregion

        /************************************************************************/

        #region Internal / public methods
        /// <summary>
        /// Gets a boolean value that determines whether the specified extension
        /// exists within <see cref="Extensions"/>.
        /// </summary>
        /// <param name="extension">The extension to check.</param>
        /// <returns>true if the extension exists; otherwise, false.</returns>
        /// <remarks>
        /// This method is used by <see cref="DocumentTypeTable"/>
        /// when getting a document type from a file name. It strips
        /// off the leading dot first because extensions are stored without one.
        /// </remarks>
        internal bool ContainsExtension(string extension)
        {
            foreach (string ext in Extensions.Split(';'))
            {
                if (ext == extension)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the string representation of this object.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return Name;
        }
        #endregion


    }
}