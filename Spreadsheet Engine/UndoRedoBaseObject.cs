/* Matthew R. Jones SID: 11566314
 * Homework 8+ (Spreadsheet Project)
 * UndoRedoBaseObject.cs : Contains the parent class from which all other Undo-Redo objects should be instantiated.
 */
namespace Spreadsheet_Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Parent class from which all other Undo-Redo objects should be instantiated.
    /// </summary>
    public class UndoRedoBaseObject
    {
        private string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoRedoBaseObject"/> class.
        /// The base constructor for an UndoRedoBaseObject.
        /// </summary>
        /// <param name="descriptionString">A string describing what was undone or redone.</param>
        public UndoRedoBaseObject(string descriptionString)
        {
            description = descriptionString;
        }

        /// <summary>
        /// Gets the description of the undo or redo object.
        /// </summary>
        public string Description
        {
            get { return description; }
        }

        /// <summary>
        /// Gets the description of the undo or redo object.
        /// </summary>
        /// <returns>A list of strings representing commands to the spreadsheet.</returns>
        public virtual List<string> CommandString()
        {
            return new List<string> { };
        }
    }
}
