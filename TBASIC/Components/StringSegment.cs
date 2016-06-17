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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

namespace Tbasic.Components
{
    internal sealed class StringSegment : IEnumerable<char>, IEquatable<StringSegment>, IEquatable<string>
    {
        public static readonly StringSegment Empty = new StringSegment(string.Empty);

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

        public StringSegment(string fullStr)
            : this(fullStr, 0)
        {
        }

        public StringSegment(string fullStr, int offset)
            : this(fullStr, offset, fullStr.Length - offset)
        {
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
                if (index >= len || index < 0)
                    throw new IndexOutOfRangeException();
                return GetCharAt(index);
            }
        }

        private char GetCharAt(int index)
        {
            return full[start + index];
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
            return full.IndexOf(value, start, len) - start;
        }

        public int IndexOf(char value, int startIndex)
        {
            return full.IndexOf(value, start + startIndex, len) - start;
        }

        public StringSegment Remove(int index)
        {
            return new StringSegment(full, start, index);
        }

        public StringSegment Trim()
        {
            int new_start = SkipWhiteSpace();
            if (new_start == -1)
                return Empty;
            int new_end = len - 1;
            while (char.IsWhiteSpace(GetCharAt(new_end))) {
                --new_end;
            }
            return Subsegment(new_start, new_end - new_start + 1);
        }

        public override string ToString()
        {
            return full.Substring(start, len);
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

        public bool Equals(StringSegment other)
        {
            return Equals(this, other);
        }

        public bool Equals(string other)
        {
            if (other == null) {
                return false;
            }
            return this.SequenceEqual(other);
        }

        public override bool Equals(object obj)
        {
            StringSegment seg = obj as StringSegment;
            if (seg != null) {
                return Equals(seg);
            }
            string str = obj as string;
            if (str != null) {
                return Equals(str);
            }
            return base.Equals(obj);
        }

        public static bool Equals(StringSegment a, StringSegment b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if ((object)a == null || (object)b == null)
                return false;

            if (a.Length != b.Length)
                return false;

            unsafe
            {
                fixed(char* afullptr = a.FullString) fixed (char* bfullptr = b.FullString)
                {
                    return EqualsHelper(afullptr + a.Offset, bfullptr + b.Offset, a.Length);
                }
            }
        }

        public static bool Equals(StringSegment a, string b)
        {
            if ((object)a == null || (object)b == null)
                return false;

            if (a.Length != b.Length)
                return false;

            unsafe
            {
                fixed (char* afullptr = a.FullString) fixed (char* bfullptr = b)
                {
                    return EqualsHelper(afullptr + a.Offset, bfullptr, a.Length);
                }
            }
        }

        private static unsafe bool EqualsHelper(char* aptr, char* bptr, int length)
        {
            for(int index = 0; index < length; ++index) {
                if (aptr[index] != bptr[index]) {
                    return false;
                }
            }
            return true;
        }

        public static bool operator==(StringSegment first, StringSegment second)
        {
            return Equals(first, second);
        }

        public static bool operator !=(StringSegment first, StringSegment second)
        {
            return !Equals(first, second);
        }


        public override int GetHashCode()
        {
            return full.GetHashCode() ^ len ^ start; // TODO: Optimize this so there aren't many collisions 6/16/16
        }

        public IEnumerator<char> GetEnumerator()
        {
            return new StringSegEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class StringSegEnumerator : IEnumerator<char>
        {
            private StringSegment seg;
            private int curr = -1;

            public StringSegEnumerator(StringSegment segment)
            {
                seg = segment;
            }

            public char Current
            {
                get {
                    return seg.GetCharAt(curr);
                }
            }

            object IEnumerator.Current
            {
                get {
                    return Current;
                }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                return ++curr < seg.len;
            }

            public void Reset()
            {
                curr = -1;
            }
        }
    }
}
