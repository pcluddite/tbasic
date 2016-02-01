/**
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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Tbasic.Win32
{
    internal class Windows {

        [DllImport("user32.dll", SetLastError = true)]
        static extern System.IntPtr DefWindowProcW(
            IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam
        );

        public static bool WindowExists(IntPtr hwnd) {
            RECT rect;
            return User32.GetWindowRect(hwnd, out rect);
        }

        public static int GetState(IntPtr hwnd) {
            IntPtr exists = User32.GetWindow(hwnd, 0);
            int state = 1;
            if (exists.ToInt32() == 0) { return 0; }
            if (User32.IsWindowVisible(hwnd)) { state += 2; }
            if (User32.IsWindowEnabled(hwnd)) { state += 4; }
            if (User32.GetForegroundWindow() == hwnd) { state += 8; }
            WINDOWPLACEMENT plac;
            User32.GetWindowPlacement(hwnd, out plac);
            if (plac.showCmd == 2) { state += 16; }
            if (plac.showCmd == 3) { state += 32; }
            return state;
        }

        public static IntPtr[] Enum() {
            return (new WinLister()).List().ToArray();
        }

        public static IntPtr[] Enum(int flag) {

            var results =
                from hwnd in Enum()
                where (GetState(hwnd) & flag) == flag
                select hwnd;

            return results.ToArray();
        }

        internal class WinLister {
            [DllImport("user32.dll")]
            private static extern int EnumWindows(CallBackPtr callPtr, int lPar);

            delegate bool CallBackPtr(IntPtr hwnd, int lParam);

            private Stack<IntPtr> list;
            private static CallBackPtr callBackPtr;

            public WinLister() {
                list = new Stack<IntPtr>();
            }

            private bool Report(IntPtr hwnd, int lParam) {
                list.Push(hwnd);
                return true;
            }

            public Stack<IntPtr> List() {
                list.Clear();
                callBackPtr = new CallBackPtr(Report);
                EnumWindows(callBackPtr, 0);
                return list;
            }
        }
    }
}
