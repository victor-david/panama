/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
#if DOCX
using Restless.Tools.Utility;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Restless.App.Panama.Tools
{
    /// <summary>
    /// Provides document conversion services. Converts documents from .doc to .docx.
    /// </summary>
    public sealed class DocumentConverter
    {
        #region Public properties
        /// <summary>
        /// Gets the items that are eligible for conversion.
        /// </summary>
        public ObservableCollection<DocumentConversionCandidate> Items
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of candidates that have been converted.
        /// </summary>
        public int ConvertedCount
        {
            get
            {
                int c = 0;
                foreach (var candidate in Items)
                {
                    if (candidate.IsConverted) c++;
                }
                return c;
            }
        }
        #endregion

        /************************************************************************/

        #region Public events
        /// <summary>
        /// Occurs when a single item has completed its conversion.
        /// This event is raised on the UI thread.
        /// </summary>
        public event EventHandler<ConversionCompletedEventArgs> ItemCompleted;

        /// <summary>
        /// Occurs when the entire conversion operation is complete, either all items have been processed or the operation is canceled.
        /// This event is raised on the UI thread.
        /// </summary>
        public event EventHandler ConversionCompleted;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentConverter"/> class.
        /// </summary>
        public DocumentConverter()
        {
            Items = new ObservableCollection<DocumentConversionCandidate>();
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Fills <see cref="Items"/> with eligible candidate from the specified folder.
        /// </summary>
        /// <param name="folder">The folder to look for candidates in.</param>
        public void FindFiles(string folder)
        {
            Validations.ValidateNullEmpty(folder, "FindFiles.Folder");
            Items.Clear();
            foreach (string fileName in Directory.EnumerateFiles(folder, "*", SearchOption.TopDirectoryOnly))
            {
                if (Path.GetExtension(fileName) == ".doc")
                {
                    Items.Add(new DocumentConversionCandidate(fileName));
                }
            }
        }

        /// <summary>
        /// Performs the conversion of the all the files in <see cref="Items"/>.
        /// Conversion occurs on a background thread.
        /// </summary>
        /// <param name="taskId">The task id to use when performing the conversion.</param>
        public void Convert(int taskId)
        {
            TaskManager.Instance.ExecuteTask(AppTaskId.Convert, (token) =>
            {
                foreach (var item in Items)
                {
                    item.Convert();
                    OnItemCompleted(item);
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }
                Restless.Tools.OfficeAutomation.OfficeOperation.Instance.Shutdown();
                OnConversionCompleted();
            }, null, null, true);
        }
        #endregion

        /************************************************************************/

        private void OnItemCompleted(DocumentConversionCandidate item)
        {
            var handler = ItemCompleted;
            if (handler != null)
            {
                var e = new  ConversionCompletedEventArgs(item);
                TaskManager.Instance.DispatchTask(() => { handler(this, e); });
            }
        }

        private void OnConversionCompleted()
        {
            var handler = ConversionCompleted;
            if (handler != null)
            {
                TaskManager.Instance.DispatchTask(() => { handler(this, EventArgs.Empty); });
            }
        }
    }
}
#endif