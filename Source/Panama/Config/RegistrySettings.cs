using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Restless.App.Panama.Configuration
{
    ///// <summary>
    ///// Static class to encapsulate registry settings.
    ///// </summary>
    //public static class RegistrySettings
    //{
    //    #region Private Fields
    //    private const string ApplicationRegistryKey = @"SOFTWARE\XamSystems\TitleManager\6";
    //    private const string IdInitialization = "Initialization";
    //    private const string IdApplicationDirectory = "ApplicationDir";
    //    private const string IdSqlServer = "SqlServer";
    //    private const string IdSqlInstance = "SqlInstance";
    //    private const string IdSqlCatalog = "SqlCatalog";
    //    private const string IdMsAccessProvider = "MsAccessProvider";
    //    private const string IdMsAccessProviderNote = "MsAccessProviderNote";
    //    private const string IdMsAccessDatabaseMainLive = "MsAccessDatabaseMainLive";
    //    private const string IdMsAccessDatabaseMainTest = "MsAccessDatabaseMainTest";
    //    private const string IdMsAccessDatabaseAux = "MsAccessDatabaseAux";

    //    private const string IdMainWindowTop = "MainTop";
    //    private const string IdMainWindowLeft = "MainLeft";
    //    private const string IdMainWindowWidth = "MainWidth";
    //    private const string IdMainWindowHeight = "MainHeight";
    //    private const string IdMainWindowState = "MainState";
    //    private const string IdDatabaseMode = "DataMode";
    //    private const string MsOleDb40 = "Microsoft.Jet.OLEDB.4.0";
    //    private const string MsOleDb12 = "Microsoft.ACE.OLEDB.12.0";
    //    private const string MsAccessProviderDefault = MsOleDb12;
    //    private static RegistrySettingsBase BaseKey = UserRegistrySettings.Instance;
    //    #endregion

    //    /************************************************************************/

    //    #region Public Fields
    //    public const int DatabaseModeLive = 1;
    //    public const int DatabaseModeTest = 0;
    //    public const int DatabaseModeUninitialized = -1;
    //    public const string LiveDatabaseDefaultMain = "title_manager_live_main.accdb";
    //    public const string TestDatabaseDefaultMain = "title_manager_test_main.accdb";
    //    public const string LiveDatabaseDefaultAux = "title_manager_live_aux.accdb";
    //    public const string DatabaseSubDirectory = "db";
    //    #endregion

    //    /************************************************************************/

    //    #region Public Properties
    //    /// <summary>
    //    /// Gets the initialization value.
    //    /// </summary>
    //    public static int Initialization
    //    {
    //        get { return (int)BaseKey.GetValue(IdInitialization); }
    //    }

    //    /// <summary>
    //    /// Gets the application directory.
    //    /// </summary>
    //    public static string ApplicationDirectory
    //    {
    //        get { return (string)BaseKey.GetValue(IdApplicationDirectory); }
    //    }

    //    /// <summary>
    //    /// Gets a boolean value that indicates if the <see cref="ApplicationDirectory"/> property is set.
    //    /// </summary>
    //    public static bool HaveApplicationDirectory
    //    {
    //        get { return !String.IsNullOrEmpty(ApplicationDirectory); }
    //    }
    //    /// <summary>
    //    /// Gets the SQL server
    //    /// </summary>
    //    public static string SqlServer
    //    {
    //        get { return (string)BaseKey.GetValue(IdSqlServer); }
    //    }

    //    /// <summary>
    //    /// Gets the SQL instance
    //    /// </summary>
    //    public static string SqlInstance
    //    {
    //        get { return (string)BaseKey.GetValue(IdSqlInstance); }
    //    }

    //    /// <summary>
    //    /// Gets the SQL catalog
    //    /// </summary>
    //    public static string SqlCatalog
    //    {
    //        get { return (string)BaseKey.GetValue(IdSqlCatalog); }
    //    }

    //    /// <summary>
    //    /// Gets the MS Access Provider.
    //    /// </summary>
    //    public static string MsAccessProvider
    //    {
    //        get { return (string)BaseKey.GetValue(IdMsAccessProvider); }
    //    }

    //    /// <summary>
    //    /// Gets the main MS Access database according to the data mode (live or test)
    //    /// </summary>
    //    public static string MsAccessDatabaseMain
    //    {
    //        get
    //        {
    //            string id = (DatabaseMode == DatabaseModeLive) ? IdMsAccessDatabaseMainLive : IdMsAccessDatabaseMainTest;
    //            return System.IO.Path.Combine(ApplicationDirectory, DatabaseSubDirectory, (string)BaseKey.GetValue(id));
    //        }
    //    }

    //    /// <summary>
    //    /// Gets the main MS Access live database setting, regardless of the data mode.
    //    /// </summary>
    //    public static string MsAccessDatabaseLive
    //    {
    //        get { return System.IO.Path.Combine(ApplicationDirectory, DatabaseSubDirectory, (string)BaseKey.GetValue(IdMsAccessDatabaseMainLive)); }
    //    }

    //    /// <summary>
    //    /// Gets the main MS Access test database setting, regardless of the data mode.
    //    /// </summary>
    //    public static string MsAccessDatabaseTest
    //    {
    //        get { return System.IO.Path.Combine(ApplicationDirectory, DatabaseSubDirectory, (string)BaseKey.GetValue(IdMsAccessDatabaseMainTest)); }
    //    }

    //    /// <summary>
    //    /// Gets the auxilary MS Access database. This does not change between live abd test mode.
    //    /// </summary>
    //    public static string MsAccessDatabaseAux
    //    {
    //        get { return System.IO.Path.Combine(ApplicationDirectory, DatabaseSubDirectory, (string)BaseKey.GetValue(IdMsAccessDatabaseAux)); }
    //    }

    //    /// <summary>
    //    /// Gets or sets the database mode. Use constants <see cref="DatabaseModeLive"/> or <see cref="DatabaseModeTest"/>.
    //    /// </summary>
    //    public static int DatabaseMode
    //    {
    //        get { return (int)BaseKey.GetValue(IdDatabaseMode); }
    //        set { BaseKey.SetValue(IdDatabaseMode, value); }
    //    }

    //    /// <summary>
    //    /// Gets or sets the top position of the main window.
    //    /// </summary>
    //    public static int MainWindowTop
    //    {
    //        get { return (int)BaseKey.GetValue(IdMainWindowTop); }
    //        set { BaseKey.SetValue(IdMainWindowTop, value); }
    //    }

    //    /// <summary>
    //    /// Gets or sets the left position of the main window.
    //    /// </summary>
    //    public static int MainWindowLeft
    //    {
    //        get { return (int)BaseKey.GetValue(IdMainWindowLeft); }
    //        set { BaseKey.SetValue(IdMainWindowLeft, value); }
    //    }

    //    /// <summary>
    //    /// Gets or sets the width of the main window.
    //    /// </summary>
    //    public static int MainWindowWidth
    //    {
    //        get { return (int)UserRegistrySettings.Instance.GetValue(IdMainWindowWidth); }
    //        set { BaseKey.SetValue(IdMainWindowWidth, value); }
    //    }

    //    /// <summary>
    //    /// Gets or sets the height of the main window.
    //    /// </summary>
    //    public static int MainWindowHeight
    //    {
    //        get { return (int)UserRegistrySettings.Instance.GetValue(IdMainWindowHeight); }
    //        set { BaseKey.SetValue(IdMainWindowHeight, value); }
    //    }

    //    /// <summary>
    //    /// Gets or sets the state of the main window.
    //    /// </summary>
    //    public static FormWindowState MainWindowState
    //    {
    //        get { return (FormWindowState)UserRegistrySettings.Instance.GetValue(IdMainWindowState); }
    //        set { BaseKey.SetValue(IdMainWindowState, value); }
    //    }

    //    #endregion

    //    /************************************************************************/

    //    #region Public Methods (Register)
    //    /// <summary>
    //    /// Registers the registry settings.
    //    /// </summary>
    //    public static void Register()
    //    {
    //        UserRegistrySettings.Instance.RegistryKeyName = ApplicationRegistryKey;
    //        UserRegistrySettings.Instance.RegisterSetting(IdInitialization, 0, RegistryValueKind.DWord);
    //        UserRegistrySettings.Instance.RegisterSetting(IdApplicationDirectory, String.Empty);
    //        UserRegistrySettings.Instance.RegisterSetting(IdSqlServer, "(local)");
    //        UserRegistrySettings.Instance.RegisterSetting(IdSqlInstance, "SQLEXPRESS");
    //        UserRegistrySettings.Instance.RegisterSetting(IdSqlCatalog, "master");
    //        UserRegistrySettings.Instance.RegisterSetting(IdMsAccessProvider, String.Empty);
    //        UserRegistrySettings.Instance.RegisterSetting(IdMsAccessProviderNote, String.Empty);
    //        UserRegistrySettings.Instance.RegisterSetting(IdMsAccessDatabaseMainLive, String.Empty);
    //        UserRegistrySettings.Instance.RegisterSetting(IdMsAccessDatabaseMainTest, String.Empty);
    //        UserRegistrySettings.Instance.RegisterSetting(IdMsAccessDatabaseAux, String.Empty);
    //        UserRegistrySettings.Instance.RegisterSetting(IdDatabaseMode, DatabaseModeUninitialized, RegistryValueKind.DWord);
    //        UserRegistrySettings.Instance.RegisterSetting(IdMainWindowTop, 50, RegistryValueKind.DWord);
    //        UserRegistrySettings.Instance.RegisterSetting(IdMainWindowLeft, 50, RegistryValueKind.DWord);
    //        UserRegistrySettings.Instance.RegisterSetting(IdMainWindowWidth, 714, RegistryValueKind.DWord);
    //        UserRegistrySettings.Instance.RegisterSetting(IdMainWindowHeight, 530, RegistryValueKind.DWord);
    //        UserRegistrySettings.Instance.RegisterSetting(IdMainWindowState, FormWindowState.Maximized, RegistryValueKind.DWord);
    //        if (Initialization == 0)
    //        {
    //            BaseKey.SetValue(IdInitialization, DateTime.Now.GetHashCode());
    //            BaseKey.SetValue(IdMsAccessProvider, MsAccessProviderDefault);
    //            BaseKey.SetValue(IdMsAccessProviderNote, String.Format("Set key {0} to {1} or {2} - Default is {3}", IdMsAccessProvider, MsOleDb40, MsOleDb12, MsAccessProviderDefault));
    //            BaseKey.SetValue(IdMsAccessDatabaseMainLive, LiveDatabaseDefaultMain);
    //            BaseKey.SetValue(IdMsAccessDatabaseMainTest, TestDatabaseDefaultMain);
    //            BaseKey.SetValue(IdMsAccessDatabaseAux, LiveDatabaseDefaultAux);
    //            BaseKey.SetValue(IdDatabaseMode, DatabaseModeLive);
    //        }
    //    }
    //    #endregion

    //    /************************************************************************/

    //    #region Public Methods (Application Directory)
    //    /// <summary>
    //    /// Presents a dialog to set the application location
    //    /// </summary>
    //    /// <param name="forceDialog">
    //    /// When false, the dialog is presented only if the application directory is not already set.
    //    /// When true, the dialog is presented regardless of the setting of the application directory.
    //    /// </param>
    //    /// <returns></returns>
    //    public static bool SetApplicationDirectory(bool forceDialog)
    //    {

    //        if (ValidateApplicationDirectory() && !forceDialog) return true;

    //        bool isValid = false;
    //        bool cancelled = false;

    //        while (!isValid && !cancelled)
    //        {
    //            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
    //            {
    //                dialog.Title = String.Format(Resources.Prompt_SelectRootDirectory, AssemblyInfo.Title);
    //                dialog.IsFolderPicker = true; ;
    //                dialog.Multiselect = false;
    //                dialog.NavigateToShortcut = false;
    //                dialog.DefaultDirectory = (HaveApplicationDirectory) ? ApplicationDirectory : System.IO.Path.GetDirectoryName(AssemblyInfo.Location);
    //                cancelled = (dialog.ShowDialog() != CommonFileDialogResult.Ok);
    //                if (!cancelled)
    //                {
    //                    BaseKey.SetValue(IdApplicationDirectory, dialog.FileName);
    //                    isValid = ValidateApplicationDirectory();
    //                    if (!isValid)
    //                    {
    //                        Messages.ShowError(String.Format(Resources.Info_ApplicationDoesNotExist, AssemblyInfo.Title));
    //                    }
    //                }
    //            }
    //        }

    //        return isValid;
    //    }

    //    /// <summary>
    //    /// Validates that the application directory points to a location that contains the database files.
    //    /// </summary>
    //    /// <returns></returns>
    //    public static bool ValidateApplicationDirectory()
    //    {
    //        if (!HaveApplicationDirectory) return false;
    //        if (!System.IO.File.Exists(MsAccessDatabaseMain)) return false;
    //        return true;
    //    }
    //    #endregion
    //}
}
