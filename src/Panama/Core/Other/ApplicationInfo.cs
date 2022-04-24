/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Reflection;
using System.Runtime.Versioning;

namespace Restless.Panama.Core
{
    /// <summary>
    /// A singleton class that provides information about the application.
    /// </summary>
    public sealed class ApplicationInfo
    {
        #region Public properties
        /// <summary>
        /// Gets the singleton instance of this class.
        /// </summary>
        public static ApplicationInfo Instance { get; } = new ApplicationInfo();

        /// <summary>
        /// Gets the assembly information object.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// Gets the assembly product
        /// </summary>
        public string Product => Assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;

        /// <summary>
        /// Gets the assembly title
        /// </summary>
        public string Title => Assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title; 

        /// <summary>
        /// Gets the company
        /// </summary>
        public string Company => Assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;

        /// <summary>
        /// Gets the description
        /// </summary>
        public string Description => Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description;

        /// <summary>
        /// Gets the framework
        /// </summary>
        public string Framework => Assembly.GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName;

        /// <summary>
        /// Gets the raw version property for the assmebly.
        /// </summary>
        public Version VersionRaw => Assembly.GetName().Version;

        /// <summary>
        /// Gets the full version of the assembly as a string.
        /// </summary>
        public string Version => VersionRaw.ToString();

        /// <summary>
        /// Gets the major version of the calling assembly.
        /// </summary>
        public string VersionMajor => $"{VersionRaw.Major}.{VersionRaw.Minor}";

        /// <summary>
        /// Gets a boolean value that indicates if the current process is a 64 bit process.
        /// </summary>
        public bool Is64Bit => Environment.Is64BitProcess;
        #endregion

        /************************************************************************/

        #region Constructor (private)

        private ApplicationInfo()
        {
            Assembly = Assembly.GetEntryAssembly();
        }
        #endregion
    }
}