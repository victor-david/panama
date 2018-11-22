using Restless.Tools.Mvvm;
using Restless.Tools.Utility;
using System;

namespace Restless.App.Panama.Tools
{
    /// <summary>
    /// Represents the base class for a tool that scans files. This class must be inherited.
    /// </summary>
    public abstract class FileScanBase : ObservableObject
    {
        #region Private
        private bool isRunning;
        private int totalCount;
        private int scanCount;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a boolean value that indicates if this scanner is running
        /// </summary>
        public bool IsRunning
        {
            get => isRunning; 
            protected set => SetProperty(ref isRunning, value);
        }

        /// <summary>
        /// Gets the total record count
        /// </summary>
        public int TotalCount
        {
            get => totalCount;
            protected set => SetProperty(ref totalCount, value);
        }

        /// <summary>
        /// Gets the count of scanned files
        /// </summary>
        public int ScanCount
        {
            get => scanCount;
            protected set
            {
                if (SetProperty(ref scanCount, value))
                {
                    OnScanCountChanged();
                }
            }
        }
        #endregion

        /************************************************************************/
        
        #region Constructor
        #pragma warning disable 1591
        public FileScanBase()
        {
            ScanCount = 0;
            TotalCount = int.MaxValue;
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/
        
        #region Public methods
        /// <summary>
        /// Begins execution of the operation on a background thread.
        /// </summary>
        /// <param name="taskId">The unique task id.</param>
        public void Execute(int taskId)
        {
            IsRunning = true;
            ScanCount = 0;
            TaskManager.Instance.ExecuteTask(taskId, (token) =>
                {
                    ExecuteTask();
                    OnCompleted();
                    IsRunning = false;
                }, null, null, false);
        }

        /// <summary>
        /// Begins syncronous execution of the operation.
        /// </summary>
        /// <remarks>
        /// Unlike <see cref="Execute(int)"/>, this method does not start a background thread.
        /// </remarks>
        public void Execute()
        {
            IsRunning = true;
            ScanCount = 0;
            ExecuteTask();
            OnCompleted();
            IsRunning = false;
        }
        #endregion

        /************************************************************************/
        
        #region Events
        /// <summary>
        /// Raised when the scan count changes.
        /// </summary>
        public event EventHandler ScanCountChanged;


        /// <summary>
        /// Raised when a file scan result is updated.
        /// </summary>
        public event EventHandler<FileScanEventArgs> Updated;

        /// <summary>
        /// Raised when a file scan result is not found.
        /// </summary>
        public event EventHandler<FileScanEventArgs> NotFound;

        /// <summary>
        /// Raised when the scan is completed.
        /// </summary>
        public event EventHandler Completed;
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// When implemented in a derived class, performs its operation.
        /// </summary>
        protected abstract void ExecuteTask();

        /// <summary>
        /// Raises the <see cref="Updated"/> event.
        /// </summary>
        /// <param name="item">The item</param>
        protected void OnUpdated(FileScanDisplayObject item)
        {
            Updated?.Invoke(this, new FileScanEventArgs(item));
        }

        /// <summary>
        /// Raises the <see cref="NotFound"/> event.
        /// </summary>
        /// <param name="item">The item</param>
        protected void OnNotFound(FileScanDisplayObject item)
        {
            NotFound?.Invoke(this, new FileScanEventArgs(item));
        }

        /// <summary>
        /// Raises the <see cref="ScanCountChanged"/> event.
        /// </summary>
        protected void OnScanCountChanged()
        {
            ScanCountChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="Completed"/> event.
        /// </summary>
        protected void OnCompleted()
        {
            Completed?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
