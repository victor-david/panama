using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Core.Utility;
using System;
using System.Globalization;
using System.IO;

namespace Restless.Panama.Tools
{
    public class MessageSync : Scanner
    {
        private SubmissionMessageTable SubmissionMessageTable => DatabaseController.Instance.GetTable<SubmissionMessageTable>();

        /// <inheritdoc/>
        protected override FileScanResult ExecuteTask()
        {
            FileScanResult result = new();

            // empty. only init if needed.
            string[] allFiles = Array.Empty<string>();

            foreach (SubmissionMessageRow message in SubmissionMessageTable.EnumerateFileSystem())
            {
                result.ScanCount++;

                string fullPath = Paths.SubmissionMessage.WithRoot(message.EntryId);

                if (!File.Exists(fullPath))
                {
                    if (allFiles.Length == 0)
                    {
                        allFiles = Directory.GetFiles(Config.Instance.FolderSubmissionMessage, "*", SearchOption.AllDirectories);
                    }
                    fullPath = FindMissingMessageFile(allFiles, fullPath, Path.GetFileName(message.EntryId));
                }

                MimeKitMessage msg = new(fullPath);
                if (!msg.IsError)
                {
                    string newEntryId = GetSynchronizedEntryId(msg, fullPath);

                    if (message.EntryId != newEntryId)
                    {
                        try
                        {
                            string newFullPath = Paths.SubmissionMessage.WithRoot(newEntryId);
                            File.Move(fullPath, newFullPath);
                            message.UpdateEntryId(newEntryId);
                            result.AppendOutputText($"Update {message.EntryId} to {newEntryId}");
                            result.Updated.Add(FileScanItem.Create(newEntryId));
                        }
                        catch (Exception ex)
                        {
                            result.AppendOutputText(ex.Message);
                        }
                    }
                }
                else
                {
                    result.AppendOutputText("Not found: " + fullPath);
                    result.NotFound.Add(FileScanItem.Create(fullPath));
                }
            }

            SubmissionMessageTable.Save();
            return result;
        }

        private string FindMissingMessageFile(string[] allFiles, string currentFullPath, string entryId)
        {
            foreach (string fullPath in allFiles)
            {
                if (Path.GetFileName(fullPath) == entryId)
                {
                    return fullPath;
                }
            }
            return currentFullPath;
        }

        private string GetSynchronizedEntryId(MimeKitMessage msg, string fullPath)
        {
            string entryId = Paths.SubmissionMessage.WithoutRoot(fullPath);
            string directoryPreface = string.Empty;

            // see if message file is inside a sub directory
            if (entryId.Contains(Path.DirectorySeparatorChar))
            {
                directoryPreface = Path.GetDirectoryName(entryId) + Path.DirectorySeparatorChar;
            }

            string cleanSubject = GetCleanStr(msg.Subject);
            string cleanSender = GetCleanStr(msg.FromName, msg.FromName == msg.FromEmail);

            // MessageDate_Subject.ext
            // 2022-07-28_17.21.09_Subject.eml

            string synchronizedEntryId =
                string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}{3}",
                msg.MessageDateUtc.ToString("yyyy-MM-dd_HH.mm.ss", CultureInfo.InvariantCulture),
                Format.ValidFileName(cleanSender),
                Format.ValidFileName(cleanSubject),
                Path.GetExtension(entryId));

            return directoryPreface + synchronizedEntryId;
        }

        private string GetCleanStr(string str, bool allowDot = false)
        {
            str = str.Replace(":", "-").Replace(" ", "").Replace(",", "").Replace("'", "");
            if (!allowDot)
            {
                str = str.Replace(".", "");
            }

            return Format.ValidFileName(str);
        }
    }
}