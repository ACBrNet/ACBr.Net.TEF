using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using ACBr.Net.Core;
using ACBr.Net.Core.InteropServices;

namespace ACBr.Net.TEF
{
    public static class UtilTEF
    {
        #region InnerTypes

        private sealed class User32Client : ACBrSafeHandle
        {
            #region InnerTypes

            [StructLayout(LayoutKind.Sequential)]
            public struct MSG
            {
                public IntPtr hwnd;
                public uint message;
                public IntPtr wParam;
                public IntPtr lParam;
                public uint time;
                public Point p;
            }

            private const int PM_REMOVE = 1;
            private const int PM_NOYIELD = 2;
            private const int WM_KEYFIRST = 256;
            private const int WM_KEYLAST = 264;

            private sealed class Delegates
            {
                [return: MarshalAs(UnmanagedType.Bool)]
                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                public delegate bool PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

                [return: MarshalAs(UnmanagedType.Bool)]
                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                public delegate bool SetForegroundWindow(IntPtr hWnd);

                [return: MarshalAs(UnmanagedType.Bool)]
                [UnmanagedFunctionPointer(CallingConvention.StdCall)]
                public delegate bool BlockInput([MarshalAs(UnmanagedType.Bool)] bool blockIt);
            }

            #endregion InnerTypes

            #region Constructors

            public User32Client() : base("user32.dll")
            {
                AddMethod<Delegates.PeekMessage>("PeekMessageA");
                AddMethod<Delegates.SetForegroundWindow>("SetForegroundWindow");
                AddMethod<Delegates.BlockInput>("BlockInput");
            }

            #endregion Constructors

            #region Methods

            [HandleProcessCorruptedStateExceptions]
            public void CleanKeyboardBuffer()
            {
                try
                {
                    var method = GetMethod<Delegates.PeekMessage>();
                    while (method(out _, IntPtr.Zero, WM_KEYFIRST, WM_KEYLAST, PM_REMOVE | PM_NOYIELD)) { }
                }
                catch (Exception exception)
                {
                    throw new ACBrException(exception.Message, exception);
                }
            }

            public bool SetForegroundWindow(IntPtr hWnd)
            {
                var method = GetMethod<Delegates.SetForegroundWindow>();
                return ExecuteMethod(() => method(hWnd));
            }

            public bool BlockInput(bool block)
            {
                var method = GetMethod<Delegates.BlockInput>();
                return ExecuteMethod(() => method(block));
            }

            #endregion Methods
        }

        #endregion InnerTypes

        #region Methods

        public static bool BringWindowToFocus(IntPtr hWnd)
        {
            using (var cliente = new User32Client())
            {
                return cliente.SetForegroundWindow(hWnd);
            }
        }

        public static void CleanKeyboardBuffer()
        {
            using (var cliente = new User32Client())
            {
                cliente.CleanKeyboardBuffer();
            }
        }

        public static bool BlockInput(bool blockIt)
        {
            using (var cliente = new User32Client())
            {
                return cliente.BlockInput(blockIt);
            }
        }

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