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
