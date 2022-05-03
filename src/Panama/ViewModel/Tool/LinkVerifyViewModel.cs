using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Network;
using Restless.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TableColumns = Restless.Panama.Database.Tables.LinkVerifyTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    public class LinkVerifyViewModel : DataRowViewModel<LinkVerifyTable>
    {
        #region Private
        private LinkVerifyRow selectedLink;
        private bool operationInProgress;
        private bool isCanceling;
        private CancellationTokenSource tokenSource;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the currently selected author
        /// </summary>
        public LinkVerifyRow SelectedLink
        {
            get => selectedLink;
            private set => SetProperty(ref selectedLink, value);
        }

        /// <summary>
        /// Gets a boolean value that indicates whether the verification operation is in progress
        /// </summary>
        public bool OperationInProgress
        {
            get => operationInProgress;
            private set => SetProperty(ref operationInProgress, value);
        }

        /// <summary>
        /// Gets a boolean value that indicates whether the verification operation is canceling
        /// </summary>
        public bool IsCanceling
        {
            get => isCanceling;
            private set => SetProperty(ref isCanceling, value);
        }
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

            Commands.Add("Refresh", RunRefreshCommand);
            Commands.Add("Verify", RunVerifyCommand);
            Commands.Add("Cancel", RunCancelVerifyCommand);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();
        }

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

        private void RunRefreshCommand(object parm)
        {
            Table.Refresh();
            ListView.Refresh();
        }

        private void RunCancelVerifyCommand(object parm)
        {
            IsCanceling = true;
            tokenSource.Cancel();
        }

        private async void RunVerifyCommand(object parm)
        {
            OperationInProgress = true;
            IsCanceling = false;
            tokenSource = new CancellationTokenSource();
            await Task.WhenAll(EnumerateTasks());
            Table.Save();
            OperationInProgress = false;
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
            Debug.WriteLine("Update Link");
            link.SetScanned().SetStatus(0).SetSize(0).SetStatusText(null).SetError(null);

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

        /// <summary>
        /// Returns a string prefaced with http:// if the string doesn't already have it.
        /// </summary>
        /// <param name="s">The string to check.</param>
        /// <returns>A string with http://</returns>
        private string MakeHttp(string s)
        {
            s = ((!s.Contains("://", System.StringComparison.CurrentCulture)) ? "http://" : string.Empty) + s;
            return s;
        }
    }
}