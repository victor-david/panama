using Restless.Panama.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Provides collections used for data display by the tools window.
    /// </summary>
    public class ToolResultData
    {
        /// <summary>
        /// Gets an array of collections for updated items
        /// </summary>
        public ObservableCollection<FileScanItem>[] Updated
        {
            get;
        }

        /// <summary>
        /// Gets an array of collections for not found items
        /// </summary>
        public ObservableCollection<FileScanItem>[] NotFound
        {
            get;
        }

        /// <summary>
        /// Gets the collection of status messages
        /// </summary>
        public ObservableCollection<string> Status
        {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolResultData"/> class
        /// </summary>
        /// <param name="size">The size of the collections</param>
        public ToolResultData(int size)
        {
            Updated = new ObservableCollection<FileScanItem>[size];
            NotFound = new ObservableCollection<FileScanItem>[size];
            Status = new ObservableCollection<string>();

            for (int idx = 0; idx < size; idx++)
            {
                Updated[idx] = new ObservableCollection<FileScanItem>();
                NotFound[idx] = new ObservableCollection<FileScanItem>();
                Status.Add(null);
            }
        }

        /// <summary>
        /// Clears all data structures for the specified index
        /// </summary>
        /// <param name="index">The index to clear</param>
        public void Clear(int index)
        {
            Updated[index].Clear();
            NotFound[index].Clear();
            Status[index] = null;
        }

        /// <summary>
        /// Adds items to update according to the specified index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="source">The source</param>
        public void AddToUpdate(int index, IEnumerable<FileScanItem> source)
        {
            Throw.IfNull(source);

            foreach (FileScanItem item in source)
            {
                Updated[index].Add(item);
            }
        }

        /// <summary>
        /// Adds items to not found according to the specified index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="source">The source</param>
        public void AddToNotFound(int index, IEnumerable<FileScanItem> source)
        {
            Throw.IfNull(source);

            foreach (FileScanItem item in source)
            {
                NotFound[index].Add(item);
            }
        }

        /// <summary>
        /// Sest status message for the specified index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="status">The status message</param>
        public void SetStatus(int index, string status)
        {
            Status[index] = status;
        }
    }
}
