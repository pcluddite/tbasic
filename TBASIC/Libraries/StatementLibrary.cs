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
using Tbasic.Runtime;
using System.IO;
using Tbasic.Errors;

namespace Tbasic.Libraries
{
    internal class StatementLibrary : Library
    {
        public StatementLibrary()
        {
            Add("#include", Include);
            Add("LET", LET);
            Add("EXIT", Exit);
            Add("BREAK", Break);
            Add("DIM", DIM);
            Add("SLEEP", Sleep);
            Add("ELSE", UhOh);
            Add("END", UhOh);
            Add("WEND", UhOh);
        }

        private void Include(StackFrame stackFrame)
        {
            stackFrame.AssertArgs(2);
            string path = Path.GetFullPath(stackFrame.Get<string>(1));
            if (!File.Exists(path)) {
                throw new FileNotFoundException();
            }

            CodeBlock[] funcs;
            LineCollection lines = Executer.ScanLines(File.ReadAllLines(path), out funcs);



            NULL(stackFrame);
        }

        private void Sleep(StackFrame stackFrame)
        {
            stackFrame.AssertArgs(2);
            System.Threading.Thread.Sleep(stackFrame.Get<int>(1));
            NULL(stackFrame);
        }

        private void Break(StackFrame stackFrame)
        {
            stackFrame.AssertArgs(1);
            stackFrame.StackExecuter.RequestBreak();
            NULL(stackFrame);
        }

        internal void Exit(StackFrame stackFrame)
        {
            stackFrame.AssertArgs(1);
            stackFrame.StackExecuter.RequestExit();
            NULL(stackFrame);
        }

        internal static void NULL(StackFrame stackFrame)
        {
            stackFrame.Context.PersistReturns(stackFrame);
        }

        internal void UhOh(StackFrame stackFrame)
        {
            throw ScriptException.NoOpeningStatement(stackFrame.Text);
        }

        internal void Const(StackFrame stackFrame)
        {
            if (stackFrame.Count < 4) {
                stackFrame.AssertArgs(4);
            }
            if (!stackFrame.Get(2).Equals("=")) {
                throw ScriptException.InvalidOperatorDeclaration(stackFrame.Get(2));
            }
            Variable var = new Variable(stackFrame.Get<string>(1), stackFrame.StackExecuter);
            if (!var.IsValid) {
                throw ScriptException.InvalidVariableName(var.Name);
            }
            if (var.Indices == null) {
                Evaluator e = new Evaluator(stackFrame.Text.Substring(stackFrame.Text.IndexOf('=')), stackFrame.StackExecuter);
                stackFrame.StackExecuter.Context.SetConstant(stackFrame.Get<string>(1), e.Evaluate());

                NULL(stackFrame);
            }
            else {
                throw new ArgumentException("arrays cannot be defined as constants");
            }
        }

        internal void DIM(StackFrame stackFrame)
        {
            if (stackFrame.Count < 4) {
                stackFrame.AssertArgs(2);
            }
            if (stackFrame.Count > 2) {
                LET(stackFrame);
            }
            else {
                Variable v = new Variable(stackFrame.Get<string>(1), stackFrame.StackExecuter);
                ObjectContext context = stackFrame.StackExecuter.Context.FindVariableContext(v.Name);
                if (context == null) {
                    stackFrame.StackExecuter.Context.SetVariable(
                        v.Name,
                        array_alloc(v.Indices, 0));
                }
                else {
                    object obj = context.GetVariable(v.Name);
                    array_realloc(ref obj, v.Indices, 0);
                    context.SetVariable(v.Name, obj);
                }
                NULL(stackFrame);
            }
        }

        private object array_alloc(int[] sizes, int index)
        {
            if (index < sizes.Length) {
                object[] o = new object[sizes[index]];
                index++;
                for (int i = 0; i < o.Length; i++) {
                    o[i] = array_alloc(sizes, index);
                }
                return o;
            }
            else {
                return null;
            }
        }

        private void array_realloc(ref object o, int[] sizes, int index)
        {
            if (o != null) {
                if (o.GetType().IsArray) {
                    object[] _aObj = (object[])o;
                    if (index < sizes.Length) {
                        Array.Resize<object>(ref _aObj, sizes[index]);
                        index++;
                        for (int i = 0; i < _aObj.Length; i++) {
                            array_realloc(ref _aObj[i], sizes, index);
                        }
                        o = _aObj;
                    }
                }
                else {
                    o = array_alloc(sizes, index);
                }
            }
        }

        internal void LET(StackFrame stackFrame)
        {
            if (stackFrame.Count < 4) {
                stackFrame.AssertArgs(4);
            }
            if (!stackFrame.Get(2).Equals("=")) {
                throw ScriptException.InvalidOperatorDeclaration(stackFrame.Get(2));
            }

            Evaluator e = new Evaluator(
                stackFrame.Text.Substring(stackFrame.Text.IndexOf('=') + 1),
                stackFrame.StackExecuter);

            object data = e.Evaluate();

            stackFrame.StackExecuter.Context.SetVariable(
                stackFrame.Get<string>(1),
                data
                );

            NULL(stackFrame);
        }
    }
}