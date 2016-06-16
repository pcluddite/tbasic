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
using System.Text;
using System.Text.RegularExpressions;
using Tbasic.Errors;
using Tbasic.Parsing;
using Tbasic.Components;

namespace Tbasic.Runtime
{
    /// <summary>
    /// This class provides functionality for evaluating functions
    /// </summary>
    internal class Function : IExpression
    {
        #region Private Members

        private StringSegment _expression;
        private StringSegment _function;
        private bool _bParsed;
        private IList<object> _params;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the expression to be evaluated
        /// </summary>
        /// <value></value>
        public StringSegment Expression
        {
            get { return _expression; }
            set {
                _expression = value;
                _function = StringSegment.Empty;
                _bParsed = false;
                _params = null;
            }
        }

        public int LastIndex { get; private set; }

        public StringSegment Name
        {
            get {
                if (StringSegment.IsNullOrEmpty(_function)) {
                    int index = _expression.IndexOf('(');
                    if (index < 1) {
                        throw new FormatException("string is not a function");
                    }
                    return _function = _expression.Remove(index);
                }
                else {
                    return _function;
                }
            }
        }

        public ObjectContext CurrentContext { get { return CurrentExecution.Context; } }

        public Executer CurrentExecution { get; set; }

        #endregion

        #region Construction

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Function(Executer exec)
        {
            CurrentExecution = exec;
        }

        /// <summary>
        /// Initializes the Expression Property
        /// </summary>
        /// <param name="expression">Expression to evaluate</param>
        /// <param name="exec">the current context</param>
        public Function(StringSegment expression, Executer exec)
        {
            CurrentExecution = exec;
            Expression = expression;
        }

        #endregion

        #region Methods

        public void Parse()
        {
            if (!_bParsed) {
                _params = GetParameters();
                _bParsed = true;
                if (LastIndex < _expression.Length - 1) {
                    _expression = _expression.Remove(LastIndex + 1);
                }
            }
        }

        /// <summary>
        /// Evaluates the Expression
        /// </summary>
        /// <returns></returns>
        public object Evaluate()
        {
            if (Executer.ExitRequest) {
                return null;
            }
            Parse();
            return ExecuteFunction(_function, _params);
        }

        /// <summary>
        /// Evaluates a string expression of a function
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="exec">the current execution</param>
        /// <returns>evauluated value</returns>
        public static object Evaluate(StringSegment expression, Executer exec)
        {
            if (Executer.ExitRequest) {
                return null;
            }
            Function expr = new Function(expression, exec);
            return expr.Evaluate();
        }
        
        /// <summary>
        /// string override, return Expression property
        /// </summary>
        /// <returns>returns Expression property</returns>
        public override string ToString() { return Expression.ToString(); }

        /// <summary>
        /// returns the parameters of a function
        /// </summary>
        /// <returns></returns>
        public IList<object> GetParameters()
        {
            IList<object> result;
            LastIndex = GroupParser.ReadGroup(_expression.ToString(),
                                            Name.Length, // Start at the end of the name
                                            CurrentExecution, out result);
            return result;
        }

        /// <summary>
        /// Executes the function based upon the name of the function
        /// </summary>
        /// <param name="_name">name of the function to execute</param>
        /// <param name="l_params">parameter list</param>
        /// <returns>returned value of executed function</returns>
        private object ExecuteFunction(StringSegment _name, IList<object> l_params)
        {
            if (Executer.ExitRequest) {
                return null;
            }
            string name = _name.Trim().ToString();
            object[] a_evaluated = null;
            if (l_params != null) {
                a_evaluated = new object[l_params.Count];
                l_params.CopyTo(a_evaluated, 0);
                for (int x = 0; x < a_evaluated.Length; x++) {
                    IExpression expr = a_evaluated[x] as IExpression;
                    if (expr != null) {
                        a_evaluated[x] = expr.Evaluate();
                    }
                }
            }
            ObjectContext context = CurrentContext.FindFunctionContext(name);
            if (context == null) {
                throw ThrowHelper.UndefinedFunction(name);
            }
            else {
                TFunctionData _sframe = new TFunctionData(CurrentExecution);
                _sframe.SetAll(a_evaluated);
                _sframe.Name = name;
                context.GetFunction(name).Invoke(_sframe);
                CurrentContext.SetReturns(_sframe);
                return _sframe.Data;
            }
        }

        #endregion
    }
}
