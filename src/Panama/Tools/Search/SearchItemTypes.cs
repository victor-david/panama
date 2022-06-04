namespace Restless.Panama.Tools
{
    /// <summary>
    /// Provides names of search item types
    /// </summary>
    public static class SearchItemTypes
    {
        /// <summary>
        /// Provides static value for Mapi serach item types.
        /// </summary>
        public static class Mapi
        {
            /// <summary>
            /// Gets the string type for a mapi note.
            /// </summary>
            public const string Note = "MAPI/IPM.Note";

            /// <summary>
            /// Gets the string type for a mapi note that has been read.
            /// </summary>
            public const string NoteRead = "MAPI/IPM.Note.Read";

            /// <summary>
            /// Gets the string type for a Mapi folder
            /// </summary>
            public const string Folder = "MAPI/Folder";
        }
    }
}
