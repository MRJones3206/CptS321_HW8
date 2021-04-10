/* Matthew R. Jones SID: 11566314
 * Homework 8+ (Spreadsheet Project)
 * UndoRedoTextChange : Child class for undoing and redoing text value changes.
 */
namespace Spreadsheet_Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Container class for undoing and redoing a text change.
    /// </summary>
    public class UndoRedoTextChange : UndoRedoBaseObject
    {
        private string oldText;
        private string newText;
        private int cellColumn;
        private int cellRow;

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoRedoTextChange"/> class.
        /// </summary>
        /// <param name="descriptionString">String describing what is being changed.</param>
        /// <param name="oldCellText">The old text in the cell.</param>
        /// <param name="newCellText">The new text in the cell.</param>
        /// <param name="cellColumnVal">The cell column.</param>
        /// <param name="cellRowVal">The cell row.</param>
        public UndoRedoTextChange(string descriptionString, string oldCellText, string newCellText, int cellColumnVal, int cellRowVal)
            : base(descriptionString)
        {
            oldText = oldCellText;
            newText = newCellText;
            cellColumn = cellColumnVal;
            cellRow = cellRowVal;
        }

        /// <summary>
        /// Returns the command string.
        /// </summary>
        /// <returns>Returns a command string in the form of "PARAMETER~priorValue~newValue~cellX~cellY".</returns>
        public override List<string> CommandString()
        {
            string container = "TEXT~" + oldText + "~" + newText + "~" + cellColumn + "~" + cellRow;
            return new List<string> { container };
        }
    }
}
