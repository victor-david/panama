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
            public static readonly Dictionary<long, string> TitleStatusIconMap = new()
            {
                { SubmissionValues.StatusWithdrawn, SquareSmallGrayIconKey },
                { SubmissionValues.StatusAccepted, SquareSmallGreenIconKey },
            };

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

        public static class ToolTip
        {
            public const string SubmissionTitleStatusToolTip = nameof(SubmissionTitleStatusToolTip);
        }
    }
}