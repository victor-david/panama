namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents options that are used to control the behavior of the MessageSelectWindowViewModel class
    /// </summary>
    public class MessageSelectOptions
    {
        /// <summary>
        /// Gets the message selection mode.
        /// </summary>
        public MessageSelectMode Mode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the scope for the message selection
        /// </summary>
        public string Scope
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSelectOptions"/> class.
        /// </summary>
        /// <param name="mode">The message selection mode</param>
        /// <param name="scope">The scope. May be a file or a mapi scope.</param>
        public MessageSelectOptions(MessageSelectMode mode, string scope)
        {
            Mode = mode;
            Scope = scope;
        }
    }
}