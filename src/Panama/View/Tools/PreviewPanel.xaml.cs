/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Restless.Panama.View
{
    public partial class PreviewPanel : Grid
    {
        public PreviewPanel()
        {
            InitializeComponent();
        }


        /************************************************************************/
        #region PreviewMode
        /// <summary>
        /// Gets or sets the preview mode
        /// </summary>
        public PreviewMode PreviewMode
        {
            get => (PreviewMode)GetValue(PreviewModeProperty);
            set => SetValue(PreviewModeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PreviewMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PreviewModeProperty = DependencyProperty.Register
            (
                nameof(PreviewMode), typeof(PreviewMode), typeof(PreviewPanel), new FrameworkPropertyMetadata()
                {
                    DefaultValue = PreviewMode.None,
                    PropertyChangedCallback = OnPreviewModeChanged
                }
            );

        private static void OnPreviewModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PreviewPanel)?.SetPreviewVisibility();
        }
        #endregion

        /************************************************************************/

        #region Source data
        /// <summary>
        /// Gets or sets the preview text
        /// </summary>
        public string PreviewText
        {
            get => (string)GetValue(PreviewTextProperty);
            set => SetValue(PreviewTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PreviewText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PreviewTextProperty = DependencyProperty.Register
            (
                nameof(PreviewText), typeof(string), typeof(PreviewPanel), new FrameworkPropertyMetadata()
            );

        /// <summary>
        /// Gets or sets the image source for image preview
        /// </summary>
        public ImageSource PreviewImageSource
        {
            get => (ImageSource)GetValue(PreviewImageSourceProperty);
            set => SetValue(PreviewImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PreviewImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PreviewImageSourceProperty = DependencyProperty.Register
            (
                nameof(PreviewImageSource), typeof(ImageSource), typeof(PreviewPanel), new FrameworkPropertyMetadata()
            );

        #endregion

        /************************************************************************/

        #region Visibility
        /// <summary>
        /// Gets or sets the visibility for the header
        /// </summary>
        public Visibility HeaderVisibility
        {
            get => (Visibility)GetValue(HeaderVisibilityProperty);
            set => SetValue(HeaderVisibilityProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="HeaderVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderVisibilityProperty = DependencyProperty.Register
            (
                nameof(HeaderVisibility), typeof(Visibility), typeof(PreviewPanel), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Visibility.Visible
                }
            );

        /// <summary>
        /// Gets the visibility of the text previewer
        /// </summary>
        public Visibility TextPreviewVisibility
        {
            get => (Visibility)GetValue(TextPreviewVisibilityProperty);
            private set => SetValue(TextPreviewVisibilityPropertyKey, value);
        }

        private static readonly DependencyPropertyKey TextPreviewVisibilityPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(TextPreviewVisibility), typeof(Visibility), typeof(PreviewPanel), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Visibility.Collapsed
                }
            );

        /// <summary>
        /// Identifies the <see cref="TextPreviewVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextPreviewVisibilityProperty = TextPreviewVisibilityPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the visibility of the image previewer
        /// </summary>
        public Visibility ImagePreviewVisibility
        {
            get => (Visibility)GetValue(ImagePreviewVisibilityProperty);
            private set => SetValue(ImagePreviewVisibilityPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ImagePreviewVisibilityPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ImagePreviewVisibility), typeof(Visibility), typeof(PreviewPanel), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Visibility.Collapsed
                }
            );

        /// <summary>
        /// Identifies the <see cref="ImagePreviewVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ImagePreviewVisibilityProperty = ImagePreviewVisibilityPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the visibility for the unsupported notice
        /// </summary>
        public Visibility UnsupportedVisibility
        {
            get => (Visibility)GetValue(UnsupportedVisibilityProperty);
            private set => SetValue(UnsupportedVisibilityPropertyKey, value);
        }

        private static readonly DependencyPropertyKey UnsupportedVisibilityPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(UnsupportedVisibility), typeof(Visibility), typeof(PreviewPanel), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Visibility.Collapsed
                }
            );

        /// <summary>
        /// Identifies the <see cref="UnsupportedVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UnsupportedVisibilityProperty = UnsupportedVisibilityPropertyKey.DependencyProperty;
        #endregion

        private void SetPreviewVisibility()
        {
            TextPreviewVisibility = PreviewMode == PreviewMode.Text ? Visibility.Visible : Visibility.Collapsed;
            ImagePreviewVisibility = PreviewMode == PreviewMode.Image ? Visibility.Visible : Visibility.Collapsed;
            UnsupportedVisibility = PreviewMode == PreviewMode.Unsupported ? Visibility.Visible : Visibility.Collapsed;

        }
    }
}