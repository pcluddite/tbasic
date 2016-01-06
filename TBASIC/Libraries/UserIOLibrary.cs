/**
 *  TBASIC 2.0
 *  Copyright (C) 2016 Timothy Baxendale
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
using Microsoft.VisualBasic;
using System;
using System.Speech.Synthesis;
using System.Threading;
using System.Windows.Forms;
using Tbasic.Runtime;

namespace Tbasic.Libraries
{
    internal class UserIOLibrary : Library
    {
        public UserIOLibrary() {
            Add("TRAYTIP", TrayTip);
            Add("MSGBOX", MsgBox);
            Add("SAY", Say);
            Add("INPUT", Input);
            Add("CONSOLEWRITE", ConsoleWrite);
            Add("CONSOLEPAUSE", ConsolePause);
        }

        private void ConsoleWrite(ref StackFrame _sframe) {
            _sframe.AssertArgs(2);
            Console.Write(_sframe.Get(1));
        }

        private void ConsolePause(ref StackFrame _sframe) {
            _sframe.AssertArgs(1);
            _sframe.Data =  Console.ReadKey(true).KeyChar;
        }

        private void Input(ref StackFrame _sframe) {
            if (_sframe.Count == 2) {
                _sframe.SetAll(
                    _sframe.Get(0), _sframe.Get(1),
                    "Input",  -1, -1
                    );
            }
            if (_sframe.Count == 3) {
                _sframe.SetAll(
                    _sframe.Get(0), _sframe.Get(1),
                    _sframe.Get(2), -1, -1);
            }
            if (_sframe.Count == 4) {
                _sframe.SetAll(
                    _sframe.Get(0), _sframe.Get(1),
                    _sframe.Get(2), _sframe.Get(3), -1);
            }
            _sframe.AssertArgs(5);

            int x = _sframe.Get<int>(3),
                y = _sframe.Get<int>(4);

            string resp = Interaction.InputBox(
                _sframe.Get<string>(1),
                _sframe.Get<string>(2),
                "", x, y);


            if (resp == null || resp.Equals("")) {
                _sframe.Status = -1; // -1 no input 2/24
            }
            else {
                _sframe.Data =  resp;
            }
        }

        private void TrayTip(ref StackFrame _sframe) {
            if (_sframe.Count == 2) {
                _sframe.Add(""); // title
                _sframe.Add(0); // icon
                _sframe.Add(5000); // timeout
            }
            else if (_sframe.Count == 3) {
                _sframe.Add(0); // icon
                _sframe.Add(5000); // timeout
            }
            else if (_sframe.Count == 4) {
                _sframe.Add(5000); // timeout
            }
            _sframe.AssertArgs(5);
            string text = _sframe.Get<string>(1);
            string title = _sframe.Get<string>(2);
            int icon = _sframe.GetIntRange(3, 0, 3);
            int timeout = _sframe.Get<int>(4);
            Thread t = new Thread(TrayTip);
            t.Start(new object[] { timeout, icon, text, title });
        }

        private void TrayTip(object param) {
            try {
                object[] cmd = (object[])param;
                using (NotifyIcon tray = new NotifyIcon()) {
                    tray.Icon = Properties.Resources.Danrabbit_Elementary_Button_info;
                    tray.Visible = true;
                    int timeout = (int)cmd[0];
                    ToolTipIcon icon;
                    switch ((int)cmd[1]) {
                        case 1: icon = ToolTipIcon.Info; break;
                        case 2: icon = ToolTipIcon.Warning; break;
                        case 3: icon = ToolTipIcon.Error; break;
                        default: icon = ToolTipIcon.None; break;
                    }
                    tray.ShowBalloonTip(timeout, (string)cmd[3], (string)cmd[2], icon);
                    System.Threading.Thread.Sleep(timeout);
                    tray.Visible = false;
                }
            }
            catch {
            }
        }

        public static void MsgBox(ref StackFrame _sframe) {
            if (_sframe.Count == 3) {
                _sframe.Add("");
            }
            _sframe.AssertArgs(4);

            int flag = _sframe.Get<int>(1);
            string text = _sframe.Get<string>(2),
                   title = _sframe.Get<string>(3);

            _sframe.Data =  Interaction.MsgBox(text, (MsgBoxStyle)flag, title).ToString();
        }

        private void Say(ref StackFrame _sframe) {
            _sframe.AssertArgs(2);
            Thread t = new Thread(Say);
            t.Start(_sframe.Get<string>(1));
        }

        public void Say(object text) {
            try {
                using (SpeechSynthesizer ss = new SpeechSynthesizer()) {
                    ss.Speak(text.ToString());
                }
            }
            catch {
            }
        }
    }
}
