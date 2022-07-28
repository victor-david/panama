/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.Collections.Generic;
using System.Text;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Represents the result of a file scan
    /// </summary>
    public class FileScanResult
    {
        /// <summary>
        /// Gets or sets the scan count
        /// </summary>
        public int ScanCount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the updated items
        /// </summary>
        public List<FileScanItem> Updated
        {
            get;
        }

        /// <summary>
        /// Gets the not found items
        /// </summary>
        public List<FileScanItem> NotFound
        {
            get;
        }

        /// <summary>
        /// Gets the output text
        /// </summary>
        public StringBuilder OutputText
        {
            get;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileScanResult"/> class
        /// </summary>
        public FileScanResult()
        {
            Updated = new List<FileScanItem>();
            NotFound = new List<FileScanItem>();
            OutputText = new StringBuilder();
        }

        public void AppendOutputText(string text)
        {
            OutputText.AppendLine(text);
        }
    }
}