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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tbasic.Libraries {
    internal class SystemLibrary : Library {

        public SystemLibrary() {
            Add("GetMonth", GetMonth);
            Add("GetDay", GetDay);
            Add("GetDayOfWeek", GetDayOfWeek);
            Add("GetYear", GetYear);
            Add("GetHour", GetHour);
            Add("GetMinute", GetMinute);
            Add("GetSecond", GetSecond);
            Add("GetMillisecond", GetMillisecond);
        }

        private void GetMonth(ref StackFrame _sframe) {
            _sframe.Assert(1);
            _sframe.Data = DateTime.Now.Month;
        }

        private void GetDay(ref StackFrame _sframe) {
            _sframe.Assert(1);
            _sframe.Data = DateTime.Now.Day;
        }

        private void GetDayOfWeek(ref StackFrame _sframe) {
            _sframe.Assert(1);
            _sframe.Data = (int)DateTime.Now.DayOfWeek;
        }

        private void GetYear(ref StackFrame _sframe) {
            _sframe.Assert(1);
            _sframe.Data = DateTime.Now.Year;
        }

        private void GetHour(ref StackFrame _sframe) {
            _sframe.Assert(1);
            _sframe.Data = DateTime.Now.Hour;
        }

        private void GetMinute(ref StackFrame _sframe) {
            _sframe.Assert(1);
            _sframe.Data = DateTime.Now.Minute;
        }

        private void GetSecond(ref StackFrame _sframe) {
            _sframe.Assert(1);
            _sframe.Data = DateTime.Now.Second;
        }

        private void GetMillisecond(ref StackFrame _sframe) {
            _sframe.Assert(1);
            _sframe.Data = DateTime.Now.Millisecond;
        }
    }
}
