/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Utility;
using System;
using System.Collections.Generic;

namespace Restless.Panama.Tools
{
    public class FileScanResult
    {
        public int ScanCount
        {
            get;
            set;
        }

        public List<FileScanItem> Updated
        {
            get;
        }

        public List<FileScanItem> NotFound
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
        }
    }
}