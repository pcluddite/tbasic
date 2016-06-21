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

namespace Tbasic.Operators
{
    internal struct BinaryOperator : IComparable<BinaryOperator>, IEquatable<BinaryOperator>
    {
        public delegate object BinaryOpDelegate(object left, object right);

        public string OperatorString { get; private set; }
        public int Precedence { get; private set; }
        public BinaryOpDelegate ExecuteOperator { get; private set; }

        public BinaryOperator(string strOp, int precedence, BinaryOpDelegate doOp)
        {
            OperatorString = strOp.ToUpper();
            Precedence = precedence;
            ExecuteOperator = doOp;
        }
        
        public int CompareTo(BinaryOperator other)
        {
            return Precedence.CompareTo(other.Precedence);
        }

        public bool Equals(BinaryOperator other)
        {
            return OperatorString == other.OperatorString && Precedence == other.Precedence;
        }

        public static bool operator ==(BinaryOperator first, BinaryOperator second)
        {
            return Equals(first, second);
        }

        public static bool operator !=(BinaryOperator first, BinaryOperator second)
        {
            return !Equals(first, second);
        }

        public override bool Equals(object obj)
        {
            BinaryOperator? op = obj as BinaryOperator?;
            if (op != null)
                return Equals(op.Value);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return OperatorString.GetHashCode() ^ Precedence ^ ExecuteOperator.GetHashCode();
        }
    }
}
