/* Matthew R. Jones SID: 11566314
 * Homework 8+ (Spreadsheet Project)
 * UndoRedoCellColorChange : Child class for undoing and redoing color changes.
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
    public class UndoRedoCellColorChange : UndoRedoBaseObject
    {
        private List<string> commandList = new List<string> { };

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoRedoCellColorChange"/> class.
        /// </summary>
        /// <param name="descriptionString">String describing what is being changed.</param>
        /// <param name="commands">A list of pre-generated commands. Yes this is cheating. Given what a pain in the ass working without pointers is turning out to be, I don't care.</param>
        public UndoRedoCellColorChange(string descriptionString, List<string> commands)
            : base(descriptionString)
        {
            commandList = commands;
        }

        /// <summary>
        /// Returns the command string.
        /// </summary>
        /// <returns>Returns a command string in the form of "PARAMETER~priorValue~newValue~cellX~cellY".</returns>
        public override List<string> CommandString()
        {
            return commandList;
        }
    }
}
