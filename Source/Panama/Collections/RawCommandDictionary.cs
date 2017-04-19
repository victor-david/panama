using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.Tools.Utility;

namespace Restless.App.Panama.Collections
{
    /// <summary>
    /// Represents a dictionary of commands.
    /// </summary>
    /// <remarks>
    /// A RawCommandDictionary collection is used by the various view models and associated controllers
    /// to create commands without the need to declare a separate property for each one.
    /// </remarks>
    public class RawCommandDictionary
    {
        #region Private
        private Dictionary<string, RelayCommand> storage;
        #endregion

        /************************************************************************/
        
        #region Public properties
        /// <summary>
        /// Acceses the dictionary value according to the string key
        /// </summary>
        /// <param name="key">The string key</param>
        /// <returns>The RelayCommand object, or null if not present</returns>
        public RelayCommand this [string key]
        {
            get 
            {
                if (storage.ContainsKey(key))
                {
                    return storage[key];
                }
                return null;
            }
        }

        #endregion
        
        /************************************************************************/

        #region Constructor
        #pragma warning disable 1591
        public RawCommandDictionary()
        {
            storage = new Dictionary<string, RelayCommand>();
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Adds a command to the dictionary.
        /// </summary>
        /// <param name="key">The name of the command in the dictionary</param>
        /// <param name="command">The RelayCommand object</param>
        public void Add(string key, RelayCommand command)
        {
            Validations.ValidateNullEmpty(key, "Add.Key");
            Validations.ValidateNull(command, "Add.Command");
            storage.Add(key, command);
        }

        /// <summary>
        /// Adds a command to the dictionary.
        /// </summary>
        /// <param name="key">The name of the command in the dictionary</param>
        /// <param name="runCommand">The action to run the command</param>
        /// <param name="canRunCommand">The predicate to determine if the command can run, or null if it can always run</param>
        public void Add(string key, Action<object> runCommand, Predicate<object> canRunCommand)
        {
            Add(key, new RelayCommand(runCommand, canRunCommand));
        }

        /// <summary>
        /// Adds a command without a predicate.
        /// </summary>
        /// <param name="key">The name of the command in the dictionary</param>
        /// <param name="runCommand">The action to run the command</param>
        public void Add(string key, Action<object> runCommand)
        {
            Add(key, runCommand, null);
        }
        #endregion
    }
}
