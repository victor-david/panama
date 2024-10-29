using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides native methods.
    /// </summary>
    /// <remarks>
    /// https://learn.microsoft.com/en-us/windows/apps/desktop/modernize/apply-rounded-corners
    /// </remarks>
    internal static partial class NativeMethods
    {
        #region Rounded corners
        /// <summary>
        /// The enum flag for DwmSetWindowAttribute's second parameter, which tells the function what attribute to set.
        /// Copied from dwmapi.h
        /// </summary>
        private enum DWMWINDOWATTRIBUTE
        {
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }

        /// <summary>
        /// The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
        /// what value of the enum to set. Copied from dwmapi.h
        /// </summary>
        private enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }

        /// <summary>
        /// Applies rounded corners to the specified window if possible
        /// </summary>
        /// <param name="window">The window</param>
        public static void ApplyRoundedCorners(Window window)
        {
            try
            {
                if (Environment.OSVersion.Version.Major >= 10)
                {
                    IntPtr hWnd = new WindowInteropHelper(Window.GetWindow(window)).EnsureHandle();
                    DWMWINDOWATTRIBUTE attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
                    DWM_WINDOW_CORNER_PREFERENCE preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
                    Marshal.ThrowExceptionForHR(DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint)));
                }
            }
            catch (Exception)
            {
            }
        }

        [LibraryImport("dwmapi.dll")]
        private static partial int DwmSetWindowAttribute
            (
                IntPtr hwnd, DWMWINDOWATTRIBUTE attribute, ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute, uint cbAttribute
            );
        #endregion

        /************************************************************************/


        #region Windows message (old)

        //public const int HWND_BROADCAST = 0xffff;
        //public static readonly int WM_SHOW_ACTIVE_WIN = RegisterWindowMessage("911BEAEF-5448-4CB8-A0CF-4A839B7602D1");

        //[DllImport("user32")]
        //public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        //[DllImport("user32", CharSet = CharSet.Unicode)]
        //public static extern int RegisterWindowMessage(string message);
        #endregion


        #region Window message
        private const int HWND_BROADCAST = 0xffff;

        public static void ShowActiveWindow()
        {
            PostMessage(HWND_BROADCAST, WM_SHOW_ACTIVE_WIN, IntPtr.Zero, IntPtr.Zero);
        }

        public static readonly int WM_SHOW_ACTIVE_WIN = RegisterWindowMessage("4FF85593-9ED4-4FD9-A24F-BCDD76C87608");

        [DllImport("user32")]
        [SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", Justification = "Makes app crash")]
        private static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);


        [DllImport("user32", CharSet = CharSet.Unicode)]
        [SuppressMessage("Interoperability", "SYSLIB1054:Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time", Justification = "Makes app crash")]
        private static extern int RegisterWindowMessage(string message);
        #endregion



    }
}