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
using System.Collections.Generic;
using System.Text;
using Tbasic.Errors;
using Tbasic.Parsing;
using System;
using Tbasic.Components;

namespace Tbasic.Runtime
{
    internal class Variable : IExpression
    {

        #region Private Members

        private StringSegment _expression = null;
        private StringSegment _variable = null;

        #endregion

        #region Properties

        public int[] Indices { get; private set; }

        public bool IsMacro
        {
            get {
                return Name[0] == '@';
            }
        }

        public bool IsValid
        {
            get {
                return Name[Name.Length - 1] == '$';
            }
        }

        public ObjectContext CurrentContext
        {
            get {
                return CurrentExecution.Context;
            }
        }

        public Executer CurrentExecution { get; set; }

        public StringSegment Expression
        {
            get {
                return _expression;
            }
            set {
                _expression = value.Trim();
                if (_expression.Length > Name.Length) {
                    StringSegment indicesString = _expression.Subsegment(_expression.SkipWhiteSpace(Name.Length));
                    if (indicesString.Offset > -1 && indicesString[0] == '[') {
                        IList<object> indices;
                        int last = GroupParser.ReadGroup(_expression, _expression.IndexOf('['), CurrentExecution, out indices);
                        if (last < _expression.Length - 1) {
                            _expression = _expression.Remove(last + 1);
                        }
                        if (indices.Count == 0) {
                            throw ThrowHelper.NoIndexSpecified();
                        }
                        Indices = new int[indices.Count];
                        for (int i = 0; i < Indices.Length; ++i) {
                            int? index = indices[i] as int?;
                            if (index == null) {
                                throw ThrowHelper.InvalidTypeInExpression(indices[i].GetType().Name, typeof(int).Name);
                            }
                            else {
                                Indices[i] = index.Value;
                            }
                        }
                    }
                    else {
                        Indices = null;
                    }
                }
                else {
                    Indices = null;
                }
            }
        }

        public StringSegment Name
        {
            get {
                if (_variable == null) {
                    _variable = GetName(_expression);
                }
                return _variable;
            }
        }

        #endregion

        public Variable(StringSegment full, Executer exec)
        {
            CurrentExecution = exec;
            Expression = full;
        }

        public Variable(StringSegment full, StringSegment name, int[] indices, Executer exec)
        {
            CurrentExecution = exec;
            _expression = full;
            _variable = name;
            Indices = indices;
        }

        private StringSegment GetName(StringSegment str)
        {
            int bracket = str.IndexOf('[');
            int space = str.IndexOf(' ');
            if (bracket < 0 && space < 0) {
                return str;
            }
            else if (bracket < 0 && space > 0) {
                return str.Remove(space);
            }
            else if (space < 0 && bracket > 0) {
                return str.Remove(bracket);
            }
            else if (space < bracket) {
                return str.Remove(space);
            }
            else {
                return str.Remove(bracket);
            }
        }

        public override string ToString()
        {
            return _expression.ToString();
        }

        public object Evaluate()
        {
            object obj = CurrentContext.GetVariable(Name.ToString());
            if (Indices != null) {
                obj = CurrentContext.GetArrayAt(Name.ToString(), Indices);
            }
            return obj;
        }

        public string GetFullName()
        {
            return GetFullName(Name.ToString(), Indices);
        }

        public static string GetFullName(string name, int[] indices)
        {
            StringBuilder sb = new StringBuilder(name);
            if (indices != null && indices.Length > 0) {
                sb.Append("[");
                for (int i = 0; i < indices.Length; ++i) {
                    sb.AppendFormat("{0},", indices[i]);
                }
                sb.AppendFormat("{0}]", indices[0]);
            }
            return sb.ToString();
        }

        public static object ConvertToObject(object _oObj)
        {
            if (_oObj == null) {
                return 0;
            }
            int? _iObj = _oObj as int?;
            if (_iObj != null)
                return _iObj.Value;
            
            double? _dObj = _oObj as double?;
            if (_dObj != null) {
                Number n = new Number(_dObj.Value);
                if (!n.HasFraction())
                    return n.ToInt();
            }

            decimal? _mObj = _oObj as decimal?;
            if (_mObj != null) {
                Number n = new Number(_dObj.Value);
                if (!n.HasFraction())
                    return n.ToInt();
            }

            IntPtr? _pObj = _oObj as IntPtr?;
            if (_pObj != null) {
                return ConvertToObject(_pObj.Value);
            }

            return _oObj;
        }

        public static object ConvertToObject(IntPtr ptr)
        {
            if (IntPtr.Size == sizeof(long)) { // 64-bit
                return ptr.ToInt64();
            }
            else { // 32-bit
                return ptr.ToInt32();
            }
        }
    }
}