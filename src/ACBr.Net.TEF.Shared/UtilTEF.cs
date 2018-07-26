using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using ACBr.Net.Core.Extensions;

namespace ACBr.Net.TEF
{
    public static class UtilTEF
    {
        #region Fields

        private struct MSG
        {
            private IntPtr hwnd;
            private uint message;
            private IntPtr wParam;
            private IntPtr lParam;
            private int time;
            private int ptX;
            private int ptY;
        }

        private const int PM_REMOVE = 0x0001;
        private const int WM_KEYFIRST = 0x0100;
        private const int WM_KEYLAST = 0x0109;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PeekMessage(out MSG lpMsg, uint hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

        #endregion Fields

        #region Methods

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BringWindowToFocus(IntPtr hWnd);

        public static void CleanKeyboardBuffer()
        {
            while (PeekMessage(out _, 0, WM_KEYFIRST, WM_KEYLAST, PM_REMOVE)) { }
        }

        [DllImport("user32.dll", EntryPoint = "BlockInput")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BlockInput([MarshalAs(UnmanagedType.Bool)]bool blockIt);

        internal static bool DeleteFile(string file)
        {
            try
            {
                if (File.Exists(file))
                    File.Delete(file);

                return !File.Exists(file);
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal static string ToAdicParam(this Dictionary<string, string> dic)
        {
            var ret = string.Empty;

            foreach (var parans in dic)
            {
                ret += $"{parans.Key}={parans.Value};";
            }

            return $"[{ret.TrimEnd(';')}]";
        }

        #endregion Methods
    }
}