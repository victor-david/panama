using Restless.Toolkit.Core.Database.SQLite;
using System.Data;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that holds combined info from the published and self published tables.
    /// </summary>
    public class PublishedAllTable : Core.ApplicationTableBase
    {
        public static class Defs
        {
            public const string TableName = "publishedall";

            public static class Columns
            {
                public const string Id = DefaultPrimaryKeyName;

                public const string TypeId = "typeid";

                public const string RelatedId = "relatedid";

                public const string Added = "added";

                public const string Published = "published";

                public const string Title = "title";

                public const string Publisher = "publisher";

                public const string Url = "url";

                public const string Note = "note";
            }
        }

        public PublishedAllTable() : base(Core.DatabaseController.MemorySchemaName, Defs.TableName)
        {
            IsReadOnly = true;
        }

        public override void Load()
        {
            Load(null, $"{Defs.Columns.Added} desc");
        }

        public void Initialize()
        {
            if (Rows.Count == 0)
            {
                Populate();
            }
        }

        protected override ColumnDefinitionCollection GetColumnDefinitions()
        {
            return new ColumnDefinitionCollection()
            {
                { Defs.Columns.Id, ColumnType.Integer, true },
                { Defs.Columns.TypeId, ColumnType.Integer },
                { Defs.Columns.RelatedId, ColumnType.Integer },
                { Defs.Columns.Added, ColumnType.Timestamp },
                { Defs.Columns.Published, ColumnType.Timestamp, false, true },
                { Defs.Columns.Title, ColumnType.Text },
                { Defs.Columns.Publisher, ColumnType.Text },
                { Defs.Columns.Url, ColumnType.Text, false, true },
                { Defs.Columns.Note, ColumnType.Text, false, true },
            };
        }

        private void Populate()
        {
            long id = 1;
            foreach (PublishedRow item in Controller.GetTable<PublishedTable>().EnumerateAll())
            {
                DataRow row = NewRow();
                row[Defs.Columns.Id] = id++;
                row[Defs.Columns.Added] = item.Added;
                row[Defs.Columns.Note] = item.Notes;
                row[Defs.Columns.Published] = item.Published;
                row[Defs.Columns.Publisher] = item.PublisherName;
                row[Defs.Columns.RelatedId] = item.Id;
                row[Defs.Columns.Title] = GetTitle(item.TitleId);
                row[Defs.Columns.TypeId] = 1;
                row[Defs.Columns.Url] = item.Url;
                Rows.Add(row);
            }

            foreach (SelfPublishedRow item in Controller.GetTable<SelfPublishedTable>().EnumerateAll())
            {
                DataRow row = NewRow();
                row[Defs.Columns.Id] = id++;
                row[Defs.Columns.Added] = item.Added;
                row[Defs.Columns.Note] = item.Notes;
                row[Defs.Columns.Published] = item.Published;
                row[Defs.Columns.Publisher] = item.PublisherName;
                row[Defs.Columns.RelatedId] = item.Id;
                row[Defs.Columns.Title] = GetTitle(item.TitleId);
                row[Defs.Columns.TypeId] = 2;
                row[Defs.Columns.Url] = item.Url;
                Rows.Add(row);
            }
        }

        private string GetTitle(long titleId)
        {
            TitleRow title = Controller.GetTable<TitleTable>().GetSingleRecord(titleId);
            return title != null ? title.Title : "--";
        }
    }
}
