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

namespace Tbasic.Runtime
{
    internal class Variable : IExpression
    {

        #region Private Members

        private string _expression;
        private string _variable = null;

        #endregion

        #region Properties

        public int[] Indices { get; private set; }

        public bool IsMacro
        {
            get {
                return Name.StartsWith("@");
            }
        }

        public bool IsValid
        {
            get {
                return Name.EndsWith("$");
            }
        }

        public ObjectContext CurrentContext
        {
            get {
                return CurrentExecution.Context;
            }
        }

        public Executer CurrentExecution { get; set; }

        public string Expression
        {
            get {
                return _expression;
            }
            set {
                _expression = value.Trim();
                if (_expression.Length > Name.Length) {
                    string index = _expression.Substring(Name.Length);
                    if (index.Trim().StartsWith("[")) {
                        IList<object> indices;
                        int last = GroupParser.ReadGroup(_expression, _expression.IndexOf('['), CurrentExecution, out indices);
                        _expression = value.Remove(last) + ']';
                        if (indices.Count == 0) {
                            throw ThrowHelper.NoIndexSpecified();
                        }
                        Indices = new int[indices.Count];
                        for (int i = 0; i < Indices.Length; i++) {
                            if (indices[i] is int) {
                                Indices[i] = (int)indices[i];
                            }
                            else {
                                throw ThrowHelper.InvalidExpression(indices[i].GetType().Name, typeof(int).Name);
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

        public string Name
        {
            get {
                if (_variable == null) {
                    _variable = GetName(_expression);
                }
                return _variable;
            }
        }

        #endregion

        public Variable(string full, Executer exec)
        {
            CurrentExecution = exec;
            Expression = full;
        }

        private string GetName(string str)
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
            return _expression;
        }

        public object Evaluate()
        {
            object obj = CurrentContext.GetVariable(Name);
            if (Indices != null) {
                for (int n = 0; n < Indices.Length; n++) {
                    if (obj.GetType().IsArray) {
                        object[] _aObj = (object[])obj;
                        if (Indices[n] < _aObj.Length) {
                            obj = _aObj[Indices[n]];
                        }
                        else {
                            throw ThrowHelper.IndexOutOfRange(BuildName(n - 1), Indices[n]);
                        }
                    }
                    else {
                        throw ThrowHelper.IndexUnavailable(BuildName(n));
                    }
                }
            }
            return DuckType(obj);
        }

        private string BuildName(int n)
        {
            StringBuilder sb = new StringBuilder(Name);
            if (n > 0) {
                sb.Append("[");
                while (n > 0) {
                    sb.AppendFormat("{0},", Indices[n]);
                    n--;
                }
                sb.AppendFormat("{0}]", Indices[0]);
            }
            return sb.ToString();
        }

        public static object DuckType(object _oObj)
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

            return _oObj;
        }
    }
}