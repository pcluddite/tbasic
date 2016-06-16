using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tbasic.Components
{
    internal struct StringSegment
    {
        private string full;
        private int start;
        private int len;

        public int Length
        {
            get {
                return len;
            }
        }

        public int Offset
        {
            get {
                return start;
            }
        }

        public string FullString
        {
            get {
                return full;
            }
        }

        public StringSegment(string fullStr, int offset)
        {
            full = fullStr;
            start = offset;
            len = fullStr.Length - start;
        }

        public StringSegment(string fullStr, int offset, int count)
        {
            full = fullStr;
            start = offset;
            len = count;
        }

        public char this[int index]
        {
            get {
                return full[start + index];
            }
        }

        public string Substring(int startIndex)
        {
            return full.Substring(startIndex + start);
        }

        public string Substring(int startIndex, int length)
        {
            return full.Substring(startIndex + start, length);
        }

        public override string ToString()
        {
            return full.Substring(start, len);
        }
    }
}
