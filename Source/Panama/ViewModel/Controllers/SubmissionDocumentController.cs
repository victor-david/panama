using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.WindowsAPICodePack.Dialogs;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Database.SQLite;
using Restless.Tools.Utility;
using Restless.App.Panama.View;
using Restless.Tools.OpenXml;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Submission document controller. Handles documents related to a submission
    /// </summary>
    public class SubmissionDocumentController : SubmissionController
    {
        #region Private
        private DocumentTypeTable documentTypeTable;
        private bool isPreviewMode;
        private bool isOpenXml;
        private string previewText;
        private string selectedFileName;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets a value that indicates if the controller is in document preview mode.
        /// </summary>
        public bool IsPreviewMode
        {
            get => isPreviewMode;
            set
            {
                isPreviewMode = value;
                DisplayPreviewIf();
            }
        }

        /// <summary>
        /// Gets a value that indicates if the selected document is Open Xml.
        /// </summary>
        public bool IsOpenXml
        {
            get => isOpenXml;
            private set => SetProperty(ref isOpenXml, value);
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

        #region Protected properties
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionDocumentController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public SubmissionDocumentController(SubmissionViewModel owner)
            : base(owner)
        {
            AssignDataViewFrom(DatabaseController.Instance.GetTable<SubmissionDocumentTable>());
            DataView.RowFilter = String.Format("{0}=-1", SubmissionDocumentTable.Defs.Columns.BatchId);
            DataView.Sort = SubmissionDocumentTable.Defs.Columns.Title;

            Columns.CreateImage<IntegerToImageConverter>("T", SubmissionDocumentTable.Defs.Columns.DocType, "ImageFileType", 20.0);
            Columns.Create("Updated", SubmissionDocumentTable.Defs.Columns.Updated).MakeDate();
            Columns.SetDefaultSort(Columns.Create("Title", SubmissionDocumentTable.Defs.Columns.Title), ListSortDirection.Ascending);
            Columns.Create("Id", SubmissionDocumentTable.Defs.Columns.DocId);

            documentTypeTable = DatabaseController.Instance.GetTable<DocumentTypeTable>();

            Owner.Commands.Add("DocumentAdd", RunAddDocumentCommand, (o) =>
                {
                    return
                        Owner.SelectedRow != null &&
                        !(bool)Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Locked];
                });

            Owner.Commands.Add("DocumentReplace", RunReplaceDocumentCommand, (o) =>
            {
                return
                    SelectedRow != null &&
                    Owner.SelectedRow != null &&
                    !(bool)Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Locked];
            });

            Owner.Commands.Add("DocumentRemove", RunRemoveDocumentCommand, (o) =>
                {
                    return
                        SelectedRow != null &&
                        Owner.SelectedRow != null &&
                        !(bool)Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Locked];
                });
            HeaderPreface = Strings.HeaderDocuments;

        }
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called by the <see cref=" ControllerBase{VM,T}.Owner"/> of this controller
        /// in order to update the controller values.
        /// </summary>
        protected override void OnUpdate()
        {
            Int64 id = GetOwnerSelectedPrimaryId();
            DataView.RowFilter = String.Format("{0}={1}", SubmissionDocumentTable.Defs.Columns.BatchId, id);
        }

        /// <summary>
        /// Called when the selected grid item changes
        /// </summary>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            IsOpenXml = false;
            var row = SelectedRow;
            if (row != null)
            {
                selectedFileName = row[SubmissionDocumentTable.Defs.Columns.DocId].ToString();
                IsOpenXml = documentTypeTable.GetDocTypeFromFileName(selectedFileName) == DocumentTypeTable.Defs.Values.WordOpenXmlFileType;
                DisplayPreviewIf();
            }
        }

        /// <summary>
        /// Runs the <see cref="DataGridViewModel{T}.OpenRowCommand"/> to open the selected document.
        /// </summary>
        /// <param name="item">The <see cref="DataRowView"/> object of the selected row.</param>
        protected override void RunOpenRowCommand(object item)
        {
            if (item is DataRowView view)
            {
                string docid = view.Row[SubmissionDocumentTable.Defs.Columns.DocId].ToString();
                if (String.IsNullOrEmpty(docid))
                {
                    Messages.ShowError(Strings.InvalidOpNoDocumentId);
                    return;
                }

                Int64 docType = (Int64)view.Row[SubmissionDocumentTable.Defs.Columns.DocType];
                if (!documentTypeTable.IsDocTypeSupported(docType))
                {
                    Messages.ShowError(Strings.InvalidOpDocumentTypeNotSupported);
                    return;
                }

                OpenFileRow(view.Row, SubmissionDocumentTable.Defs.Columns.DocId, Config.Instance.FolderSubmissionDocument, (f) =>
                    {
                        Messages.ShowError(String.Format(Strings.FormatStringFileNotFound, f, "FolderSubmissionDocument"));
                    });
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods

        private void DisplayPreviewIf()
        {
            if (IsOpenXml && IsPreviewMode)
            {
                string fileName = Paths.SubmissionDocument.WithRoot(selectedFileName);
                Execution.TryCatch(() =>
                {
                    PreviewText = OpenXmlDocument.Reader.GetText(fileName);
                }, 
                (ex) => { MainViewModel.CreateNotificationMessage(ex.Message); });
            }
        }

        private void RunAddDocumentCommand(object o)
        {
            if (!Directory.Exists(Config.Instance.FolderSubmissionDocument))
            {
                Messages.ShowError(Strings.InvalidOpSubmissionDocumentFolderNotSet);
                return;
            }

            Int64 batchId = (Int64)Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Id];
            string publisher = Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Joined.Publisher].ToString();

            var select = WindowFactory.SubmissionDocumentSelect.Create();
            select.ShowDialog();

            if (select.GetValue(WindowViewModel.ViewModelProperty) is SubmissionDocumentSelectWindowViewModel vm)
            {
                switch (vm.CreateType)
                {
                    case SubmissionDocumentCreateType.CreateDocX:
                        Execution.TryCatch(() =>
                        {
                            var ops = Config.Instance.SubmissionDocOptions;

                            var ai = new AssemblyInfo(AssemblyInfoType.Entry);
                            var xml = new Restless.Tools.OpenXml.OpenXmlDocumentCreator()
                            {
                                Filename = String.Format("{0}.docx", Path.Combine(Config.Instance.FolderSubmissionDocument, Format.MakeFileName(publisher))),
                                HeaderText = ProcessPlaceholders(ops.Header),
                                FooterText = ProcessPlaceholders(ops.Footer),
                                HeaderPageNumbers = ops.HeaderPageNumbers,
                                FooterPageNumbers = ops.FooterPageNumbers,
                                Paragraphs = GetParagraphs(ProcessPlaceholders(ops.Text)),
                                Author = DatabaseController.Instance.GetTable<AuthorTable>().GetDefaultAuthorName(),
                                Description = String.Format("Created by {0} {1}", ai.Title, ai.Version),
                                Title = String.Format("Submissions to {0}", publisher),
                                Company = ops.Company
                            };
                            xml.Create();
                            DatabaseController.Instance.GetTable<SubmissionDocumentTable>().AddEntry(batchId, Paths.SubmissionDocument.WithoutRoot(xml.Filename));
                            OpenHelper.OpenFile(xml.Filename);
                        });
                        break;

                    case SubmissionDocumentCreateType.CreatePlaceholder:
                        DatabaseController.Instance.GetTable<SubmissionDocumentTable>().AddEntry(batchId);
                        break;

                    case SubmissionDocumentCreateType.None:
                        break;
                }
            }
        }

        private void RunReplaceDocumentCommand(object o)
        {
            using (var dialog = CommonDialogFactory.Create(Config.Instance.FolderSubmissionDocument, Strings.CaptionSelectSubmissionDocument))
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    // SubmissionDocumentTable.OnColumnChanged(e) method will update the associated fields: Updated, Size, and DocType
                    SelectedRow[SubmissionDocumentTable.Defs.Columns.DocId] = Paths.SubmissionDocument.WithoutRoot(dialog.FileName);
                    DatabaseController.Instance.GetTable<SubmissionDocumentTable>().Save();

                }
            }
        }

        private void RunRemoveDocumentCommand(object o)
        {
            if (SelectedRow != null && Messages.ShowYesNo(Strings.ConfirmationDeleteSubmissionDocument))
            {
                SelectedRow.Delete();
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
            string author = DatabaseController.Instance.GetTable<AuthorTable>().GetDefaultAuthorName();
            string temp = str.Replace("[publisher]", Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Joined.Publisher].ToString());
            temp = temp.Replace("[author]", author);
            temp = temp.Replace("[date]", DateTime.Now.ToString("D"));
            temp = temp.Replace("[month]", DateTime.Now.ToString("MMMM"));
            temp = temp.Replace("[year]", DateTime.Now.ToString("yyyy"));
            return temp;
        }
        #endregion

    }
}
