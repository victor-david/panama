using System.Collections.Generic;
using SubmissionValues = Restless.Panama.Database.Tables.SubmissionTable.Defs.Values;

namespace Restless.Panama.Core
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
            public const string IconAdd = "App.Icon.Plus";
            public const string IconAlert = "App.Icon.Alert";
            public const string IconArrowDown = "App.Icon.Arrow.Down";
            public const string IconArrowUp = "App.Icon.Arrow.Up";
            public const string IconCalendar = "App.Icon.Calendar";
            public const string IconCalendarClear = "App.Icon.Calendar.Clear";
            public const string IconCheck = "App.Icon.Check";
            public const string IconChevronDown = "App.Icon.Chevron.Down";
            public const string IconChevronLeft = "App.Icon.Chevron.Left";
            public const string IconChevronRight = "App.Icon.Chevron.Right";
            public const string IconChevronUp = "App.Icon.Chevron.Up";
            public const string IconClose = "App.Icon.Close";
            public const string IconCopy = "App.Icon.Copy";
            public const string IconDelete = "App.Icon.Delete";
            public const string IconError = IconAlert;
            public const string IconFile = "App.Icon.File";
            public const string IconFileExtension = "App.Icon.File.Extension";
            public const string IconFileReplace = "App.Icon.File.Replace";
            public const string IconFilter = "App.Icon.Filter";
            public const string IconFilterOff = "App.Icon.Filter.Off";
            public const string IconFolder = "App.Icon.Folder";
            public const string IconInUse = IconTrayFull;
            public const string IconMinus = "App.Icon.Minus";
            public const string IconOpenWebSite = IconChevronRight;
            public const string IconRemove = "App.Icon.Remove";
            public const string IconSquare = "App.Icon.Square";
            public const string IconStatusAccepted = IconCheck;
            public const string IconStatusWithdrawn = IconRemove;
            public const string IconSynchronize = "App.Icon.Synchronize";
            public const string IconToggle = "App.Icon.Toggle";
            public const string IconTrayFull = "App.Icon.Tray.Full";
            public const string IconTrayPlus = "App.Icon.Tray.Plus";
            public const string IconTrayRemove = "App.Icon.Tray.Remove";
        }
    }
}