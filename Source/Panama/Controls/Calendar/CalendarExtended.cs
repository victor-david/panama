using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Provides a calendar control with extended features
    /// </summary>
    public class CalendarExtended : Calendar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarExtended"/> class.
        /// </summary>
        public CalendarExtended()
        {
            HorizontalAlignment = HorizontalAlignment.Left;
        }

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the PreviewMouseUp event is called. Prevents the calendar from "sticking".
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);

            if (Mouse.Captured is CalendarItem)
            {
                Mouse.Capture(null);
            }
        }

        /// <summary>
        /// Called when the selected date or dates change.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnSelectedDatesChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectedDatesChanged(e);
            if (SelectionMode == CalendarSelectionMode.SingleDate)
            {
                if (e.AddedItems.Count == 0)
                {
                    DisplayDate = DateTime.UtcNow;
                }
                else
                {
                    DisplayDate = (DateTime)e.AddedItems[0];
                }
            }
        }
        #endregion
    }
}
