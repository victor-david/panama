using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Network;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using TableColumns = Restless.Panama.Database.Tables.LinkVerifyTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    public class LinkVerifyViewModel : DataRowViewModel<LinkVerifyTable>
    {
        #region Private
        private LinkVerifyRow selectedLink;
        private bool isCanceling;
        private CancellationTokenSource tokenSource;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the currently selected link
        /// </summary>
        public LinkVerifyRow SelectedLink
        {
            get => selectedLink;
            private set => SetProperty(ref selectedLink, value);
        }

        /// <summary>
        /// Gets a boolean value that indicates whether the verification operation is canceling
        /// </summary>
        public bool IsCanceling
        {
            get => isCanceling;
            private set => SetProperty(ref isCanceling, value);
        }

        public ICommand RefreshCommand { get; }
        public ICommand VerifyCommand { get; }
        public ICommand CancelCommand { get; }

        #endregion

        /************************************************************************/

        #region Constuctor
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkVerifyViewModel"/> class
        /// </summary>
        public LinkVerifyViewModel()
        {
            Columns.Create("Id", TableColumns.Id).MakeFixedWidth(FixedWidth.W042);

            Columns.Create("Xid", TableColumns.Xid).MakeFixedWidth(FixedWidth.W042);

            Columns.Create("Source", TableColumns.Source)
                .MakeFixedWidth(FixedWidth.W096)
                .MakeInitialSortAscending();

            Columns.Create("Url", TableColumns.Url);

            Columns.Create("Scanned", TableColumns.Scanned)
                .MakeDate();

            Columns.Create("Status", TableColumns.Status)
                .MakeFixedWidth(FixedWidth.W076);

            Columns.Create("Text", TableColumns.StatusText);

            Columns.Create("Size", TableColumns.Size)
                .MakeFixedWidth(FixedWidth.W064);

            RefreshCommand = RelayCommand.Create(p => RunRefreshCommand());
            VerifyCommand = RelayCommand.Create(p => RunVerifyCommand());
            CancelCommand = RelayCommand.Create(p => RunCancelVerifyCommand());

            //Commands.Add("Refresh", RunRefreshCommand);
            //Commands.Add("Verify", RunVerifyCommand);
            //Commands.Add("Cancel", RunCancelVerifyCommand);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedLink = LinkVerifyRow.Create(SelectedRow);
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareString(item1, item2, TableColumns.Source);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunRefreshCommand()
        {
            Table.Refresh();
            ListView.Refresh();
        }

        private void RunCancelVerifyCommand()
        {
            IsCanceling = true;
            tokenSource?.Cancel();
        }

        private async void RunVerifyCommand()
        {
            IsOperationInProgress = true;
            IsCanceling = false;
            Table.PopulateFromAllSources();
            ListView.Refresh();
            tokenSource = new CancellationTokenSource();
            await Task.WhenAll(EnumerateTasks());
            Table.Save();
            IsOperationInProgress = false;
            IsCanceling = false;
        }

        private IEnumerable<Task> EnumerateTasks()
        {
            foreach (LinkVerifyRow link in Table.EnumerateAll())
            {
                Task task = Task.Run(async () =>
                {
                    NetworkResponse response = await NetworkManager.Instance.GetHttpAsync(MakeHttp(link.Url), tokenSource.Token);

                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        UpdateLink(link, response);
                    }));

                });

                yield return task;
            }
        }

        private void UpdateLink(LinkVerifyRow link, NetworkResponse response)
        {
            if (response.Exception is not OperationCanceledException)
            {
                link.SetScanned().ClearAll();

                if (response.IsFaulted)
                {
                    link.SetStatus(-1).SetStatusText("Error").SetError(response.Exception);
                }
                else if (response.IsSuccess)
                {
                    link.SetStatus((long)response.HttpResponse.StatusCode)
                        .SetStatusText(response.HttpResponse.ReasonPhrase)
                        .SetSize(response.ResponseBody.Length);
                }
                else
                {
                    link.SetStatus((long)response.HttpResponse.StatusCode)
                        .SetStatusText(response.HttpResponse.ReasonPhrase)
                        .SetSize(0);
                }
            }
        }

        /// <summary>
        /// Returns a string prefaced with http:// if the string doesn't already have it.
        /// </summary>
        /// <param name="s">The string to check.</param>
        /// <returns>A string with http://</returns>
        private string MakeHttp(string s)
        {
            s = ((!s.Contains("://", StringComparison.CurrentCulture)) ? "http://" : string.Empty) + s;
            return s;
        }
        #endregion
    }
}