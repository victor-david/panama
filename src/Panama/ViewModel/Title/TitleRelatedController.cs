using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableColumns = Restless.Panama.Database.Tables.TitleRelatedTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    public class TitleRelatedController : BaseController<TitleViewModel, TitleRelatedTable>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleRelatedController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TitleRelatedController(TitleViewModel owner) : base(owner)
        {
            Columns.Create("Id", TableColumns.RelatedId)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create("Title", TableColumns.Joined.Title);
            Columns.Create("Written", TableColumns.Joined.Written).MakeDate();

            Columns.Create("Updated", TableColumns.Joined.Updated)
                .MakeDate()
                .AddToolTip(Strings.TooltipTitleUpdated);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            //SelectedPublished = PublishedRow.Create(SelectedRow);
            //OnPropertyChanged(nameof(PublishedDate));
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareString(item2, item1, TableColumns.Joined.Title);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return (long)item[TableColumns.TitleId] == (Owner?.SelectedTitle?.Id ?? 0);
        }
        #endregion
    }
}