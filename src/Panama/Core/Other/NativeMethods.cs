using System;
using System.Runtime.InteropServices;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides native methods.
    /// </summary>
    internal static class NativeMethods
    {
        public const int HWND_BROADCAST = 0xffff;
        public static readonly int WM_SHOW_ACTIVE_WIN = RegisterWindowMessage("911BEAEF-5448-4CB8-A0CF-4A839B7602D1");

        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32", CharSet = CharSet.Unicode)]
        public static extern int RegisterWindowMessage(string message);
    }
}