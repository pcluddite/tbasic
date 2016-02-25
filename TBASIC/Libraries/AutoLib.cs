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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Tbasic.Win32;
using Forms = System.Windows.Forms;

namespace Tbasic.Libraries {
    /// <summary>
    /// A library used to automate and manipulate key strokes, mouse clicks and other input
    /// </summary>
    public class AutoLib : Library {

        /// <summary>
        /// Initializes a new instance of this class
        /// </summary>
        public AutoLib() {
            Add("MOUSECLICK", MouseClick);
            Add("BLOCKINPUT", BlockInput);
            Add("MOUSEMOVE", MouseMove);
            Add("SEND", Send);
            Add("VOLUMEUP", VolumeUp);
            Add("VOLUMEDOWN", VolumeDown);
            Add("VOLUMEMUTE", VolumeMute);
        }

        /// <summary>
        /// Mouse buttons
        /// </summary>
        public enum MouseButton : long {
            /// <summary>
            /// The left mouse button
            /// </summary>
            Left = User32.MOUSEEVENTF_LEFTDOWN | User32.MOUSEEVENTF_LEFTUP,
            /// <summary>
            /// The right mouse button
            /// </summary>
            Right = User32.MOUSEEVENTF_RIGHTDOWN | User32.MOUSEEVENTF_RIGHTUP
        }

        /// <summary>
        /// Clicks the mouse a specified number of times
        /// </summary>
        /// <param name="button">the mouse button to press</param>
        /// <param name="x">the final x position of the cursor</param>
        /// <param name="y">the final y position of the cursor</param>
        /// <param name="clicks">the number of clicks</param>
        /// <param name="delay">the delay to cursor motion (higher numbers are slower)</param>
        public static void MouseClick(MouseButton button, int x, int y, int clicks = 1, int delay = 1) {
            MouseMove(x, y, delay);
            long action = (long)button;
            for (int i = 0; i < clicks; i++) {
                User32.mouse_event(action, x, y, 0,0);
            }
        }

        private void MouseClick(Paramaters _sframe) {
            if (_sframe.Count == 4) {
                _sframe.Add("1");
                _sframe.Add("1");
            }
            if (_sframe.Count == 5) {
                _sframe.Add("5");
            }
            _sframe.AssertArgs(6);

            int x = _sframe.Get<int>(2),
                y = _sframe.Get<int>(3),
                clicks = _sframe.Get<int>(4),
                speed = _sframe.Get<int>(5);

            MouseButton button;
            if (_sframe.Get(1, "button", "LEFT", "RIGHT").EqualsIgnoreCase("LEFT")) {
                button = MouseButton.Left;
            }
            else {
                button = MouseButton.Right;
            }
            MouseClick(button, x, y, clicks, speed);
        }

        private void MouseMove(Paramaters _sframe) {
            if (_sframe.Count == 3) {
                _sframe.Add(1);
            }
            _sframe.AssertArgs(4);

            MouseMove(_sframe.Get<int>(1),
                      _sframe.Get<int>(2),
                      _sframe.Get<int>(3));
        }

        /// <summary>
        /// Moves the mouse to a specified position on the screen
        /// </summary>
        /// <param name="endX">the final x position of the cursor</param>
        /// <param name="endY">the final y position of the cursor</param>
        /// <param name="delay">the delay to cursor motion (higher numbers are slower)</param>
        public static void MouseMove(double endX, double endY, int delay = 1) {
            if (delay > 0) {
                double startX = Cursor.Position.X,
                       startY = Cursor.Position.Y;

                double direction = startX < endX ? 1 : -1;
                double slope = (endY - startY) / (endX - startX);
                delay = (int)(Math.Sqrt(delay));

                double oldX = startX;
                for (double x = startX; !IsBetween(endX, oldX, x); x += direction) {
                    double y = slope * (x - startX) + startY;
                    int newX = (int)(x + 0.5),
                        newY = (int)(y + 0.5);
                    System.Threading.Thread.Sleep(delay);
                    oldX = x;
                    Cursor.Position = new Point(newX, newY);

                }
            }
            Cursor.Position = new Point((int)endX, (int)endY);
        }

        private static bool IsBetween(double x, double d1, double d2) {
            if (d1 < d2) {
                return (x <= d2) && (x >= d1);
            }
            else {
                return (x <= d1) && (x >= d2);
            }
        }

        /// <summary>
        /// Blocks user input
        /// </summary>
        /// <param name="blocked">true will block, false will unblock</param>
        /// <returns>true if the operation succeeded, otherwise false</returns>
        public static bool BlockInput(bool blocked = true) {
            return User32.BlockInput(blocked);
        }

        private void BlockInput(Paramaters _sframe) {
            _sframe.AssertArgs(2);
            _sframe.SetAll(_sframe.Get(0), _sframe.Get(1).ToString().Replace("1", "true").Replace("0", "false"));
            _sframe.Data = BlockInput(_sframe.Get<bool>(1));
        }

        /// <summary>
        /// Sends keys to the active window
        /// </summary>
        /// <param name="keys">the formatted key string</param>
        public static void Send(string keys) {
            SendKeys.SendWait(keys);
        }

        private void Send(Paramaters _sframe) {
            _sframe.AssertArgs(2);
            Send(_sframe.Get<string>(1));
        }

        /// <summary>
        /// Press the volume up key a given number of times
        /// </summary>
        /// <param name="amnt">number of times to press the key</param>
        public static void VolumeUp(int amnt = 1) {
            for (int i = 0; i < amnt; i++) {
                User32.keybd_event((byte)Forms.Keys.VolumeUp, 0, 0, 0);
            }
        }

        private void VolumeUp(Paramaters _sframe) {
            if (_sframe.Count == 1) {
                _sframe.SetAll(_sframe.Get(0), "1");
            }
            _sframe.AssertArgs(2);
            VolumeUp(_sframe.Get<int>(1));
        }

        /// <summary>
        /// Press the volume down key a given number of times
        /// </summary>
        /// <param name="amnt">number of times to press the key</param>
        public static void VolumeDown(int amnt = 1) {
            for (int i = 0; i < amnt; i++) {
                User32.keybd_event((byte)Forms.Keys.VolumeDown, 0, 0, 0);
            }
        }

        private void VolumeDown(Paramaters _sframe) {
            if (_sframe.Count == 1) {
                _sframe.SetAll(_sframe.Get(0), "1");
            }
            _sframe.AssertArgs(2);
            VolumeDown(_sframe.Get<int>(1));
        }

        /// <summary>
        /// Toggle volume mute
        /// </summary>
        public static void VolumeMute() {
            User32.keybd_event((byte)Forms.Keys.VolumeMute, 0, 0, 0);
        }

        private void VolumeMute(Paramaters _sframe) {
            _sframe.AssertArgs(1);
            VolumeMute();
        }

        internal static byte[] ReadToEnd(Stream stream) {
            long originalPosition = stream.Position;
            stream.Position = 0;

            try {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0) {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length) {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1) {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead) {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally {
                stream.Position = originalPosition;
            }
        }
    }
}