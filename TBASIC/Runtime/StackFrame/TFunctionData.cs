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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Tbasic.Errors;

namespace Tbasic.Runtime
{
    /// <summary>
    /// Manages parameters and other data passed to a function or subroutine
    /// </summary>
    public class TFunctionData : IList<object>, ICloneable
    {
        private List<object> _params = new List<object>();

        /// <summary>
        /// The executer that called the function
        /// </summary>
        public Executer StackExecuter { get; private set; }

        /// <summary>
        /// Gets or sets the current context of stack executer
        /// </summary>
        public ObjectContext Context
        {
            get {
                return StackExecuter.Context;
            }
            set {
                StackExecuter.Context = value;
            }
        }

        /// <summary>
        /// The Tbasic function as text. This value cannot be changed after it has been set in the constructor.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets a parameter at a specified index
        /// </summary>
        /// <param name="index">the zero-based index of the parameter</param>
        /// <returns></returns>
        public object this[int index]
        {
            get {
                return Get(index);
            }
            set {
                Set(index, value);
            }
        }

        /// <summary>
        /// The name of the function (the first parameter)
        /// </summary>
        public string Name
        {
            get {
                if (_params != null && _params.Count > 0) {
                    return _params[0].ToString();
                }
                else {
                    return "";
                }
            }
            set {
                if (_params == null || _params.Count == 0) {
                    _params = new List<object>() { value };
                }
                else {
                    Insert(0, value);
                }
            }
        }

        /// <summary>
        /// Gets the number of parameters in this collection
        /// </summary>
        public int Count
        {
            get {
                return _params.Count;
            }
        }


        /// <summary>
        /// Gets or sets the status that the function returned
        /// </summary>
        public int Status { get; set; } = ErrorSuccess.OK;

        /// <summary>
        /// Gets or sets the return data for the function
        /// </summary>
        public object Data { get; set; } = null;

        /// <summary>
        /// Constructs a StackFrame object
        /// </summary>
        /// <param name="exec">the execution that called the function</param>
        public TFunctionData(Executer exec)
        {
            StackExecuter = exec;
        }

        /// <summary>
        /// Constructs a StackFrame object
        /// </summary>
        /// <param name="text">the text to be processed (formatted as a shell command)</param>
        /// <param name="exec">the execution that called the function</param>
        public TFunctionData(Executer exec, string text)
            : this(exec)
        {
            SetAll(text);
        }

        /// <summary>
        /// Sets the data for this StackFrame object
        /// </summary>
        /// <param name="parameters">parameters to manage</param>
        public void SetAll(params object[] parameters)
        {
            if (parameters == null) {
                _params = new List<object>();
            }
            else {
                _params = new List<object>(parameters);
            }
        }

        internal void SetAll(List<object> parameters)
        {
            if (parameters == null) {
                _params = new List<object>();
            }
            else {
                _params = new List<object>(parameters);
            }
        }

        /// <summary>
        ///  Sets the data for this StackFrame object
        /// </summary>
        /// <param name="message">the text to be processed (formatted as a shell command)</param>
        public void SetAll(string message)
        {
            Text = message;
            _params = new List<object>(ParseArguments(message));
        }

        private string[] ParseArguments(string commandLine)
        {
            commandLine = commandLine.Trim(); // just a precaution
            List<string> args = new List<string>();
            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < commandLine.Length; index++) {
                switch (commandLine[index]) {
                    case ' ':
                        if (index > 0 && commandLine[index - 1] == ' ') {
                            continue; // ignore extra whitespace
                        }
                        else {
                            args.Add(sb.ToString());
                            sb = new StringBuilder();
                        }
                        break;
                    case '\'':
                    case '"':
                        string parsed;
                        index = Evaluator.ReadString(commandLine, index, out parsed);
                        if (parsed.Length == 0) {
                            args.Add(parsed);
                        }
                        else {
                            sb.Append(parsed);
                        }
                        break;
                    default:
                        sb.Append(commandLine[index]);
                        break;
                }
            }
            if (sb.Length > 0) {
                args.Add(sb.ToString()); // Don't forget about me!
            }
            return args.ToArray();
        }

        /// <summary>
        /// Adds a parameter to this collection
        /// </summary>
        /// <param name="o">the new parameter to add</param>
        /// <return></return>
        public void Add(object o)
        {
            _params.Add(o);
        }

        /// <summary>
        /// Adds a range of parameters to this collection
        /// </summary>
        /// <param name="col_params">the new parameters to add</param>
        public void AddRange(ICollection<object> col_params)
        {
            _params.AddRange(col_params);
        }

        /// <summary>
        /// Assigns new data to a parameter
        /// </summary>
        /// <param name="index">The index of the argument</param>
        /// <param name="data">The new string data to assign</param>
        public void Set(int index, object data)
        {
            if (index < _params.Count) {
                _params[index] = data;
            }
        }

        /// <summary>
        /// Throws an ArgumentException if the number of parameters does not match a specified count
        /// </summary>
        /// <param name="count">the number of parameters expected</param>
        public void AssertArgs(int count)
        {
            if (_params.Count != count) {
                throw new ArgumentException(string.Format("{0} does not take {1} parameter{2}", Name.ToUpper(), _params.Count - 1,
                _params.Count == 2 ? "" : "s"));
            }
        }

        /// <summary>
        /// Returns the parameter at an index. Throws a IndexOutOfRangeException if the argument is out of bounds
        /// </summary>
        /// <param name="index">The index of the argument</param>
        /// <returns></returns>
        public object Get(int index)
        {
            if (index < _params.Count && index >= 0) {
                return _params[index];
            }
            throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Returns the object type at a given index
        /// </summary>
        /// <param name="index">index of the argument</param>
        /// <returns></returns>
        public Type GetTypeAt(int index)
        {
            return Get(index).GetType();
        }

        /// <summary>
        /// Returns the parameter as an integer if the integer is within a given range.
        /// </summary>
        /// <param name="index">the index of the argument</param>
        /// <param name="lower">the inclusive lower bound</param>
        /// <param name="upper">the inclusive upper bound</param>
        /// <returns></returns>
        public int GetIntRange(int index, int lower, int upper)
        {
            int n = Get<int>(index);
            if (n < lower || n > upper) {
                throw new FormatException(string.Format("parameter {0} expected to be integer between {1} and {2}", index, lower, upper));
            }
            return n;
        }

        /// <summary>
        /// Gets a parameter at a given index as a specified type. Throws an InvalidCastException if the object cannot be converted to the specified type
        /// </summary>
        /// <typeparam name="T">the type to convert the object</typeparam>
        /// <param name="index">the zero-based index of the parameter</param>
        /// <returns></returns>
        public T Get<T>(int index)
        {
            T ret;
            if (Evaluator.TryParse<T>(Get(index), out ret)) {
                return ret;
            }
            throw new InvalidCastException(string.Format("expected parameter {0} to be of type {1}", index, typeof(T).Name));
        }

        /// <summary>
        /// Gets an argument as a string. Throws a TException if the argument is out of bounds or is not listed as an exceptable string value
        /// </summary>
        /// <param name="index">The index of the argument</param>
        /// <param name="typeName">The type the argument represents</param>
        /// <param name="values">Acceptable string values</param>
        /// <returns></returns>
        internal string Get(int index, string typeName, params string[] values)
        {
            string arg = Get(index).ToString();
            foreach (string val in values) {
                if (val.EqualsIgnoreCase(arg)) {
                    return arg;
                }
            }
            throw new ArgumentException("expected parameter " + index + " to be of type " + typeName);
        }

        /// <summary>
        /// Clones this StackFrame
        /// </summary>
        /// <returns>A new StackFrame object with the same data</returns>
        public TFunctionData Clone()
        {
            TFunctionData clone = new TFunctionData(StackExecuter);
            clone.Text = Text;
            if (_params == null) {
                clone._params = new List<object>();
            }
            else {
                clone._params.AddRange(_params);
            }
            clone.Status = Status;
            clone.Data = Data;
            return clone;
        }

        /// <summary>
        /// Copies all properties of another StackFrame into this one
        /// </summary>
        /// <param name="other"></param>
        public void CopyFrom(TFunctionData other)
        {
            TFunctionData clone = other.Clone();
            StackExecuter = clone.StackExecuter;
            Text = clone.Text;
            _params = clone._params;
            Status = clone.Status;
            Data = clone.Data;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        /// <summary>
        /// Removes all elements from the collection
        /// </summary>
        public void Clear()
        {
            _params.Clear();
        }

        /// <summary>
        /// Determines whether an element exists in this collection
        /// </summary>
        /// <param name="value">the object to contains (value cannot be null)</param>
        /// <returns></returns>
        public bool Contains(object value)
        {
            return _params.Contains(value);
        }

        /// <summary>
        /// Searches for the specified object and returns the index of the first occourence
        /// </summary>
        /// <param name="value">the object to locate (value cannot be null)</param>
        /// <returns></returns>
        public int IndexOf(object value)
        {
            return _params.IndexOf(value);
        }

        /// <summary>
        /// Inserts an element at the specified index
        /// </summary>
        /// <param name="index">the zero-based index of the element to remove</param>
        /// <param name="value">the object to insert (value cannot be null)</param>
        public void Insert(int index, object value)
        {
            _params.Insert(index, value);
        }

        /// <summary>
        /// Removes an element at the specified index
        /// </summary>
        /// <param name="value">the object to remove (value cannot be null)</param>
        public bool Remove(object value)
        {
            return _params.Remove(value);
        }

        /// <summary>
        /// Removes an element at the specified index
        /// </summary>
        /// <param name="index">the zero-based index of the element to remove</param>
        public void RemoveAt(int index)
        {
            _params.RemoveAt(index);
        }

        bool ICollection<object>.IsReadOnly
        {
            get { return false; }
        }
        
        void ICollection<object>.CopyTo(object[] array, int index)
        {
            _params.CopyTo(array, index);
        }

        /// <summary>
        /// Returns this StackFrame enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<object> GetEnumerator()
        {
            return _params.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
