/**
 *  TBASIC
 *  Copyright (C) 2016 Timothy Baxendale
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

namespace Tbasic.Runtime
{
    /// <summary>
    /// This class provides functionality for evaluating functions
    /// </summary>
    internal class Function : IExpression
    {
        #region Private Members

        private string _expression = "";
        private string _function = "";
        private bool _bParsed;
        private IList<object> _params;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the expression to be evaluated
        /// </summary>
        /// <value></value>
        public string Expression
        {
            get { return _expression; }
            set {
                _expression = value;
                _function = "";
                _bParsed = false;
                _params = null;
            }
        }

        public int LastIndex { get; private set; }

        public string Name
        {
            get {
                if (_function == null || _function.Equals("")) {
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
        public Function(string expression, Executer exec)
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
                _expression = _expression.Remove(LastIndex) + ")";
            }
        }

        /// <summary>
        /// Evaluates the Expression
        /// </summary>
        /// <returns></returns>
        public object Evaluate()
        {
            if (Executer.ExitRequest) {
                return CurrentContext.GetVariable("@return");
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
        public static object Evaluate(string expression, Executer exec)
        {
            if (Executer.ExitRequest) {
                return exec.Context.GetVariable("@return");
            }
            Function expr = new Function(expression, exec);
            return expr.Evaluate();
        }

        /// <summary>
        /// This routine will replace functions existing in a input string with thier respective values
        /// </summary>
        /// <param name="input">input string</param>
        /// <param name="exec">the current context</param>
        /// <returns>input string with all found functions replaced with returned values</returns>
        public static string Replace(string input, Executer exec)
        {
            Function expr = new Function(input, exec);
            return expr.Replace();
        }

        /// <summary>
        /// Since the static replace will not allow a second Replace(string), Replace(ex) will do so with
        /// this instance (so that variables will work)
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>filtered string</returns>
        public string ReplaceEx(string input)
        {
            if ("" + input == "")
                return "";
            Expression = input;
            return Replace();
        }

        /// <summary>
        /// This routine will replace functions existing in the Expression property with thier respective values
        /// </summary>
        /// <returns>Expression string with all found functions replaced with returned values</returns>
        public string Replace()
        {
            StringBuilder strbRet = new StringBuilder(Expression);
            Match m = DefinedRegex.Function.Match(Expression);

            while (m.Success) {
                int nDepth = 1;
                int nIdx = m.Index + m.Length;
                //Get the parameter string
                while (nDepth > 0) {
                    if (nIdx >= strbRet.Length) {
                        throw new ArgumentException("Missing ')' in Expression");
                    }
                    if (strbRet[nIdx] == ')')
                        nDepth--;
                    if (strbRet[nIdx] == '(')
                        nDepth++;
                    nIdx++;
                }
                string expression = strbRet.ToString(m.Index, nIdx - m.Index);
                Function eval = new Function(expression, CurrentExecution);
                strbRet.Replace(expression, "" + eval.Evaluate());
                m = DefinedRegex.Function.Match(strbRet.ToString());
            }

            //Replace Variable in the path!
            m = DefinedRegex.Variable.Match(strbRet.ToString());
            while (m.Success) {
                strbRet.Replace(m.Value, "" + CurrentContext.GetVariable(m.Value));
                m = DefinedRegex.Variable.Match(strbRet.ToString());
            }

            return strbRet.ToString();
        }

        /// <summary>
        /// string override, return Expression property
        /// </summary>
        /// <returns>returns Expression property</returns>
        public override string ToString() { return Expression; }

        /// <summary>
        /// returns the parameters of a function
        /// </summary>
        /// <returns></returns>
        public IList<object> GetParameters()
        {
            IList<object> result;
            LastIndex = Evaluator.ReadGroup(_expression,
                                            Name.Length, // Start at the end of the name
                                            CurrentExecution, out result);
            return result;
        }

        /// <summary>
        /// Executes the function based upon the name of the function
        /// </summary>
        /// <param name="name">name of the function to execute</param>
        /// <param name="l_params">parameter list</param>
        /// <returns>returned value of executed function</returns>
        private object ExecuteFunction(string name, IList<object> l_params)
        {
            if (Executer.ExitRequest) {
                return CurrentContext.GetVariable("@return");
            }
            name = name.Trim();
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
                throw ScriptException.UndefinedObject(name);
            }
            else {
                StackFrame _sframe = new StackFrame(CurrentExecution);
                _sframe.SetAll(a_evaluated);
                _sframe.Name = name;
                context.GetFunction(name).Invoke(ref _sframe);
                CurrentContext.SetReturns(_sframe);
                return _sframe.Data;
            }
        }

        #endregion
    }
}
