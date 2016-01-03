using System;

namespace Tbasic {

    /// <summary>
    /// Defines a set of methods and properties for a line of Tbasic code
    /// </summary>
    public class Line : IComparable<Line>, IEquatable<Line> {

        /// <summary>
        /// Initializes a line of Tbasic code
        /// </summary>
        /// <param name="id">The id of the line. This should be the line number.</param>
        /// <param name="line">The text of the line</param>
        public Line(int id, string line) {
            LineNumber = id;
            Text = line.Trim(); // Ignore leading and trailing whitespace.
            VisibleName = Name;
        }

        /// <summary>
        /// Initializes a line of Tbasic code carring the same information as another Tbasic.Line
        /// </summary>
        /// <param name="line"></param>
        public Line(Line line) {
            LineNumber = line.LineNumber;
            Text = line.Text;
            VisibleName = line.Name;
        }

        /// <summary>
        /// Gets or sets the line number
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the text of this line
        /// </summary>
        public virtual string Text { get; set; }

        /// <summary>
        /// Gets or sets the block in which this line is nested
        /// </summary>
        public CodeBlock CurrentBlock { get; set; }

        private string visibleName = null;

        /// <summary>
        /// Gets or sets the name of the line displayed in exceptions
        /// </summary>
        public string VisibleName {
            get {
                if (visibleName == null) {
                    return Name;
                }
                return visibleName;
            }
            set {
                visibleName = value;
            }
        }

        private string name = null;

        /// <summary>
        /// Retrieves the name that is retreived from the Tbasic.ObjectContext libraries
        /// </summary>
        public string Name {
            get {
                if (name == null) { // This way we don't have to do this every time
                    int bracket = Text.IndexOf('(');
                    int space = Text.IndexOf(' ');
                    if (bracket < 0 && space < 0) {
                        name = Text;
                    }
                    else if (bracket < 0 && space > 0) {
                        name = Text.Remove(space);
                    }
                    else if (space < 0 && bracket > 0) {
                        name = Text.Remove(bracket);
                    }
                    else if (space < bracket) {
                        name = Text.Remove(space);
                    }
                    else {
                        name = Text.Remove(bracket);
                    }
                }
                return name;
            }
        }

        /// <summary>
        /// Returns the text that this line represents
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return Text;
        }

        /// <summary>
        /// Compares this Tbasic.Line to another Tbasic.Line by comparing their LineNumber
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Line other) {
            return LineNumber.CompareTo(other.LineNumber);
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current System.Object
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other) {
            if (other is Line) {
                return this.Equals((Line)other);
            }
            return base.Equals(other);
        }

        /// <summary>
        /// Hash code for the LineNumber
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return LineNumber.GetHashCode();
        }

        /// <summary>
        /// Determines if two Tbasic.Line objects share the same LineNumber
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Line other) {
            return other.LineNumber == LineNumber;
        }
    }
}
