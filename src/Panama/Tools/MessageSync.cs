/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    // TODO
                }
                else
                {
                    result.NotFound.Add(FileScanItem.Create(fileName));
                }
            }
            return result;
        }
    }
}