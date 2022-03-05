/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Toolkit.Core.Utility;
using System;
using System.Windows.Media.Imaging;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Extends <see cref="DataGridViewModelBase"/> to provide functionality
    /// for a view model that can preview the selected data grid row.
    /// This class must be inherited.
    /// </summary>
    public abstract class DataGridPreviewViewModel : DataGridViewModelBase
    {
        #region Private
        private PreviewMode previewMode;
        private bool isPreviewActive;
        private string previewText;
        private BitmapImage previewImageSource;
        private int previewImageWidth;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets an enumeration value that indicates the current preview mode.
        /// </summary>
        public PreviewMode PreviewMode
        {
            get { return previewMode; }
            private set
            {
                SetProperty(ref previewMode, value);
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates if preview mode is active.
        /// </summary>
        public bool IsPreviewActive
        {
            get { return isPreviewActive; }
            private set
            {
                if (SetProperty(ref isPreviewActive, value))
                {
                    OnPropertyChanged(nameof(PreviewActiveIcon));
                    OnIsPreviewActiveChanged();
                    PerformPreviewIf();
                }

            }
        }

        /// <summary>
        /// Gets an expand / collapse icon for preview mode.
        /// </summary>
        public object PreviewActiveIcon
        {
            get
            {
                string resource = (isPreviewActive) ? "ImageChevronRightTool" : "ImageChevronLeftTool";
                return Resources.LocalResources.Get(resource);
            }
        }

        /// <summary>
        /// When <see cref="IsPreviewActive"/> is true, gets the preview text for the selected item.
        /// </summary>
        public string PreviewText
        {
            get { return previewText; }
            protected set
            {
                SetProperty(ref previewText, value);
            }
        }

        /// <summary>
        /// Gets or (from a derived class) sets the image source used to preview an image file.
        /// </summary>
        public BitmapImage PreviewImageSource
        {
            get { return previewImageSource; }
            protected set
            {
                SetProperty(ref previewImageSource, value);
            }
        }

        /// <summary>
        /// Gets or (from a derived class) sets the image width.
        /// </summary>
        public int PreviewImageWidth
        {
            get { return previewImageWidth; }
            protected set
            {
                SetProperty(ref previewImageWidth, value);
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridPreviewViewModel"/> class.
        /// </summary>
        /// <param name="owner">The VM that owns this view model.</param>
        public DataGridPreviewViewModel(ApplicationViewModel owner) : base(owner)
        {
            IsPreviewActive = false;
            PreviewMode = PreviewMode.None;
            Commands.Add("TogglePreview", (o) =>
            {
                IsPreviewActive = !IsPreviewActive;
            });
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the selected grid item changes.
        /// If <see cref="IsPreviewActive"/> is true, calls <see cref="OnPreview(object)"/>.
        /// </summary>
        protected override void OnSelectedItemChanged()
        {
            PreviewText = null;
            PreviewImageSource = null;
            PerformPreviewIf();
        }

        /// <summary>
        /// Performs a default preview action. A derived class can use this method
        /// to execute the preview, or use a custom action if necessary.
        /// </summary>
        /// <param name="fileName">The file name as determined by the derived class.</param>
        /// <remarks>
        /// When a preview is ready to be executed, this class calls the <see cref="GetPreviewMode(object)"/>
        /// method to allow a derived class to establish the type of preview, followed by the <see cref="OnPreview(object)"/>
        /// method. A derived class can determine the file name from the selected item and then call this method
        /// to perform the actual preview.
        /// </remarks>
        protected void PerformPreview(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            switch (PreviewMode)
            {
                case PreviewMode.Text:
                    PreviewText = DocumentPreviewer.GetText(fileName);
                    break;
                case PreviewMode.Image:
                    Execution.TryCatchSwallow(() =>
                    {
                        var img = DocumentPreviewer.GetImage(fileName);
                        PreviewImageWidth = img.DecodePixelWidth;
                        PreviewImageSource = img;
                    });
                    break;
            }
        }

        /// <summary>
        /// Called when a preview of the selected item is needed.
        /// Override in a derived class to perform specific preview action.
        /// The base implementation does nothing.
        /// </summary>
        /// <param name="selectedItem">The currently selected grid item.</param>
        protected virtual void OnPreview(object selectedItem)
        {
        }

        /// <summary>
        /// Override in a derived class to perform actions when <see cref="IsPreviewActive"/> changes.
        /// The base implementation does nothing.
        /// </summary>
        protected virtual void OnIsPreviewActiveChanged()
        {
        }

        /// <summary>
        /// Override in a derived class to get the preview mode for the specified item.
        /// The base class returns PreviewMode.None;
        /// </summary>
        /// <param name="selectedItem">The selected grid item</param>
        /// <returns>The preview mode</returns>
        protected virtual PreviewMode GetPreviewMode(object selectedItem)
        {
            return PreviewMode.None;
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void PerformPreviewIf()
        {
            if (IsPreviewActive && SelectedItem != null)
            {
                PreviewMode = GetPreviewMode(SelectedItem);
                OnPreview(SelectedItem);
            }
        }
        #endregion
    }
}