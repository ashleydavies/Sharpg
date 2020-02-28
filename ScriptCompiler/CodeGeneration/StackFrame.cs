﻿using System.Collections.Generic;
using System.Linq;
using ScriptCompiler.Types;

namespace ScriptCompiler.CompileUtil {
    public class StackFrame {
        // Used to track the portion of the stack frame reserved for the return value for a function to populate.
        // Accessed by the code generator for the `return` statement.
        public const string ReturnIdentifier = "!RETURN";
        // Sometimes we want to push inaccessible variables to the stack, such as a return address during preparation
        //  for entering a function. These affect the access to stack variables (they are all now further down the
        //  stack) and therefore we need to keep track of how much we've "nudged" these things onto the stack
        public int Length { get; private set; }
        private readonly StackFrame? _parent;
        private readonly Dictionary<string, (SType type, int )> _variableTable;

        public StackFrame(StackFrame parent) : this() {
            _parent = parent;
        }

        public StackFrame() {
            _variableTable = new Dictionary<string, (SType, int)>();
        }

        public (StackFrame? parent, int length) Purge() {
            var expectedLength = _variableTable.Values.Sum(v => v.type.Length);
            
            if (Length != expectedLength) {
                throw new CompileException(
                    $"Unexpected call to StackFrame::Purge() - not all locals popped? {Length - expectedLength} words left", 
                    0, 0);
            }

            return (_parent, expectedLength);
        }

        public bool ExistsLocalScope(string identifier) {
            return _variableTable.ContainsKey(identifier);
        }
        
        /// <summary>
        /// Gets the offset of a given identifier from the top of the stack, along with the type, providing it exists.
        /// Otherwise, returns a NoType type, with undefined behaviour for the offset in this case.
        /// </summary>
        public (SType type, int position) Lookup(string identifier) {
            if (ExistsLocalScope(identifier)) {
                var (type, pos) = _variableTable[identifier];
                return (type, -Length + pos);
            }
            
            if (_parent != null) {
                var (type, pos) = _parent.Lookup(identifier);
                return (type, pos - Length);
            }

            return (SType.SNoType, 0);
        }

        /// <summary>
        /// Adds an identifier with a given type to the stack, and adjusts the stack length (and all consequent
        /// accesses)
        /// </summary>
        public void AddIdentifier(SType type, string identifier) {
            _variableTable[identifier] = (type, Length);
            Length += type.Length;
        }

        /// <summary>
        /// Given some data, allows future accesses to stack variables to account for changes on the stack
        /// outside of the initial stack frame.
        /// </summary>
        public void Pushed(SType data) {
            Length += data.Length;
        }

        /// <summary>
        /// Undoes the changes made by Pushed for a given type
        /// </summary>
        public void Popped(SType data) {
            Length -= data.Length;
        }
    }
}