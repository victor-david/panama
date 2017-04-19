using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Provides a calendar control with extended features
    /// </summary>
    public class CalendarExtended : Calendar
    {

        #pragma warning disable 1591
        public CalendarExtended()
        {
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
        }
        #pragma warning restore 1591

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
        #endregion
    }
}
