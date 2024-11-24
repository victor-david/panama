using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Restless.Panama.Controls
{
    /// <summary>
    /// Represents a control to select a month and a day
    /// </summary>
    public class MonthDaySelector : Control
    {
        #region Private
        private const long DefaultSelectedMonth = 1;
        private const long DefaultSelectedDay = 1;

        private readonly ObservableCollection<long> days;
        private static readonly Dictionary<long, long> MonthDayMap = new()
        {
            { 1, 31 }, { 2, 28 }, { 3, 31 }, { 4, 30 },
            { 5, 31 }, { 6, 30 }, { 7, 31 }, { 8, 31 },
            { 9, 30 }, { 10, 31 }, { 11, 30 }, { 12, 31 },
        };
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MonthDaySelector"/> class
        /// </summary>
        public MonthDaySelector()
        {
            days = new ObservableCollection<long>();
            InitializeMonths();
            InitializeDays();
        }

        static MonthDaySelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthDaySelector), new FrameworkPropertyMetadata(typeof(MonthDaySelector)));
        }
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets or sets the selected month
        /// </summary>
        public long SelectedMonth
        {
            get => (long)GetValue(SelectedMonthProperty);
            set => SetValue(SelectedMonthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectedMonth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedMonthProperty = DependencyProperty.Register
            (
                nameof(SelectedMonth), typeof(long), typeof(MonthDaySelector), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultSelectedMonth,
                    BindsTwoWayByDefault = true,
                    CoerceValueCallback = OnCoerceSelectedMonth,
                    PropertyChangedCallback = OnSelectedMonthChanged
                }
            );

        private static object OnCoerceSelectedMonth(DependencyObject d, object baseValue)
        {
            return baseValue is long value ? Math.Clamp(value, 1, 12) : baseValue;
        }

        private static void OnSelectedMonthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as MonthDaySelector)?.AdjustAvailableDays();
        }

        /// <summary>
        /// Gets or sets the selected day
        /// </summary>
        public long SelectedDay
        {
            get => (long)GetValue(SelectedDayProperty);
            set => SetValue(SelectedDayProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectedDay"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDayProperty = DependencyProperty.Register
            (
                nameof(SelectedDay), typeof(long), typeof(MonthDaySelector), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultSelectedDay,
                    BindsTwoWayByDefault = true
                }
            );

        /// <summary>
        /// Gets or sets the brush for selected foreground
        /// </summary>
        public Brush SelectedForegroundBrush
        {
            get => (Brush)GetValue(SelectedForegroundBrushProperty);
            set => SetValue(SelectedForegroundBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectedForegroundBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedForegroundBrushProperty = DependencyProperty.Register
            (
                nameof(SelectedForegroundBrush), typeof(Brush), typeof(MonthDaySelector), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Brushes.DarkBlue
                }
            );

        /// <summary>
        /// Gets or sets the brush for selected background
        /// </summary>
        public Brush SelectedBackgroundBrush
        {
            get => (Brush)GetValue(SelectedBackgroundBrushProperty);
            set => SetValue(SelectedBackgroundBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectedBackgroundBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedBackgroundBrushProperty = DependencyProperty.Register
            (
                nameof(SelectedBackgroundBrush), typeof(Brush), typeof(MonthDaySelector), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Brushes.LightGray
                }
            );

        /// <summary>
        /// Gets or sets the style used for the month selector
        /// </summary>
        public Style MonthSelectorStyle
        {
            get => (Style)GetValue(MonthSelectorStyleProperty);
            set => SetValue(MonthSelectorStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MonthSelectorStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MonthSelectorStyleProperty = DependencyProperty.Register
            (
                nameof(MonthSelectorStyle), typeof(Style), typeof(MonthDaySelector), new FrameworkPropertyMetadata()
            );

        /// <summary>
        /// Gets or sets the style used for the day selector
        /// </summary>
        public Style DaySelectorStyle
        {
            get => (Style)GetValue(DaySelectorStyleProperty);
            set => SetValue(DaySelectorStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DaySelectorStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DaySelectorStyleProperty = DependencyProperty.Register
            (
                nameof(DaySelectorStyle), typeof(Style), typeof(MonthDaySelector), new FrameworkPropertyMetadata()
                {
                    DefaultValue = null
                }
            );

        /// <summary>
        /// Gets the months collection
        /// </summary>
        public ObservableCollection<Month> Months
        {
            get => (ObservableCollection<Month>)GetValue(MonthsProperty);
            private set => SetValue(MonthsPropertyKey, value);
        }

        private static readonly DependencyPropertyKey MonthsPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(Months), typeof(ObservableCollection<Month>), typeof(MonthDaySelector), new FrameworkPropertyMetadata()
                {
                    DefaultValue = null
                }
            );

        /// <summary>
        /// Identifies the <see cref="Months"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MonthsProperty = MonthsPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the list collection to display days
        /// </summary>
        public ListCollectionView Days
        {
            get => (ListCollectionView)GetValue(DaysProperty);
            private set => SetValue(DaysPropertyKey, value);
        }

        private static readonly DependencyPropertyKey DaysPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(Days), typeof(ListCollectionView), typeof(MonthDaySelector), new PropertyMetadata()
            );

        /// <summary>
        /// Identifies the <see cref="Days"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DaysProperty = DaysPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Private methods
        private void InitializeMonths()
        {
            Months = new ObservableCollection<Month>();
            for (int month = 1; month <= 12; month++)
            {
                Months.Add(new Month(month, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)));
            }
        }

        private void InitializeDays()
        {
            for (long day = 1; day <= 31; day++)
            {
                days.Add(day);
            }

            Days = new ListCollectionView(days);
            using (Days.DeferRefresh())
            {
                Days.Filter = (item) => item is long day && IsDayIncluded(day);
            }
        }

        private bool IsDayIncluded(long day)
        {
            return day <= MonthDayMap[SelectedMonth];
        }

        private void AdjustAvailableDays()
        {
            if (SelectedDay > MonthDayMap[SelectedMonth])
            {
                SelectedDay = MonthDayMap[SelectedMonth];
            }
            Days.Refresh();
        }
        #endregion
    }
}