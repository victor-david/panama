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
            public const string IconTrayPlus = "Panama.Icon.Tray.Plus";
            public const string IconTrayRemove = "Panama.Icon.Tray.Remove";
        }
    }
}