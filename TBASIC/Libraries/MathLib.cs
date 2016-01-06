﻿/**
 *  TBASIC 2.0
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
using System.Globalization;
using Tbasic.Runtime;

namespace Tbasic.Libraries {
    /// <summary>
    /// A library containing several mathmatical functions
    /// </summary>
    public class MathLib : Library {

        private static Random rand = new Random();

        /// <summary>
        /// Initializes a new instance of this class
        /// </summary>
        public MathLib(ObjectContext context) {
            Add("POW", Pow);
            Add("IPART", iPart);
            Add("FPART", fPart);
            Add("ROUND", Round);
            Add("EVAL", Eval);
            Add("RANDOM", Rand);
            Add("NOT", Not);
            Add("ABS", Abs);
            Add("SIN", Sin);
            Add("ASIN", Asin);
            Add("SINH", Sinh);
            Add("COS", Cos);
            Add("ACOS", Acos);
            Add("COSH", Cosh);
            Add("TAN", Tan);
            Add("ATAN", Atan);
            Add("LOG", Log);
            Add("LN", Ln);
            context.SetConstant("@PI", Math.PI); // pi
            context.SetConstant("@E", Math.E); // euler's number
        }

        private void Log(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(2);
            stackFrame.Data = Math.Log10(stackFrame.Get<double>(1));
        }

        private void Ln(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(2);
            stackFrame.Data = Math.Log(stackFrame.Get<double>(1));
        }

        private void Abs(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(2);
            stackFrame.Data = Math.Abs(stackFrame.Get<double>(1));
        }

        private void Sin(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(2);
            stackFrame.Data = Math.Sin(stackFrame.Get<double>(1));
        }

        private void Asin(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(2);
            stackFrame.Data = Math.Asin(stackFrame.Get<double>(1));
        }

        private void Sinh(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(2);
            stackFrame.Data = Math.Sinh(stackFrame.Get<double>(1));
        }

        private void Cos(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(2);
            stackFrame.Data = Math.Cos(stackFrame.Get<double>(1));
        }

        private void Acos(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(2);
            stackFrame.Data = Math.Acos(stackFrame.Get<double>(1));
        }

        private void Cosh(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(2);
            stackFrame.Data = Math.Cosh(stackFrame.Get<double>(1));
        }

        private void Tan(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(2);
            stackFrame.Data = Math.Tan(stackFrame.Get<double>(1));
        }

        private void Atan(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(2);
            stackFrame.Data = Math.Atan(stackFrame.Get<double>(1));
        }

        private void Tanh(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(2);
            stackFrame.Data = Math.Tanh(stackFrame.Get<double>(1));
        }

        private void Not(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(2);
            Evaluator e = new Evaluator(stackFrame.Get<string>(1),stackFrame.StackExecuter);
            try {
                stackFrame.Data = !e.EvaluateBool();
            }
            catch {
                stackFrame.Status = 1;
            }
        }

        /// <summary>
        /// Returns a pseudo-random double between 0 and 1
        /// </summary>
        /// <returns>a pseudo-random double between 0 and 1</returns>
        public static double Rand() {
            return rand.NextDouble();
        }

        private void Rand(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(1);
            stackFrame.Data = Rand();
        }

        /// <summary>
        /// Rounds a double value to a given number of places
        /// </summary>
        /// <param name="number">the number to round</param>
        /// <param name="places">the number of places</param>
        /// <returns>the rounded double</returns>
        public static double Round(double number, int places) {
            return Math.Round(number, places);
        }

        private void Round(ref StackFrame stackFrame) {
            if (stackFrame.Count == 2) { 
                stackFrame.Add(2);
            }
            stackFrame.AssertArgs(3);
            stackFrame.Data = Round(stackFrame.Get<double>(1), stackFrame.Get<int>(2));
        }

        /// <summary>
        /// Returns the integer part of a double value
        /// </summary>
        /// <param name="d">the double to truncate</param>
        /// <returns>the truncated double</returns>
        public static int iPart(double d) {
            return (int)d;
        }

        private void iPart(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(2);
            stackFrame.Data = iPart(stackFrame.Get<double>(1));
        }

        /// <summary>
        /// Returns the fractional part of a double value
        /// </summary>
        /// <param name="d">the double to truncate</param>
        /// <returns>the truncated double</returns>
        public static double fPart(double d) {
            // TEST THIS! I'm just assuming that it works, but you know what they say about assuming...
            return (double)(d - (int)d);
        }

        private void fPart(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(2);
            stackFrame.Data = fPart(stackFrame.Get<double>(1));
        }

        /// <summary>
        /// Evaluates a mathmatical expression
        /// </summary>
        /// <param name="expr">the expression to evaluate</param>
        /// <returns>the evaluated expression</returns>
        public static object Eval(string expr) {
            Executer e = new Executer(); // local execution
            e.Global.AddLibrary(new MathLib(e.Global)); // only allow math libs
            e.Global.SetFunction("eval", null); // that's a no-no
            e.Global.SetFunction("not", null); // also a no-no
            return Evaluator.Evaluate(expr, e);
        }

        private void Eval(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(2);
            try {
                stackFrame.Data = Eval(stackFrame.Get<string>(1));
            }
            catch {
                stackFrame.Status = 1;
            }
        }

        private static void Pow(ref StackFrame stackFrame) {
            stackFrame.AssertArgs(3);
            stackFrame.Data = Pow(stackFrame.Get<double>(1), stackFrame.Get<double>(2));
        }

        /// <summary>
        /// Raises a number to a given exponent
        /// </summary>
        /// <param name="dBase">the base</param>
        /// <param name="dPower">the exponent</param>
        /// <returns>the evaluated base raised to the given exponent</returns>
        public static double Pow(double dBase, double dPower) {
            return Math.Pow(dBase, dPower);
        }
    }
}