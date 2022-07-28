/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
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

            foreach (SubmissionMessageRow message in SubmissionMessageTable.EnumerateFileSystem())
            {
                result.ScanCount++;
                string fileName = Path.Combine(Config.Instance.FolderSubmissionMessage, message.EntryId);

                MimeKitMessage msg = new(fileName);
                if (!msg.IsError)
                {
                    string newFileName = GetSynchronizedFileName(msg, fileName);
                    if (message.EntryId != newFileName)
                    {
                        try
                        {
                            string newFileNameFull = Path.Combine(Config.Instance.FolderSubmissionMessage, newFileName);
                            File.Move(fileName, newFileNameFull);
                            message.UpdateEntryId(newFileName);
                            result.AppendOutputText($"Update {message.EntryId} to {newFileName}");
                            result.Updated.Add(FileScanItem.Create(newFileName));
                        }
                        catch (Exception ex)
                        {
                            result.AppendOutputText(ex.Message);
                        }

                    }
                }
                else
                {
                    result.NotFound.Add(FileScanItem.Create(fileName));
                }
            }

            SubmissionMessageTable.Save();
            return result;
        }

        private string GetSynchronizedFileName(MimeKitMessage msg, string fileName)
        {
            string cleanSubject = GetCleanStr(msg.Subject);
            string cleanSender = GetCleanStr(msg.FromName, msg.FromName == msg.FromEmail);

            // MessageDate_Subject.ext
            // 2022-07-28_17.21.09_Subject.eml
            return
                string.Format(CultureInfo.InvariantCulture, "{0}_{1}_{2}{3}",
                msg.MessageDateUtc.ToString("yyyy-MM-dd_HH.mm.ss", CultureInfo.InvariantCulture),
                Format.ValidFileName(cleanSender),
                Format.ValidFileName(cleanSubject),
                Path.GetExtension(fileName));
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