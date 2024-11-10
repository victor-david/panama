using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using ResponseColumns = Restless.Panama.Database.Tables.ResponseTable.Defs.Columns;
using ResponseValues = Restless.Panama.Database.Tables.ResponseTable.Defs.Values;

namespace Restless.Panama.Core
{
    public static class DataTranslator
    {
        public static void TranslateDatabaseTables()
        {
            DatabaseController.Instance.GetTable<ResponseTable>().ApplySessionChanges(row =>
            {
                if ((long)row[ResponseColumns.Id] != ResponseValues.NoResponse)
                {
                    string key = $"SubmissionResponse{row[ResponseColumns.Id].ToString().PadLeft(3, '0')}";
                    row[ResponseColumns.Name] = TranslationSource.Instance.GetString(nameof(Data), key);
                }
            });
        }
    }
}