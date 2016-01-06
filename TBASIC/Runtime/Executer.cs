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
using System.Linq;
using System.Text;
using Tbasic.Libraries;

namespace Tbasic.Runtime
{
    /// <summary>
    /// An event handler for a user exit
    /// </summary>
    /// <param name="sender">the object that has sent this message</param>
    /// <param name="e">any additional event arguments</param>
    public delegate void UserExittedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Executes a script and stores information on the current state of the line being executed
    /// </summary>
    public class Executer
    {
        /// <summary>
        /// A string containing information on this version of Tbasic
        /// </summary>
        public const string VERSION = "TBASIC 2.0.2015";

        #region properties
        /// <summary>
        /// The global context for this ScriptRunner
        /// </summary>
        public ObjectContext Global { get; private set; }

        /// <summary>
        /// Gets if request to break has been petitioned
        /// </summary>
        public bool BreakRequest { get; internal set; }

        /// <summary>
        /// Gets the current line in the script that the executer is processing
        /// </summary>
        public int CurrentLine { get; internal set; }

        /// <summary>
        /// Gets or sets the ObjectContext in which the code is executed
        /// </summary>
        public ObjectContext Context { get; set; }

        /// <summary>
        /// Gets if a request to exit has been petitioned. This should apply to the scope of the whole application.
        /// </summary>
        public static bool ExitRequest { get; internal set; }

        /// <summary>
        /// Raised when a user has requested to exit
        /// </summary>
        public event UserExittedEventHandler OnUserExitRequest;

        #endregion

        /// <summary>
        /// Initializes a new script executer
        /// </summary>
        public Executer()
        {
            Global = new ObjectContext(null);
            Context = Global;
            CurrentLine = 0;
            BreakRequest = false;
        }

        /// <summary>
        /// Runs a Tbasic script
        /// </summary>
        /// <param name="script">the full text of the script to process</param>
        public void Execute(string script)
        {
            Execute(script.Replace("\r\n", "\n").Split('\n'));
        }

        /// <summary>
        /// Runs a Tbasic script
        /// </summary>
        /// <param name="lines">the lines of the script to process</param>
        public void Execute(string[] lines)
        {
            CodeBlock[] userFuncs;
            LineCollection code = ScanLines(lines, out userFuncs);

            if (userFuncs != null && userFuncs.Length > 0) {
                foreach (CodeBlock cb in userFuncs) {
                    FuncBlock fBlock = (FuncBlock)cb;
                    Global.SetFunction(fBlock.Template.Name, fBlock.CreateDelegate());
                }
            }
            Execute(code);

            /*if (ManagedWindows.Count != 0 && !this.ExitRequest) {
                System.Windows.Forms.Application.Run(new FormLoader(this));
            }*/
        }

        internal StackFrame Execute(LineCollection lines)
        {
            StackFrame stackFrame = new StackFrame(this);
            for (int index = 0; index < lines.Count; index++) {
                if (BreakRequest) {
                    break;
                }
                Line current = lines[index];
                CurrentLine = current.LineNumber;
                try {
                    ObjectContext blockContext = Context.FindBlockContext(current.Name);
                    if (blockContext == null) {
                        Execute(ref stackFrame, current);
                    }
                    else {
                        CodeBlock block = blockContext.GetBlock(current.Name).Invoke(index, lines);
                        Context = Context.CreateSubContext();
                        block.Execute(this);
                        Context = Context.Collect();
                        index += block.Length - 1; // skip the length of the executed block
                    }
                }
                catch (Exception ex) {
                    throw new ScriptException(current.LineNumber, current.VisibleName,
                        new ScriptException(current.LineNumber, current.VisibleName, ex));
                }
            }
            return stackFrame;
        }

        internal static void Execute(ref StackFrame stackFrame, Line codeLine)
        {
            ObjectContext context = stackFrame.Context.FindCommandContext(codeLine.Name);
            if (context == null) {
                Evaluator eval = new Evaluator(codeLine.Text, stackFrame.StackExecuter);
                object result = eval.Evaluate();
                stackFrame.Context.PersistReturns(stackFrame);
                stackFrame.Data = result;
            }
            else {
                stackFrame.SetAll(codeLine.Text);
                context.GetCommand(codeLine.Name).Invoke(ref stackFrame);
            }
            stackFrame.Context.SetReturns(stackFrame);
        }

        internal static LineCollection ScanLines(string[] lines, out CodeBlock[] userFunctions)
        {
            LineCollection allLines = new LineCollection();
            List<int> funLines = new List<int>();

            for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++) {
                Line current = new Line(lineNumber + 1, lines[lineNumber]); // Tag all lines with its line number (index + 1)

                if (current.Text.StartsWith(";") || current.Text.Equals("")) {
                    continue;
                }
                if (current.Name.EndsWith("$") || current.Name.EndsWith("]")) {
                    current = new Line(current);
                    current.Text = "LET " + current.Text; // add the word LET if it's an equality
                }
                else if (current.Name.Equals("FUNCTION", StringComparison.OrdinalIgnoreCase)) {
                    funLines.Add(current.LineNumber);
                }
                else {
                    current.VisibleName = current.VisibleName.ToUpper(); // name shown to the user
                }

                while (current.Text.EndsWith("_")) { // line continuation
                    lineNumber++;
                    if (lineNumber >= lines.Length) {
                        throw new EndOfCodeException(lineNumber, "line continuation character '_' cannot end script");
                    }
                    current = new Line(current.LineNumber, current.Text.Remove(current.Text.LastIndexOf('_')) + lines[lineNumber].Trim());
                }

                allLines.Add(current);
            }

            List<CodeBlock> userFuncs = new List<CodeBlock>();
            foreach (int funcLine in funLines) {
                FuncBlock func = new FuncBlock(allLines.IndexOf(funcLine), allLines);
                userFuncs.Add(func);
                allLines.Remove(func.Header);
                allLines.Remove(func.Body);
                allLines.Remove(func.Footer);
            }
            userFunctions = userFuncs.ToArray();
            return allLines;
        }

        internal void RequestBreak()
        {
            BreakRequest = true;
        }

        internal void HonorBreak()
        {
            if (!ExitRequest) {
                BreakRequest = false;
            }
        }

        internal void RequestExit()
        {
            ExitRequest = true;
            BreakRequest = true;
            OnUserExit(EventArgs.Empty);
        }

        private void OnUserExit(EventArgs e)
        {
            if (OnUserExitRequest != null) {
                OnUserExitRequest(this, e);
            }
        }
    }
}
