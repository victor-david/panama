/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Microsoft.WindowsAPICodePack.Dialogs;
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.OpenXml;
using Restless.Toolkit.Core.Utility;
using Restless.Toolkit.Mvvm;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using TableColumns = Restless.Panama.Database.Tables.SubmissionDocumentTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Submission document controller. Handles documents related to a submission
    /// </summary>
    public class SubmissionDocumentController : BaseController<SubmissionViewModel, SubmissionDocumentTable>
    {
        #region Private
        private SubmissionDocumentRow selectedDocument;
        private PreviewMode previewMode;
        private bool isPreviewMode;
        //private bool isOpenXml;
        private string previewText;
        #endregion

        /************************************************************************/

        #region Properties
        private AuthorTable AuthorTable => DatabaseController.Instance.GetTable<AuthorTable>();
        private DocumentTypeTable DocumentTypeTable => DatabaseController.Instance.GetTable<DocumentTypeTable>();

        /// <inheritdoc/>
        public override bool AddCommandEnabled => !(Owner.SelectedBatch?.IsLocked ?? true);

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => !(Owner.SelectedBatch?.IsLocked ?? true);

        /// <summary>
        /// Gets the selected document
        /// </summary>
        public SubmissionDocumentRow SelectedDocument
        {
            get => selectedDocument;
            private set => SetProperty(ref selectedDocument, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates if the controller is in document preview mode.
        /// </summary>
        public bool IsPreviewMode
        {
            get => isPreviewMode;
            set
            {
                SetProperty(ref isPreviewMode, value);
                PrepareDocumentPreview();
            }
        }

        /// <summary>
        /// Gets the preview mode
        /// </summary>
        public PreviewMode PreviewMode
        {
            get => previewMode;
            private set => SetProperty(ref previewMode, value);
        }

        /// <summary>
        /// Gets the preview text for the selected document.
        /// </summary>
        public string PreviewText
        {
            get => previewText;
            private set => SetProperty(ref previewText, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionDocumentController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public SubmissionDocumentController(SubmissionViewModel owner) : base(owner)
        {
            Columns.CreateImage<Int64ToPathConverter>("T", TableColumns.DocType, "ImageFileType", 20.0);
            Columns.Create("Updated", TableColumns.Updated).MakeDate();
            Columns.SetDefaultSort(Columns.Create("Title", TableColumns.Title), ListSortDirection.Ascending);
            Columns.Create("Id", TableColumns.DocId);

            MenuItems.AddItem(Strings.MenuItemAddDocumentToSubmission, AddCommand)
                .AddIconResource(ResourceKeys.Icon.PlusIconKey);

            MenuItems.AddSeparator();

            MenuItems.AddItem(
                Strings.MenuItemReplaceSubmissionDocument,
                RelayCommand.Create(RunReplaceDocumentCommand, p => AddCommandEnabled && SelectedDocument != null))
                .AddIconResource(ResourceKeys.Icon.SubmissionMediumIconKey);

            MenuItems.AddSeparator();

            MenuItems.AddItem(Strings.MenuItemRemoveSubmissionDocument, DeleteCommand)
                .AddIconResource(ResourceKeys.Icon.XMediumIconKey);

            ListView.IsLiveSorting = true;
            ListView.LiveSortingProperties.Add(TableColumns.Title);
        }
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedDocument = SubmissionDocumentRow.Create(SelectedRow);
            PrepareDocumentPreview();
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareString(item1, item2, TableColumns.Title);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return (long)item[TableColumns.BatchId] == (Owner?.SelectedBatch?.Id ?? 0);
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            ListView.Refresh();
        }

        /// <inheritdoc/>
        protected override void RunAddCommand()
        {
            if (!Directory.Exists(Config.Instance.FolderSubmissionDocument))
            {
                MessageWindow.ShowError(Strings.InvalidOpSubmissionDocumentFolderNotSet);
                return;
            }

            switch (WindowFactory.SubmissionDocumentSelect.Create().GetDocumentCreationType())
            {
                case SubmissionDocumentCreationType.None:
                    break;
                case SubmissionDocumentCreationType.CreateDocX:
                    CreateDocxDocument();
                    break;
                case SubmissionDocumentCreationType.CreatePlaceholder:
                    CreatePlaceholderDocument();
                    break;
                default:
                    break;
            }
        }

        /// <inheritdoc/>
        protected override void RunDeleteCommand()
        {
            if (DeleteCommandEnabled && MessageWindow.ShowYesNo(Strings.ConfirmationDeleteSubmissionDocument))
            {
                DeleteSelectedRow();
            }
        }

        /// <inheritdoc/>
        protected override void RunOpenRowCommand()
        {
            if (SelectedDocument?.HasDocumentId ?? false)
            {
                if (DocumentTypeTable.IsDocTypeSupported(SelectedDocument.DocType))
                {
                    Open.SubmissionDocumentFile(SelectedDocument.DocumentId);
                }
                else
                {
                    MessageWindow.ShowError(Strings.InvalidOpDocumentTypeNotSupported);
                }
            }
            else
            {
                MessageWindow.ShowError(Strings.InvalidOpNoDocumentId);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void CreateDocxDocument()
        {
            Execution.TryCatch(() =>
            {
                SubmissionDocumentOptions ops = Config.Instance.SubmissionDocOptions;

                AssemblyInfo ai = new(AssemblyInfoType.Entry);
                string publisherName = Owner.SelectedBatch.PublisherName;
                string fileName = Path.Combine(Config.Instance.FolderSubmissionDocument, Format.MakeFileName(publisherName));

                OpenXmlDocumentCreator xml = new()
                {
                    Filename = $"{fileName}.docx",
                    TemplateFile = Config.TemplateFile,
                    HeaderText = ProcessPlaceholders(ops.Header),
                    FooterText = ProcessPlaceholders(ops.Footer),
                    HeaderPageNumbers = ops.HeaderPageNumbers,
                    FooterPageNumbers = ops.FooterPageNumbers,
                    Paragraphs = GetParagraphs(ProcessPlaceholders(ops.Text)),
                    Author = AuthorTable.GetDefaultAuthorName(),
                    Description = $"Created by {ai.Title} {ai.Version}",
                    Title = $"Submissions to {publisherName}",
                    Company = ops.Company
                };
                xml.Create();
                Table.AddEntry(Owner.SelectedBatch.Id, Paths.SubmissionDocument.WithoutRoot(xml.Filename));
                ListView.Refresh();
                OpenHelper.OpenFile(xml.Filename);

            }, e => MessageWindow.ShowError(e.Message));
        }

        private void CreatePlaceholderDocument()
        {
            Table.AddEntry(Owner.SelectedBatch.Id);
            ListView.Refresh();
        }

        private void PrepareDocumentPreview()
        {
            PreviewText = null;
            PreviewMode = PreviewMode.Unsupported;

            if (IsPreviewMode && !string.IsNullOrEmpty(SelectedDocument?.DocumentId))
            {
                string fileName = Paths.SubmissionDocument.WithRoot(SelectedDocument.DocumentId);

                if (DocumentPreviewer.GetPreviewMode(fileName) == PreviewMode.Text)
                {
                    PreviewMode = PreviewMode.Text;
                    PreviewText = DocumentPreviewer.GetText(fileName);
                }
            }
        }

        private void RunReplaceDocumentCommand(object o)
        {
            using (CommonOpenFileDialog dialog = CommonDialogFactory.Create(Config.Instance.FolderSubmissionDocument, Strings.CaptionSelectSubmissionDocument))
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    // SubmissionDocumentTable.OnColumnChanged(e) method will update the associated fields: Updated, Size, and DocType
                    SelectedDocument.DocumentId = Paths.SubmissionDocument.WithoutRoot(dialog.FileName);
                    Table.Save();
                }
            }
        }

        private string[] GetParagraphs(string str)
        {
            string[] split = { Environment.NewLine };
            string[] paras = str.Split(split, StringSplitOptions.RemoveEmptyEntries);
            return paras;
        }

        private string ProcessPlaceholders(string str)
        {
            string author = AuthorTable.GetDefaultAuthorName();
            string temp = str.Replace("[publisher]", Owner.SelectedBatch.PublisherName);
            temp = temp.Replace("[author]", author);
            temp = temp.Replace("[date]", DateTime.Now.ToString("D"));
            temp = temp.Replace("[month]", DateTime.Now.ToString("MMMM"));
            temp = temp.Replace("[year]", DateTime.Now.ToString("yyyy"));
            return temp;
        }
        #endregion
    }
}