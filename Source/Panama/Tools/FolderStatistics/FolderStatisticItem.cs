using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.Tools.Utility;
using System.IO;
using Restless.App.Panama.Resources;
using System.Diagnostics;

namespace Restless.App.Panama.Tools
{
    /// <summary>
    /// Represents a single folder with associated file statistics.
    /// </summary>
    public class FolderStatisticItem
    {
        #region Public properties
        /// <summary>
        /// Gets the folder associated with the statistics.
        /// </summary>
        public string Folder
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the display name for this item. Returns only the folder name.
        /// </summary>
        public string FolderDisplay
        {
            get { return Path.GetFileName(Folder); }
        }

        /// <summary>
        /// Gets the depth of this folder statistic within the folder hierarchy.
        /// </summary>
        public int Depth
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the total number of files. Includes sub folders.
        /// </summary>
        public int Total
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of .docx files. Includes sub folders.
        /// </summary>
        public int Docx
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of .doc files. Includes sub folders.
        /// </summary>
        public int Doc
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of .pdf files. Includes sub folders.
        /// </summary>
        public int Pdf
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of .txt files. Includes sub folders.
        /// </summary>
        public int Txt
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of other files. Includes sub folders.
        /// </summary>
        public int Other
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the children of this item.
        /// </summary>
        public List<FolderStatisticItem> Children
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/
        
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderStatisticItem"/> class.
        /// </summary>
        /// <param name="folder">The folder for which this set of statistics apply.</param>
        public FolderStatisticItem(string folder)
        {
            Validations.ValidateNullEmpty(folder, "FolderStatisticItem.Folder");
            Validations.ValidateInvalidOperation(!Directory.Exists(folder), Strings.InvalidOpDirectoryDoesNotExist);
            Folder = folder;
            Children = new List<FolderStatisticItem>();
            Depth = 0;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Populates the folder statistics. This method recursively checks sub folders,
        /// creating items for the <see cref="Children"/> property. This method may take a long time to run.
        /// Consider running it on a background thread.
        /// </summary>
        public void Populate()
        {
            Populate(Folder, 0);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void Populate(string folder, int depth)
        {
            Depth = depth;
            foreach (string file in Directory.EnumerateFiles(folder, "*", SearchOption.TopDirectoryOnly))
            {
                Total++;
                if (Path.GetExtension(file) == ".docx") Docx++;
                else if (Path.GetExtension(file) == ".doc") Doc++;
                else if (Path.GetExtension(file) == ".pdf") Pdf++;
                else if (Path.GetExtension(file) == ".txt") Txt++;
                else Other++;
            }

            foreach (string dir in Directory.EnumerateDirectories(folder, "*", SearchOption.TopDirectoryOnly))
            {
                FolderStatisticItem child = new FolderStatisticItem(dir);
                Children.Add(child);
                child.Populate(dir, depth + 1);
            }
            SumChildren(this);
        }

        private void SumChildren(FolderStatisticItem item)
        {
            foreach (FolderStatisticItem child in item.Children)
            {
                SumChildren(child);
                Total += child.Total;
                Docx += child.Docx;
                Doc += child.Doc;
                Pdf += child.Pdf;
                Txt += child.Txt;
                Other += child.Other;
            }

        }
        #endregion
    }
}
