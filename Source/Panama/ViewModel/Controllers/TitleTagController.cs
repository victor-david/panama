using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.Tools.Database.SQLite;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Database;
using System.Diagnostics;
using System.Windows.Input;
using System.Data;
using System.Collections.ObjectModel;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides a controller that manages the tags that may be assigned to a title.
    /// </summary>
    public class TitleTagController : TitleController
    {
        #region Private
        private TitleTagTable titleTagTable;
        private TagCacheDictionary tagCache;
        #endregion

        /************************************************************************/
        
        #region Public properties
        /// <summary>
        /// Gets a boolean value that indicates if there are no tags assigned to the associated title.
        /// </summary>
        public bool HasZeroTags
        {
            get { return Current.Count == 0; }
        }

        /// <summary>
        /// Gets the list of currently assigned tags. The edit control binds to this.
        /// </summary>
        public ObservableCollection<TagCommandViewModel> Current
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list of available tags. The edit control binds to this.
        /// </summary>
        public ObservableCollection<TagCommandViewModel> Available
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleTagController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TitleTagController(TitleViewModel owner)
            : base(owner)
        {
            Current = new ObservableCollection<TagCommandViewModel>();
            Available = new ObservableCollection<TagCommandViewModel>();

            titleTagTable = DatabaseController.Instance.GetTable<TitleTagTable>();
            tagCache = new TagCacheDictionary();
            RefreshAvailable();
        }
        #endregion

        /************************************************************************/
        
        #region Public methods

        /// <summary>
        /// Causes the list of available tags to be refreshed
        /// </summary>
        public void RefreshAvailable()
        {
            tagCache.Clear();
            Available.Clear();
            var tagTable = DatabaseController.Instance.GetTable<TagTable>();
            foreach (DataRow row in tagTable.Rows)
            {
                Int64 tagId = (Int64)row[TagTable.Defs.Columns.Id];
                ICommand cmd = new RelayCommand((o) => { RunTagAddCommand(tagId); }, (o) => CanRunTagAddCommand(tagId));
                Available.Add(new TagCommandViewModel(tagId, row[TagTable.Defs.Columns.Tag].ToString(), row[TagTable.Defs.Columns.Description].ToString(), cmd));
                tagCache.Add(row);
            }
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called by the <see cref=" ControllerBase{VM,T}.Owner"/> of this controller
        /// in order to update the controller values.
        /// </summary>
        protected override void OnUpdate()
        {
            Int64 titleId = GetOwnerSelectedPrimaryId();
            Current.Clear();
            titleTagTable.DefaultView.RowFilter = String.Format("{0}={1}", TitleTagTable.Defs.Columns.TitleId, titleId);
            titleTagTable.DefaultView.Sort = TitleTagTable.Defs.Columns.Joined.TagName;
            foreach (DataRowView dataRowView in titleTagTable.DefaultView)
            {
                Int64 tagId = (Int64)dataRowView[TitleTagTable.Defs.Columns.TagId];
                ICommand cmd = new RelayCommand((o) => { RunTagRemoveCommand(tagId); }, (o) => CanRunTagRemoveCommand(tagId));
                Current.Add(new TagCommandViewModel(tagId, tagCache[tagId].Name, tagCache[tagId].Description, cmd));
            }
            OnPropertyChanged(nameof(HasZeroTags));
        }
        #endregion

        /************************************************************************/

        #region Private methods

        private void RunTagAddCommand(Int64 tagId)
        {
            Int64 titleId = GetOwnerSelectedPrimaryId();
            if (titleId != Int64.MinValue)
            {
                titleTagTable.Add(tagId, titleId);
                titleTagTable.Save();
                OnUpdate();
            }
        }

        private bool CanRunTagAddCommand(Int64 tagId)
        {
            Int64 titleId = GetOwnerSelectedPrimaryId();
            return !titleTagTable.TagExists(tagId, titleId);
        }

        private void RunTagRemoveCommand(Int64 tagId)
        {
            Int64 titleId = GetOwnerSelectedPrimaryId();
            if (titleId != Int64.MinValue)
            {
                titleTagTable.Remove(tagId, titleId);
                titleTagTable.Save();
                OnUpdate();
                if (Config.TitleFilter.IsTagFilterActive)
                {
                    Config.TitleFilter.Update();
                }
            }
        }

        private bool CanRunTagRemoveCommand(Int64 tagId)
        {
            return true;
        }
        #endregion
    }
}
