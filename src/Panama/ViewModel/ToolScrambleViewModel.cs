/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.Resources;
using Restless.Tools.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the scramble tool.
    /// </summary>
    public class ToolScrambleViewModel : ApplicationViewModel
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
                SetProperty(ref text, value);
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
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolScrambleViewModel"/> class.
        /// </summary>
        /// <param name="owner">The VM that owns this view model.</param>
        public ToolScrambleViewModel(ApplicationViewModel owner) : base (owner)
        {
            DisplayName = Strings.CommandToolScramble;
            MaxCreatable = 1;
            Commands.Add("Paste", Paste);
            Commands.Add("Begin", Scramble);
            rand = new Random();
        }
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
            Execution.TryCatch(() =>
            {
                if (Clipboard.ContainsText())
                {
                    Text = Clipboard.GetText();
                }
            }, (e) =>  MainWindowViewModel.Instance.CreateNotificationMessage(e.Message));
        }

        private void Scramble(object o)
        {
            Execution.TryCatch(() =>
            {
                Text = Scrambled().ToString();
            }, (e) => MainWindowViewModel.Instance.CreateNotificationMessage(e.Message));
        }

        private StringBuilder Scrambled()
        {
            StringBuilder result = new StringBuilder(1024);
            if (Text == null) Text = string.Empty;
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

            string result = string.Empty;
            foreach (int idx in used)
            {
                result += words[idx] + " ";
            }

            return result.Trim();
        }










        #endregion
    }
}