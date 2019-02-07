/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Microsoft.WindowsAPICodePack.Dialogs;
using Restless.App.Panama.Resources;
using Restless.App.Panama.ViewModel;
using Restless.Tools.Utility.Search;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using SysProps = Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Interaction logic for PathSelector.xaml
    /// </summary>
    public partial class PathSelector : UserControl
    {
        #region Public properties
        /// <summary>
        /// Gets or sets the title of this control
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Dependency property definition for the <see cref="Title"/> property
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register
            (
                nameof(Title), typeof(string), typeof(PathSelector), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the name of the file or folder.
        /// </summary>
        public string PathName
        {
            get => (string)GetValue(PathNameProperty);
            set => SetValue(PathNameProperty, value);
        }

        /// <summary>
        /// Dependency property definition for the <see cref="PathName"/> property.
        /// </summary>
        public static readonly DependencyProperty PathNameProperty = DependencyProperty.Register
            (
                nameof(PathName), typeof(string), typeof(PathSelector), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
            );


        /// <summary>
        /// Gets or the sets the editing type for this control.
        /// </summary>
        public PathSelectorType SelectorType
        {
            get => (PathSelectorType)GetValue(SelectorTypeProperty);
            set => SetValue(SelectorTypeProperty, value);
        }

        /// <summary>
        /// Dependency property definition for the <see cref="SelectorType"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectorTypeProperty = DependencyProperty.Register
            (
                nameof(SelectorType), typeof(PathSelectorType), typeof(PathSelector), new PropertyMetadata(PathSelectorType.FileSystemFolder)
            );

        /// <summary>
        /// Gets or sets a value that determines which file types are included in the dialog selector.
        /// The default is zero which includes all file types.
        /// </summary>
        public long SelectorFileType
        {
            get => (long)GetValue(SelectorFileTypeProperty);
            set => SetValue(SelectorFileTypeProperty, value);
        }

        /// <summary>
        /// Dependency property definition for the <see cref="SelectorFileType"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectorFileTypeProperty = DependencyProperty.Register
            (
                nameof(SelectorFileType), typeof(long), typeof(PathSelector), new PropertyMetadata(default(long))
            );

        /// <summary>
        /// Gets or sets a boolean value that determines if the clear functionality is enabled.
        /// </summary>
        public bool ClearEnabled
        {
            get => (bool)GetValue(ClearEnabledProperty);
            set => SetValue(ClearEnabledProperty, value);
        }

        /// <summary>
        /// Dependency property definition for the <see cref="ClearEnabled"/> property.
        /// </summary>
        public static readonly DependencyProperty ClearEnabledProperty = DependencyProperty.Register
            (
                nameof(ClearEnabled), typeof(bool), typeof(PathSelector), new PropertyMetadata(false, ClearEnabledPropertyChanged)
            );

        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PathSelector"/> class.
        /// </summary>
        public PathSelector()
        {
            InitializeComponent();
            SetClearButtonVisibility();
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        private void ButtonChangeClick(object sender, RoutedEventArgs e)
        {
            switch (SelectorType)
            {
                case PathSelectorType.FileSystemFolder:
                case PathSelectorType.FileSystemFile:
                    SelectFileSystem();
                    break;
                case PathSelectorType.Mapi:
                    SelectMapiFolder();
                    break;
            }
        }

        private void ButtonClearClick(object sender, RoutedEventArgs e)
        {
            PathName = null;
        }

        private void SelectFileSystem()
        {
            string title = (SelectorType == PathSelectorType.FileSystemFolder) ? "Select a directory" : "Select a file";

            string initialDir = string.Empty;

            if (!string.IsNullOrEmpty(PathName))
            {
                initialDir = PathName;
                if (SelectorType == PathSelectorType.FileSystemFile)
                {
                    initialDir = Path.GetDirectoryName(PathName);
                }
            }

            using (var dialog = CommonDialogFactory.Create(initialDir, title, SelectorType == PathSelectorType.FileSystemFolder, SelectorFileType))
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    PathName = dialog.FileName;
                }
            }
        }

        [Obsolete("Mapi selection is deprecated")]
        private void SelectMapiFolder()
        {
            MessageSelectOptions ops = new MessageSelectOptions(MessageSelectMode.Folder, null);
            var w = WindowFactory.MessageSelect.Create(Strings.CaptionSelectMapiFolder, ops);
            w.ShowDialog();

            if (w.GetValue(WindowViewModel.ViewModelProperty) is MessageSelectWindowViewModel vm && vm.SelectedItems != null)
            {
                if (vm.SelectedItems[0] is WindowsSearchResult result)
                {
                    string url = result.Values[SysProps.System.ItemUrl].ToString();
                    // remove "mapi:" from string
                    PathName = url.Remove(0, 5);
                }
            }
        }

        private void SetClearButtonVisibility()
        {
            PART_ButtonClear.Visibility = ClearEnabled ? Visibility.Visible : Visibility.Collapsed;
        }

        private static void ClearEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PathSelector ps)
            {
                ps.SetClearButtonVisibility();
            }
        }
        #endregion
    }
}