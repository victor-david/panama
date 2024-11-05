using System.Collections.Generic;
using SubmissionValues = Restless.Panama.Database.Tables.SubmissionTable.Defs.Values;

namespace Restless.Panama.Resources
{
    /// <summary>
    /// Provides static values for resource keys
    /// </summary>
    public static class ResourceKeys
    {
        /// <summary>
        /// Provides static values for geometry keys
        /// </summary>
        public static class Geometry
        {
            public const string AddGeometryKey = nameof(AddGeometryKey);
            public const string AlertGeometryKey = nameof(AlertGeometryKey);
            public const string ChevronDownGeometryKey = nameof(ChevronDownGeometryKey);
            public const string ChevronLeftGeometryKey = nameof(ChevronLeftGeometryKey);
            public const string ChevronRightGeometryKey = nameof(ChevronRightGeometryKey);
            public const string ChevronUpGeometryKey = nameof(ChevronUpGeometryKey);
            public const string CircleGeometryKey = nameof(CircleGeometryKey);
            public const string ClipboardGeometryKey = nameof(ClipboardGeometryKey);
            public const string DatabaseGeometryKey = nameof(DatabaseGeometryKey);
            public const string ExitGeometryKey = nameof(ExitGeometryKey);
            public const string FileGeometryKey = nameof(FileGeometryKey);
            public const string FilterGeometryKey = nameof(FilterGeometryKey);
            public const string FilterOffGeometryKey = nameof(FilterOffGeometryKey);
            public const string LinkGeometryKey = nameof(LinkGeometryKey);
            public const string LockGeometryKey = nameof(LockGeometryKey);
            public const string NoteGeometryKey = nameof(NoteGeometryKey);
            public const string PlusGeometryKey = nameof(PlusGeometryKey);
            public const string PublisherGeometryKey = nameof(PublisherGeometryKey);
            public const string ReportGeometryKey = nameof(ReportGeometryKey);
            public const string SaveGeometryKey = nameof(SaveGeometryKey);
            public const string SearchGeometryKey = nameof(SearchGeometryKey);
            public const string SettingsGeometryKey = nameof(SettingsGeometryKey);
            public const string StatisticGeometryKey = nameof(StatisticGeometryKey);
            public const string SubmissionGeometryKey = nameof(SubmissionGeometryKey);
            public const string TableGeometryKey = nameof(TableGeometryKey);
            public const string TagGeometryKey = nameof(TagGeometryKey);
            public const string TitleGeometryKey = nameof(TitleGeometryKey);
            public const string TitleQueueGeometryKey = nameof(TitleQueueGeometryKey);
            public const string ToggleGeometryKey = nameof(ToggleGeometryKey);
            public const string ToolGeometryKey = nameof(ToolGeometryKey);
            public const string UserGeometryKey = nameof(UserGeometryKey);
            public const string XGeometryKey = nameof(XGeometryKey);
            public const string ZGeometryKey = nameof(ZGeometryKey);
        }

        /// <summary>
        /// Provides static values for icon keys
        /// </summary>
        public static class Icon
        {
            /// <summary>
            /// Provides an icon map for the status of submitted titles
            /// </summary>
            private static readonly Dictionary<long, string> TitleStatusIconMap = new()
            {
                { SubmissionValues.StatusWithdrawn, IconStatusWithdrawn },
                { SubmissionValues.StatusAccepted, IconStatusAccepted },
            };

            /// <summary>
            /// Gets the title status icon map
            /// </summary>
            /// <returns>The map</returns>
            public static object GetTitleStatusIconMap() => TitleStatusIconMap;

            /// <summary>
            /// Gets the title status resource key for the specified status
            /// </summary>
            /// <param name="status">The status value</param>
            /// <returns>The resource key, or null if status doesn't exist in the map</returns>
            public static object GetTitleStatusIconKey(long status) => TitleStatusIconMap.ContainsKey(status) ? TitleStatusIconMap[status] : null;

            /// <summary>
            /// Gets the title status resource for the specified key
            /// </summary>
            /// <param name="status">The status value</param>
            /// <returns>The resource, or null</returns>
            public static object GetTitleStatusIcon(long status) => LocalResources.Get(GetTitleStatusIconKey(status));

            //  Named icons as resources. Names must correspond
            public const string IconAdd = "Panama.Icon.Plus";
            public const string IconAlert = "Panama.Icon.Alert";
            public const string IconArrowDown = "Panama.Icon.Arrow.Down";
            public const string IconArrowUp = "Panama.Icon.Arrow.Up";
            public const string IconCalendar = "Panama.Icon.Calendar";
            public const string IconCalendarClear = "Panama.Icon.Calendar.Clear";
            public const string IconCheck = "Panama.Icon.Check";
            public const string IconChevronDown = "Panama.Icon.Chevron.Down";
            public const string IconChevronLeft = "Panama.Icon.Chevron.Left";
            public const string IconChevronRight = "Panama.Icon.Chevron.Right";
            public const string IconChevronUp = "Panama.Icon.Chevron.Up";
            public const string IconClose = "Panama.Icon.Close";
            public const string IconCopy = "Panama.Icon.Copy";
            public const string IconDelete = "Panama.Icon.Delete";
            public const string IconError = IconAlert;
            public const string IconFile = "Panama.Icon.File";
            public const string IconFileExtension = "Panama.Icon.File.Extension";
            public const string IconFileReplace = "Panama.Icon.File.Replace";
            public const string IconFilter = "Panama.Icon.Filter";
            public const string IconFilterOff = "Panama.Icon.Filter.Off";
            public const string IconFolder = "Panama.Icon.Folder";
            public const string IconInUse = IconTrayFull;
            public const string IconMinus = "Panama.Icon.Minus";
            public const string IconOpenWebSite = IconChevronRight;
            public const string IconRemove = "Panama.Icon.Remove";
            public const string IconSquare = "Panama.Icon.Square";
            public const string IconStatusAccepted = IconCheck;
            public const string IconStatusWithdrawn = IconRemove;
            public const string IconSynchronize = "Panama.Icon.Synchronize";
            public const string IconToggle = "Panama.Icon.Toggle";
            public const string IconTrayFull = "Panama.Icon.Tray.Full";

            // Old names. These will be removed
            public const string AlertIconKey = nameof(AlertIconKey);
            public const string ChevronRightIconKey = nameof(ChevronRightIconKey);
            public const string CircleIconKey = nameof(CircleIconKey);
            public const string CircleSmallIconKey = nameof(CircleSmallIconKey);
            public const string CircleSmallBlueIconKey = nameof(CircleSmallBlueIconKey);
            public const string CircleSmallGreenIconKey = nameof(CircleSmallGreenIconKey);
            public const string FilterIconKey = nameof(FilterIconKey);
            public const string FilterOffIconKey = nameof(FilterOffIconKey);
            public const string LinkIconKey = nameof(LinkIconKey);
            public const string LockIconKey = nameof(LockIconKey);
            public const string LockMediumIconKey = nameof(LockMediumIconKey);
            public const string NoteIconKey = nameof(NoteIconKey);
            public const string PlusIconKey = nameof(PlusIconKey);
            public const string ReportIconKey = nameof(ReportIconKey);
            public const string SettingsIconKey = nameof(SettingsIconKey);
            public const string SearchIconKey = nameof(SearchIconKey);
            public const string SquareSmallBlueIconKey = nameof(SquareSmallBlueIconKey);
            public const string SquareSmallGrayIconKey = nameof(SquareSmallGrayIconKey);
            public const string SquareSmallGreenIconKey = nameof(SquareSmallGreenIconKey);
            public const string SquareSmallRedIconKey = nameof(SquareSmallRedIconKey);
            public const string StatisticIconKey = nameof(StatisticIconKey);
            public const string SubmissionIconKey = nameof(SubmissionIconKey);
            public const string SubmissionMediumIconKey = nameof(SubmissionMediumIconKey);
            public const string TableIconKey = nameof(TableIconKey);
            public const string TagIconKey = nameof(TagIconKey);
            public const string TitleQueueIconKey = nameof(TitleQueueIconKey);
            public const string ToggleIconKey = nameof(ToggleIconKey);
            public const string ToolIconKey = nameof(ToolIconKey);
            public const string UserIconKey = nameof(UserIconKey);
            public const string XIconKey = nameof(XIconKey);
            public const string XMediumIconKey = nameof(XMediumIconKey);
            public const string XRedIconKey = nameof(XRedIconKey);
        }
    }
}