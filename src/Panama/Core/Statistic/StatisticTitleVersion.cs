using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using System.Collections.Generic;
using System.Data;
using DocumentTypes = Restless.Panama.Database.Tables.DocumentTypeTable.Defs.Values;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides statistics for the <see cref="TitleVersionTable"/>.
    /// </summary>
    public class StatisticTitleVersion : StatisticBase
    {
        #region Private
        // Shared is the number of particular version files that are shared between titles.
        // This is normally zero, but there might be a reason why the user wants to do this.
        private int html, pdf, text, unknown, word2007, wordOpenXml, shared;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticTitleVersion"/> class.
        /// </summary>
        /// <param name="table">The title version table.</param>
        public StatisticTitleVersion(TitleVersionTable table) : base(table)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        public override IEnumerator<Statistic> GetEnumerator()
        {
            yield return Statistic.Create(Text.Total , RowCount);
            yield return Statistic.Create(".docx", wordOpenXml);
            yield return Statistic.Create(".doc", word2007);
            yield return Statistic.Create(".pdf", pdf);
            yield return Statistic.Create(".txt", text);
            yield return Statistic.Create(".html", html);
            yield return Statistic.Create(Text.Shared, shared);
            yield return Statistic.Create(Text.Unknown, unknown);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Populates the statistics
        /// </summary>
        protected override void Refresh()
        {
            base.Refresh();
            html = 0;
            pdf = 0;
            text = 0;
            unknown = 0;
            word2007 = 0;
            wordOpenXml = 0;
            shared = 0;
            Dictionary<string, int> files = new();
            foreach (DataRow row in Table.Rows)
            {
                long docType = (long)row[TitleVersionTable.Defs.Columns.DocType];
                if (docType == DocumentTypes.HtmlFileType) html++;
                if (docType == DocumentTypes.PdfFileType) pdf++;
                if (docType == DocumentTypes.TextFileType) text++ ;
                if (docType == DocumentTypes.UnknownFileType) unknown++;
                if (docType == DocumentTypes.WordOlderFileType) word2007++;
                if (docType == DocumentTypes.WordOpenXmlFileType) wordOpenXml++;
                string fileName = row[TitleVersionTable.Defs.Columns.FileName].ToString();
                if (!files.ContainsKey(fileName))
                {
                    files.Add(fileName, 1);
                }
                else
                {
                    files[fileName]++;
                }
            }

            foreach (int count in files.Values)
            {
                if (count > 1) shared++;
            }
        }
        #endregion
    }
}