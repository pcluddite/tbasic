/**
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
using System.Collections.Generic;
using Tbasic.Runtime;

namespace Tbasic.Libraries {
    
    /// <summary>
    /// Delegate for processing a TBasic function
    /// </summary>
    /// <param name="stack">The StackFrame containing parameter and execution information</param>
    public delegate void TBasicFunction(ref StackFrame stack);

    /// <summary>
    /// A library for storing and processing TBasic functions
    /// </summary>
    public class Library : Dictionary<string, TBasicFunction> {
        /// <summary>
        /// Initializes a new Tbasic Library object
        /// </summary>
        public Library()
            : base(StringComparer.CurrentCultureIgnoreCase) {
        }

        /// <summary>
        /// Initializes a new Tbasic Library object
        /// </summary>
        /// <param name="libs">a collection of Library objects that should be incorporated into this one</param>
        public Library(ICollection<Library> libs)
            : base(StringComparer.CurrentCultureIgnoreCase) {
            foreach (Library lib in libs) {
                AddLibrary(lib);
            }
        }

        /// <summary>
        /// Adds a Tbasic Library to this one
        /// </summary>
        /// <param name="lib">the Tbasic Library</param>
        public void AddLibrary(Library lib) {
            foreach (var kv_entry in lib) {
                Add(kv_entry.Key, kv_entry.Value);
            }
        }
    }
}
