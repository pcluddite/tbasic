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
using System.Text;

namespace Tbasic.Components
{
    internal sealed class StringSegment
    {
        public static readonly StringSegment Empty = new StringSegment("", 0);

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

        public StringSegment Subsegment(int startIndex)
        {
            return new StringSegment(full, start + startIndex);
        }

        public StringSegment Subsegment(int startIndex, int length)
        {
            return new StringSegment(full, start + startIndex, length);
        }

        public int IndexOf(char value)
        {
            return full.IndexOf(value, start);
        }

        public int IndexOf(char value, int startIndex)
        {
            return full.IndexOf(value, start + startIndex);
        }

        public StringSegment Remove(int index)
        {
            return new StringSegment(full, start, index);
        }

        public StringSegment Trim()
        {
            int new_start = full.SkipWhiteSpace(start);
            int new_end = len - 1;
            while (char.IsWhiteSpace(this[new_end])) {
                --new_end;
            }
            return new StringSegment(full, new_start, new_end - new_start + 1);
        }

        public override string ToString()
        {
            return full.Substring(start, len);
        }
        
        public static implicit operator StringSegment(string str)
        {
            return new StringSegment(str, 0);
        }

        public static bool IsNullOrEmpty(StringSegment segment)
        {
            return segment == null || segment.FullString == null || segment.Length == 0;
        }

        public int SkipWhiteSpace(int startIndex = 0)
        {
            for (int index = startIndex; index < Length; ++index) {
                if (!char.IsWhiteSpace(this[index])) {
                    return index;
                }
            }
            return -1;
        }
    }
}
