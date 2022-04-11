/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
        private const double DefaultMonthMinWidth = 96;

        private readonly ObservableCollection<long> days;
        private static readonly Dictionary<long, long> MonthDayMap = new Dictionary<long, long>()
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
        /// Gets or sets the minimum width for the month selector
        /// </summary>
        public double MonthMinWidth
        {
            get => (double)GetValue(MonthMinWidthProperty);
            set => SetValue(MonthMinWidthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MonthMinWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MonthMinWidthProperty = DependencyProperty.Register
            (
                nameof(MonthMinWidth), typeof(double), typeof(MonthDaySelector), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultMonthMinWidth
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
            SelectedDay = Math.Min(SelectedDay, MonthDayMap[SelectedMonth]);
            Days.Refresh();
        }
        #endregion
    }
}