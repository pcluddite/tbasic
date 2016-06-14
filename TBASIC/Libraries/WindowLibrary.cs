﻿/**
 *  TBASIC
 *  Copyright (C) 2013-2016 Timothy Baxendale
 *  
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *  
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *  
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 *  USA
 **/
using System;
using System.Drawing;
using System.IO;
using System.Text;
using Tbasic.Borrowed;
using Tbasic.Runtime;
using Tbasic.Win32;
using Tbasic.Errors;
using System.Linq;
using System.Collections.Generic;

namespace Tbasic.Libraries
{
    internal class WindowLibrary : Library
    {
        public WindowLibrary()
        {
            Add("WinActivate", WinActivate);
            Add("WinClose", WinClose);
            Add("WinKill", WinKill);
            Add("WinMove", WinMove);
            Add("WinSize", WinSize);
            Add("WinGetHandle", WinGetHandle);
            Add("WinGetTitle", WinGetTitle);
            Add("WinSetTitle", WinSetTitle);
            Add("WinSetTrans", WinSetTrans);
            Add("WinSetState", WinSetState);
            Add("WinList", WinList);
            Add("ScreenCapture", GetScreen);
            Add("WinPicture", WinPicture);
            Add("WinExists", WinExists);
        }

        public static int WinRemoveClose(IntPtr hwnd)
        {
            IntPtr hMenu = User32.GetSystemMenu(hwnd, false);
            int menuItemCount = User32.GetMenuItemCount(hMenu);
            return User32.RemoveMenu(hMenu, menuItemCount - 1, User32.MF_BYPOSITION);
        }

        private void WinRemoveClose(TFunctionData parameters)
        {
            parameters.AssertArgs(2);
            parameters.Data = WinRemoveClose(new IntPtr(parameters.Get<long>(1)));
        }

        public static IntPtr WinGetHandle(string title, string sz_class = null)
        {
            return User32.FindWindow(sz_class, title);
        }

        private void WinGetHandle(TFunctionData parameters)
        {
            parameters.AssertArgs(2);
            parameters.Data = WinGetHandle(parameters.Get<string>(1)).ToInt64();
        }

        private void WinGetTitle(TFunctionData parameters)
        {
            parameters.AssertArgs(2);
            parameters.Data = WinGetTitle(new IntPtr(parameters.Get<long>(1)));
        }

        public static string WinGetTitle(IntPtr hwnd)
        {
            int capacity = User32.GetWindowTextLength(hwnd) + 1;
            StringBuilder sb = new StringBuilder(capacity);
            User32.GetWindowText(hwnd, sb, capacity);
            return sb.ToString();
        }

        private void WinSetTitle(TFunctionData parameters)
        {
            parameters.AssertArgs(3);
            IntPtr hwnd = new IntPtr(parameters.Get<long>(1));
            if (!User32.SetWindowText(hwnd, parameters.Get<string>(2))) {
                throw new TbasicException(ErrorServer.GenericError, "Unable to set window title");
            }
        }

        public static bool WinSetState(IntPtr hwnd, uint flag)
        {
            return User32.ShowWindow(hwnd, flag);
        }

        private void WinGetState(TFunctionData parameters)
        {
            parameters.AssertArgs(2);
            parameters.Data = WinGetState(parameters.Get<long>(1));
        }

        public static int WinGetState(long hwnd)
        {
            return (int)WinGetState(new IntPtr(hwnd));
        }

        public static WindowFlag WinGetState(IntPtr hwnd)
        {
            return Windows.GetState(hwnd);
        }

        private void WinSetState(TFunctionData parameters)
        {
            parameters.AssertArgs(3);
            uint flag = parameters.Get<uint>(2);
            if (!WinSetState(new IntPtr(parameters.Get<long>(1)), flag)) {
                throw new TbasicException(ErrorServer.GenericError, "Unable to set window state");
            }
        }

        public static bool WinActivate(IntPtr hwnd)
        {
            return User32.SetForegroundWindow(hwnd);
        }

        private void WinActivate(TFunctionData parameters)
        {
            parameters.AssertArgs(2);
            if (!WinActivate(new IntPtr(parameters.Get<long>(1)))) {
                throw new TbasicException(ErrorServer.GenericError, "Unable to activate window");
            }
        }

        public static bool WinMove(IntPtr hwnd, int x, int y)
        {
            RECT rect = new RECT();
            if (!User32.GetWindowRect(hwnd, out rect)) {
                return false;
            }
            if (User32.SetWindowPos(hwnd, HWND.NoTopMost,
                x, y,
                (rect.Right - rect.Left), (rect.Bottom - rect.Top), SWP.NOACTIVATE)) {
                return true;
            }
            else {
                return false;
            }
        }

        private void WinMove(TFunctionData parameters)
        {
            parameters.AssertArgs(4);
            IntPtr hwnd = new IntPtr(parameters.Get<long>(1));
            if (!WinMove(hwnd, parameters.Get<int>(2), parameters.Get<int>(3))) {
                throw new TbasicException(ErrorServer.GenericError, "Unable to move window");
            }
        }

        public static bool WinSize(IntPtr hwnd, int width, int height)
        {
            WINDOWPLACEMENT place = new WINDOWPLACEMENT();
            if (!User32.GetWindowPlacement(hwnd, out place)) {
                return false;
            }
            if (User32.SetWindowPos(hwnd, HWND.NoTopMost,
                place.rcNormalPosition.X, place.rcNormalPosition.Y,
                width,
                height,
                SWP.NOACTIVATE)) {
                return true;
            }
            else {
                return false;
            }
        }

        private void WinSize(TFunctionData parameters)
        {
            parameters.AssertArgs(4);
            IntPtr hwnd = new IntPtr(parameters.Get<long>(1));
            if (!WinSize(hwnd, parameters.Get<int>(2), parameters.Get<int>(3))) {
                throw new TbasicException(ErrorServer.GenericError, "Unable to resize window");
            }
        }

        public static bool WinKill(IntPtr hwnd)
        {
            return User32.DestroyWindow(hwnd);
        }

        private void WinKill(TFunctionData parameters)
        {
            parameters.AssertArgs(2);
            long hwnd = parameters.Get<long>(1);
            if (!WinKill(new IntPtr(hwnd))) {
                throw new TbasicException(ErrorServer.GenericError, "Unable to kill window");
            }
        }

        public static IntPtr WinClose(IntPtr hwnd)
        {
            return User32.SendMessage(hwnd, SendMessages.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        private void WinClose(TFunctionData parameters)
        {
            parameters.AssertArgs(2);
            parameters.Data = WinClose(new IntPtr(parameters.Get<long>(1))).ToInt32(); // hope this doesn't bite me in the ass on 64-bit machines 5/20/16
        }

        public static bool WinSetTrans(IntPtr hwnd, byte trans)
        {
            User32.SetWindowLong(hwnd, User32.GWL_EXSTYLE, User32.GetWindowLong(hwnd, User32.GWL_EXSTYLE) ^ User32.WS_EX_LAYERED);
            return User32.SetLayeredWindowAttributes(hwnd, 0, trans, User32.LWA_ALPHA);
        }

        private void WinSetTrans(TFunctionData parameters)
        {
            parameters.AssertArgs(3);
            IntPtr hwnd = new IntPtr(parameters.Get<long>(1));
            if (!WinSetTrans(hwnd, parameters.Get<byte>(2))) {
                throw new TbasicException(ErrorServer.GenericError, "Unable to set window transparency");
            }
        }

        public static IEnumerable<IntPtr> WinList(WindowFlag flag = WindowFlag.None)
        {
            if (flag == 0) {
                return Windows.List();
            }
            else {
                return Windows.List(flag);
            }
        }

        private void WinList(TFunctionData parameters)
        {
            if (parameters.Count == 1) {
                parameters.Add((int)WindowFlag.Existing);
            }
            parameters.AssertArgs(2);
            int state = parameters.Get<int>(1);

            IntPtr[] hwnds = WinList((WindowFlag)state).ToArray();

            if (hwnds.Length > 0) {
                object[][] windows = new object[hwnds.Length][];
                for (int index = 0; index < windows.Length; index++) {
                    windows[index] = new object[] {
                        Variable.ConvertToObject(hwnds[index]),
                        WinGetTitle(hwnds[index])
                   };
                }
                parameters.Data = windows;
            }
            else {
                parameters.Status = ErrorSuccess.NoContent;
            }
        }

        private void GetScreen(TFunctionData parameters)
        {
            parameters.AssertArgs(2);
            int compression = parameters.GetIntRange(1, 0, 100);
            parameters.Data = GetScreen(compression);
        }

        public static byte[] GetScreen(int compression = 50)
        {
            ScreenCapture sc = new ScreenCapture();
            Image img = sc.CaptureScreen();
            using (MemoryStream ms = Compress.DoIt(img, compression)) {
                return AutoLib.ReadToEnd(ms);
            }
        }

        public static byte[] WinPicture(IntPtr hwnd, int compression = 50)
        {
            ScreenCapture sc = new ScreenCapture();
            Image pic = sc.CaptureWindow(hwnd);
            using (MemoryStream ms = Compress.DoIt(pic, compression)) {
                return AutoLib.ReadToEnd(ms);
            }
        }

        private void WinPicture(TFunctionData parameters)
        {
            parameters.AssertArgs(3);
            int compression = parameters.GetIntRange(2, 0, 100);
            IntPtr hwnd = new IntPtr(parameters.Get<long>(1));
            parameters.Data = WinPicture(hwnd, compression);
        }

        public static bool WindowExists(IntPtr hwnd)
        {
            return Windows.WinExists(hwnd);
        }

        private void WinExists(TFunctionData parameters)
        {
            parameters.AssertArgs(2);
            parameters.Data = Windows.WinExists(new IntPtr(parameters.Get<long>(1)));
        }
    }
}