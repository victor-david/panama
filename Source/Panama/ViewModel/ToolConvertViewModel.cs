﻿using Microsoft.WindowsAPICodePack.Dialogs;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Resources;
using Restless.App.Panama.Tools;
using Restless.Tools.Utility;
using System;
using System.ComponentModel;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the .doc to .docx file conversion tool.
    /// </summary>
    public class ToolConvertViewModel : DataGridViewModelBase
    {
#if DOCX
        #region Private
        private string selectedFolder;
        private bool isReadyToRun;
        private bool isRunning;
        private string foundHeader;
        #endregion

        /************************************************************************/

        #region Properties

        /// <summary>
        /// Gets the document converter object.
        /// </summary>
        public DocumentConverter Converter
        {
            get;
        }

        /// <summary>
        /// Gets a string value for the results header.
        /// </summary>
        public string FoundHeader
        {
            get => foundHeader;
            private set => SetProperty(ref foundHeader, value);
        }

        /// <summary>
        /// Gets or sets the selected folder
        /// </summary>
        public string SelectedFolder
        {
            get => selectedFolder;
            set => SetProperty(ref selectedFolder, value);
        }

        /// <summary>
        /// Gets a boolean value that indicates if the conversion process is ready to run.
        /// </summary>
        public bool IsReadyToRun
        {
            get => isReadyToRun;
            private set => SetProperty(ref isReadyToRun, value);
        }

        /// <summary>
        /// Gets a boolean value that indicates if the conversion process is running.
        /// </summary>
        public bool IsRunning
        {
            get => isRunning;
            private set => SetProperty(ref isRunning, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolConvertViewModel"/> class.
        /// </summary>
        public ToolConvertViewModel()
        {
            DisplayName = Strings.CommandToolConvert;
            MaxCreatable = 1;
            Converter = new DocumentConverter();
            Converter.ItemCompleted += ConverterItemCompleted;
            Converter.ConversionCompleted += ConverterConversionCompleted;

            MainSource.Source = Converter.Items;

            Columns.CreateImage<BooleanToImageConverter>("V", "IsVersion");
            Columns.Create("Id", "TitleId").MakeFixedWidth(FixedWidth.Standard);
            Columns.CreateImage<BooleanToImageConverter>("S", "IsSubmissionDoc");
            Columns.CreateImage<BooleanToImageConverter>("C", "Result.Success");
            //Columns.Create("Id", "TitleId").MakeFixedWidth(FixedWidth.Standard);
            
            Columns.Create("Created", "Info.CreationTimeUtc").MakeDate();
            Columns.Create("Modified", "Info.LastWriteTimeUtc").MakeDate();
            Columns.SetDefaultSort(Columns.Create("File name", "Info.FullName").MakeFlexWidth(2.0), ListSortDirection.Ascending);
            Columns.Create("Message", "Result.Message");
            
            Commands.Add("SelectFolder", RunSelectFolderCommand, CanRunSelectFolderCommand);
            Commands.Add("Convert", RunConvertCommand);
            Commands.Add("Cancel", RunCancelCommand);
            UpdateFoundHeader();
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Raises the Closing event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = TaskManager.Instance.WaitForAllRegisteredTasks(() =>
            {
                MainViewModel.CreateNotificationMessage(Strings.NotificationCannotExitTasksAreRunning);
                System.Media.SystemSounds.Beep.Play();

            }, null);
            base.OnClosing(e);
        }
        #endregion

        /************************************************************************/
        
        #region Private Methods
        private void UpdateFoundHeader()
        {
            FoundHeader = string.Format(Strings.HeaderToolOperationConvertFoundFormat, Converter.Items.Count, Converter.ConvertedCount);
        }

        private void RunSelectFolderCommand(object o)
        {
            using (var dialog = CommonDialogFactory.Create(Config.FolderTitleRoot, "Select a directory", true))
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    SelectedFolder = dialog.FileName;
                    Converter.FindFiles(SelectedFolder);
                    UpdateFoundHeader();
                    SetIsReadyToRun();
                }
            }
        }

        private bool CanRunSelectFolderCommand(object o)
        {
            return true;
        }

        private void RunConvertCommand(object o)
        {
            IsRunning = true;
            IsReadyToRun = false;
            Converter.Convert(AppTaskId.Convert);
        }

        private void RunCancelCommand(object o)
        {
            if (IsRunning)
            {
                TaskManager.Instance.CancelTask(AppTaskId.Convert);
            }
        }

        private void ConverterItemCompleted(object sender, ConversionCompletedEventArgs e)
        {
            UpdateFoundHeader();
            e.Item.SignalPropertyUpdates();
        }

        private void ConverterConversionCompleted(object sender, EventArgs e)
        {
            SetIsReadyToRun();
            IsRunning = false;
        }

        private void SetIsReadyToRun()
        {
            IsReadyToRun = Converter.ConvertedCount < Converter.Items.Count;
        }
        #endregion
#endif
    }
}