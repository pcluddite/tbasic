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

namespace Tbasic.Parsing
{
    /// <summary>
    /// Defines a set of methods and properties for a line of Tbasic code
    /// </summary>
    public struct Line : IComparable<Line>, IEquatable<Line>
    {
        /// <summary>
        /// Gets a value indicating whether this line is formatted like a function. This does not determine if the function is actually defined.
        /// </summary>
        public bool IsFunction { get; private set; }

        /// <summary>
        /// Gets the line number
        /// </summary>
        public uint LineNumber { get; private set; }

        /// <summary>
        /// Gets the text of this line
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the name of the line displayed in exceptions
        /// </summary>
        public string VisibleName { get; private set; }

        /// <summary>
        /// Retrieves the name that is retreived from the ObjectContext libraries
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a line of Tbasic code
        /// </summary>
        /// <param name="id">The id of the line. This should be the line number.</param>
        /// <param name="line">The text of the line</param>
        public Line(uint id, string line)
        {
            LineNumber = id;
            Text = line.Trim(); // Ignore leading and trailing whitespace.

            bool isFunc;
            Name = FindAndSetName(Text, out isFunc);
            VisibleName = Name;
            IsFunction = isFunc;
        }

        /// <summary>
        /// Initializes a line of Tbasic code
        /// </summary>
        /// <param name="id">The id of the line. This should be the line number.</param>
        /// <param name="line">The text of the line</param>
        /// <param name="visibleName">The visible name of this line</param>
        public Line(uint id, string line, string visibleName)
        {
            LineNumber = id;
            Text = line.Trim();
            bool isFunc;
            Name = FindAndSetName(Text, out isFunc);
            VisibleName = visibleName;
            IsFunction = isFunc;
        }

        /// <summary>
        /// Initializes a line of Tbasic code carring the same information as another Tbasic.Line
        /// </summary>
        /// <param name="line"></param>
        public Line(Line line)
        {
            LineNumber = line.LineNumber;
            Text = line.Text;
            VisibleName = line.VisibleName;
            IsFunction = line.IsFunction;
            Name = line.Name;
        }

        private static string FindAndSetName(string Text, out bool isFunc)
        {
            int paren = Text.IndexOf('(');
            int space = Text.IndexOf(' ');
            isFunc = false;
            if (paren < 0 && space < 0) { // no paren or space, the name is the who line
                return Text;
            }
            else if (paren < 0 && space > 0) { // no paren, but there's a space
                return Text.Remove(space);
            }
            else if (space < 0 && paren > 0) { // no space, but there's a paren
                isFunc = true;
                return Text.Remove(paren);
            }
            else if (space < paren) { // the space is before the paren, so that's where the name is
                return Text.Remove(space);
            }
            else {
                isFunc = true; // it's formatted like a function
                return Text.Remove(paren);
            }
        }

        /// <summary>
        /// Returns the text that this line represents
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Text;
        }

        /// <summary>
        /// Compares this Tbasic.Line to another Tbasic.Line by comparing their LineNumber
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Line other)
        {
            return LineNumber.CompareTo(other.LineNumber);
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            if (other is Line) {
                return this.Equals((Line)other);
            }
            return base.Equals(other);
        }

        /// <summary>
        /// Hash code for the LineNumber
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return LineNumber.GetHashCode();
        }

        /// <summary>
        /// Determines if two Tbasic.Line objects share the same LineNumber
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Line other)
        {
            return other.LineNumber == LineNumber;
        }
    }
}