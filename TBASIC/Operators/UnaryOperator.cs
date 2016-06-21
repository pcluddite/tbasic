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
using Tbasic.Components;

namespace Tbasic.Operators
{
    /// <summary>
    /// Represents an operator that takes one operand
    /// </summary>
    public struct UnaryOperator
    {
        /// <summary>
        /// Side that the operator uses as its operand
        /// </summary>
        public enum OperandSide
        {
            /// <summary>
            /// The operand is on the left
            /// </summary>
            Left,
            /// <summary>
            /// The operand is on the right
            /// </summary>
            Right
        }

        /// <summary>
        /// A delegate that represents the method which processes the operand. 
        /// </summary>
        /// <param name="value">the operand</param>
        /// <returns>the result of the operator</returns>
        public delegate object UnaryOpDelegate(object value);

        /// <summary>
        /// Gets the string representation of the operator
        /// </summary>
        public string OperatorString { get; private set; }

        /// <summary>
        /// Gets the method that processes the operands
        /// </summary>
        public UnaryOpDelegate ExecuteOperator { get; private set; }

        /// <summary>
        /// Gets the side that she operand should be on
        /// </summary>
        public OperandSide Side { get; private set; }

        /// <summary>
        /// Creates a new UnaryOperator
        /// </summary>
        /// <param name="strOp">the string representation of the operator</param>
        /// <param name="doOp">the method that processes the operand</param>
        /// <param name="side">the side that the operand is on</param>
        public UnaryOperator(string strOp, UnaryOpDelegate doOp, OperandSide side = OperandSide.Right)
        {
            OperatorString = strOp;
            ExecuteOperator = doOp;
            Side = side;
        }
    }
}
