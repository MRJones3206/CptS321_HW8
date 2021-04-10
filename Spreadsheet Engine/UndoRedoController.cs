/* Matthew R. Jones SID: 11566314
 * Homework 8+ (Spreadsheet Project)
 * UndoRedoController.cs : Contains an instantiable object called UndoRedoController, which is in charge of handling all undo and redo operations.
 */
namespace Spreadsheet_Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CptS321;

    /// <summary>
    /// Contains UndoRedoController - In charge of handling all undo and redo operations.
    /// </summary>
    public class UndoRedoController
    {
        private Stack<UndoRedoBaseObject> undoStack = new Stack<UndoRedoBaseObject> { };
        private Stack<UndoRedoBaseObject> redoStack = new Stack<UndoRedoBaseObject> { };

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoRedoController"/> class.
        /// </summary>
        public UndoRedoController()
        {
            // .
        }

        /// <summary>
        /// Gets the string describing what the current top undo object is doing.
        /// </summary>
        /// <returns>The string describing what the current top undo object is doing.</returns>
        public string UndoDescription()
        {
            if (undoStack.Count != 0)
            {
                return undoStack.Peek().Description;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the string describing what the current top redo object is doing.
        /// </summary>
        /// <returns>The string describing what the current top redo object is doing.</returns>
        public string RedoDescription()
        {
            if (redoStack.Count != 0)
            {
                return redoStack.Peek().Description;
            }

            return string.Empty;
        }

        /// <summary>
        /// Tell the Spreadsheet how to undo the most recent action. If no action, do nothing.
        /// </summary>
        /// <returns>A list of commands to the spreadsheet.</returns>
        public List<string> Undo()
        {
            if (undoStack.Count != 0)
            {
                UndoRedoBaseObject act = undoStack.Pop();
                redoStack.Push(act);
                return act.CommandString();
            }

            return new List<string> { };
        }

        /// <summary>
        /// Tell the Spreadsheet how to redo the most recent action. If no action, do nothing.
        /// </summary>
        /// <returns>A list of commands to the spreadsheet.</returns>
        public List<string> Redo()
        {
            if (redoStack.Count != 0)
            {
                UndoRedoBaseObject act = redoStack.Pop();
                undoStack.Push(act);
                return act.CommandString();
            }

            return new List<string> { };
        }

        /// <summary>
        /// Add an action to the undo or redo list.
        /// </summary>
        /// <param name="action">The action to undo or redo.</param>
        public void AddUndoAction(UndoRedoBaseObject action)
        {
            undoStack.Push(action);
        }
    }
}
