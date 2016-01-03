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
