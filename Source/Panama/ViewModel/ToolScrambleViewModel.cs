using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Utility;
using System.Text;
using System.Text.RegularExpressions;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the scramble tool.
    /// </summary>
    public class ToolScrambleViewModel : WorkspaceViewModel
    {
        #region Private
        private string text;
        private Random rand;
        private const string SearchPattern = @"\w+";
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets or sets the text to be scrambled.
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged("Text");
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that determines if the words within the lines will also be scrambled.
        /// </summary>
        public bool ScrambleWords
        {
            get;
            set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        #pragma warning disable 1591
        public ToolScrambleViewModel()
        {
            DisplayName = Strings.CommandToolScramble;
            MaxCreatable = 1;
            RawCommands.Add("Paste", Paste);
            RawCommands.Add("Begin", Scramble);
            rand = new Random();
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region Protected Methods
        #endregion

        /************************************************************************/
        
        #region Private Methods
        private void Paste(object o)
        {
            if (Clipboard.ContainsText())
            {
                Text = Clipboard.GetText();
            }
        }

        private void Scramble(object o)
        {
            Execution.TryCatch(() =>
                {
                    Text = Scrambled().ToString();
                }, (e) =>
                    {
                        MainViewModel.CreateNotificationMessage(e.Message);
                    });
        }

        private StringBuilder Scrambled()
        {
            StringBuilder result = new StringBuilder(1024);
            if (Text == null) Text = String.Empty;
            string[] lines = Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            int lineCount = lines.Length;
            Validations.ValidateInvalidOperation(lineCount < 4, Strings.InvalidOpNotEnoughTextToScramble);

            List<int> used = new List<int>();

            while (used.Count < lineCount)
            {
                int lineIdx = -1;
                while (lineIdx == -1 || used.Contains(lineIdx))
                {
                    lineIdx = rand.Next(lineCount);
                }
                string line = lines[lineIdx].Trim();

                if (line.Length > 0)
                {
                    if (ScrambleWords) line = ScrambledWords(line);
                    result.AppendLine(line);
                }
                used.Add(lineIdx);
            }
            return result;
        }

        private string ScrambledWords(string line)
        {
            List<string> words = new List<string>();
            MatchCollection matches = Regex.Matches(line, SearchPattern);
            foreach (Match match in matches)
            {
                string word = match.Value.Trim();
                if (word.Length > 1)
                {
                    words.Add(word);
                }
            }

            if (words.Count < 2) { return line; }

            List<int> used = new List<int>();

            while (used.Count < words.Count)
            {
                int wordIdx = -1;
                while (wordIdx == -1 || used.Contains(wordIdx))
                {
                    wordIdx = rand.Next(words.Count);
                }
                used.Add(wordIdx);
            }

            string result = String.Empty;
            foreach (int idx in used)
            {
                result += words[idx] + " ";
            }

            return result.Trim();
        }










        #endregion
    }
}