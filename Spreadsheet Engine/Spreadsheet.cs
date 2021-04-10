/* Matthew R. Jones SID: 11566314
 * Homework 4+ (Spreadsheet Project)
 * Spreadsheet.cs : Holds the Spreadsheet class.
 */
namespace CptS321
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Spreadsheet_Engine;

    /// <summary>
    /// Container class for the backend 'spreadsheet' object. In charge of handling all logic for the UI layer dataGridView1.
    /// </summary>
    public class Spreadsheet : INotifyPropertyChanged
    {
        private SpreadsheetCell[,] cellArray;
        private UndoRedoController undoRedo;
        private string[,] cellReferencedBy;
        private int columnCount;
        private int rowCount;

        // Used as a workaround for an issue with getting an individual cell location through to the Form from the Spreadsheet.
        private int callerColumn;
        private int callerRow;

        /// <summary>
        /// Initializes a new instance of the <see cref="Spreadsheet"/> class
        /// Initializes a spreadsheet of size specified by numColumn and numRow. Fills with SpreadsheetCells bound to the [numColumn][numRow] location.
        /// </summary>
        /// <param name="numColumn">An unsigned non-zero integer value representing the number of columns we want our spreadsheet to have.</param>
        /// <param name="numRow">An unsigned non-zero integer value representing the number of rows we want our spreadsheet to have.</param>
        public Spreadsheet(int numColumn, int numRow)
        {
            this.cellArray = new SpreadsheetCell[numColumn, numRow];
            this.cellReferencedBy = new string[numColumn, numRow];
            this.columnCount = numColumn;
            this.rowCount = numRow;
            for (int col = 0; col < numColumn; col++)
            {
                for (int row = 0; row < numRow; row++)
                {
                    // Note that cellArray follows the winforms standard of Column > Row, rather than traditional reading, which would be Row > Col. This will be a problem in the future...
                    this.cellArray[col, row] = new SpreadsheetCell(row, col);
                    this.cellReferencedBy[col, row] = string.Empty;
                    this.cellArray[col, row].PropertyChanged += this.CellPropertyChanged;
                }
            }

            undoRedo = new UndoRedoController();
        }

        /// <summary>
        /// Component model magic.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the number of columns in the spreadsheet.
        /// </summary>
        public int ColumnCount
        {
            get { return columnCount; }
        }

        /// <summary>
        /// Gets the number of rows in the spreadsheet.
        /// </summary>
        public int RowCount
        {
            get { return rowCount; }
        }

        /// <summary>
        /// Gets the callerColumn value, a representation of the columnar location a caller can be found at.
        /// </summary>
        public int CallerColumn
        {
            get { return this.callerColumn; }
        }

        /// <summary>
        /// Gets the callerRow value, a representation of the row location a caller can be found at.
        /// </summary>
        public int CallerRow
        {
            get { return this.callerRow; }
        }

        /// <summary>
        /// Return an abstract cell reference with a given column and row value, or null if values are out of bounds.
        /// </summary>
        /// <param name="column">A column to look for our cell in.</param>
        /// <param name="row">A row to look for our cell in.</param>
        /// <returns>A reference to an abstract cell, or null if unsuccessful.</returns>
        public SpreadsheetCellAbstract GetCell(int column, int row)
        {
            if (column >= 0 && column < this.columnCount && row >= 0 && row < this.rowCount)
            {
                return this.cellArray[column, row];
            }

            return null;
        }

        /// <summary>
        /// Adds an undoable or redoable action for the spreadsheet to track.
        /// </summary>
        /// <param name="action">The action being tracked.</param>
        public void AddUndoRedoAction(UndoRedoBaseObject action)
        {
            undoRedo.AddUndoAction(action);
        }

        /// <summary>
        /// Query the undoRedo object and do whatever it tells you to do to undo something.
        /// </summary>
        public void DoUndo()
        {
            foreach (string command in undoRedo.Undo())
            {
                string[] commandlets = command.Split('~');
                int[] coordinates = new int[2];
                coordinates[0] = int.Parse(commandlets[3]);
                coordinates[1] = int.Parse(commandlets[4]);
                if (commandlets[0] == "TEXT")
                {
                    this.GetCell(coordinates[0], coordinates[1]).Text = commandlets[1];
                    this.GetCell(coordinates[0], coordinates[1]).ThrowPropertyChanged("Text");
                }
                else if (commandlets[0] == "COLOR")
                {
                    this.GetCell(coordinates[0], coordinates[1]).Color = uint.Parse(commandlets[1]);
                    this.GetCell(coordinates[0], coordinates[1]).ThrowPropertyChanged("Color");
                }
            }
        }

        /// <summary>
        /// Query the undoRedo object and do whatever it tells you to do to undo something.
        /// </summary>
        public void DoRedo()
        {
            foreach (string command in undoRedo.Redo())
            {
                string[] commandlets = command.Split('~');
                int[] coordinates = new int[2];
                coordinates[0] = int.Parse(commandlets[3]);
                coordinates[1] = int.Parse(commandlets[4]);
                if (commandlets[0] == "TEXT")
                {
                    this.GetCell(coordinates[0], coordinates[1]).Text = commandlets[2];
                    this.GetCell(coordinates[0], coordinates[1]).ThrowPropertyChanged("Text");
                }
                else if (commandlets[0] == "COLOR")
                {
                    this.GetCell(coordinates[0], coordinates[1]).Color = uint.Parse(commandlets[2]);
                    this.GetCell(coordinates[0], coordinates[1]).ThrowPropertyChanged("Color");
                }
            }
        }

        /// <summary>
        /// Return undo description.
        /// </summary>
        /// <returns>If an item exists in the undo stack, returns its description. Otherwise returns empty string.</returns>
        public string UndoDescription()
        {
            return undoRedo.UndoDescription();
        }

        /// <summary>
        /// Return redo description.
        /// </summary>
        /// <returns>If an item exists in the redo stack, returns its description. Otherwise returns empty string.</returns>
        public string RedoDescription()
        {
            return undoRedo.RedoDescription();
        }

        /// <summary>
        /// Extraction of the initialization process for variables. Moved more for code cleanlieness than anything else.
        /// This method, when given a ExpressionTree and SpreadsheetCell object, initializes the passed in tree with the variables it needs to calculate
        /// its value, as specified by the expression provided by caller.
        /// </summary>
        /// <param name="tree">Tree that contains the expression formed from the text of Caller.</param>
        /// <param name="caller">The cell that called an evaluation request.</param>
        /// <param name="cellReferenceString">Outputs a cellReferenceString for cell recursion checking.</param>
        /// <returns>True if an issue exists with the variables that prevent initialization, otherwise false.</returns>
        private bool InitializeExpressionVariables(ExpressionTree tree, SpreadsheetCell caller, out string cellReferenceString)
        {
            // For each uninitialized variable in the list, initialize it.
            // If we run into any issues, raise a flag in the unacceptableVariableFlag tag.
            bool unacceptableVariableFlag = false;

            // A container for all the cells that are referenced by this one (if any) that we must subscribe to if the expression parses ok.
            string newCellReferences = string.Empty;

            // We need this local because we are editing the remote one. It also can't be a hashset, because VS gets grumpy if it is one.
            List<string> uninitVariableList = tree.GetUninitializedVariableList().ToList<string>();
            foreach (string uninitVariable in uninitVariableList)
            {
                // If we have one unacceptable variable, don't bother doing the rest of the work, just skip the rest. We will be returning ERROR anyway.
                if (!unacceptableVariableFlag)
                {
                    // Get the column and row corresponding to the variable. Remember to subtract because we count from 1 .. n, but index from 0 .. n.
                    int column = caller.Text[0] - 61; // Works because ASCII 'A' is 65, so row 'A' is actually row 0.

                    // If our row isn't within our boundaries, we have an issue!
                    if (column < 0 || column > rowCount)
                    {
                        unacceptableVariableFlag = true;
                    }

                    int row = 0;

                    // If everything after the '=%c' is definitly an integer and matches the bounds of our spreadsheet...
                    if (!int.TryParse(uninitVariable.Remove(0, 1), out row))
                    {
                        unacceptableVariableFlag = true;
                    }

                    // Variables would have row indexes from 1 .. n, we index from 0 .. n internally.
                    row--;

                    // If the column is outside allowable bounds...
                    if (row < 0 || row > columnCount)
                    {
                        unacceptableVariableFlag = true;
                    }

                    // Get the Value (Ie, the true value result) of the targeted cell.
                    string targetCellStringValue = this.GetCell(column, row).Value;
                    double targetCellDoubleValue = 0.0;

                    // If it isn't a double, flag it for an unacceptable variable.
                    if (!double.TryParse(targetCellStringValue, out targetCellDoubleValue))
                    {
                        unacceptableVariableFlag = true;
                    }

                    // Otherwise we know the variable is a valid format for initialization.
                    else
                    {
                            tree.SetVariable(uninitVariable, targetCellDoubleValue);
                            newCellReferences = newCellReferences + "*" + column + "," + row;
                    }

                        // Since we know that the cell we just edited now references the cell GetCell(column, row), we add that reference in our reference array
                        // so that in the future if any of the cells this one references change, this one will be pinged to update its changes too.
                }
            }

            // Update all of the cell references, removing or adding as needed, then ping them to make update the dataGridView.
            // Probably should be called 'cellsThatINeedToTellIReference'
            cellReferenceString = newCellReferences;
            return unacceptableVariableFlag;
        }

        /// <summary>
        /// Listens for the 'Text' event.
        /// </summary>
        /// <param name="sender">The Cell whose value has been changed.</param>
        /// <param name="e">A string "Text" indicating the event discovered.</param>
        private void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Edit 4/9. Added handling for color of a cell.
            SpreadsheetCell caller = sender as SpreadsheetCell;
            callerRow = caller.RowIndex;
            callerColumn = caller.ColumnIndex;
            if (e.PropertyName == "Color")
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Color"));
                return;
            }
            else if (e.PropertyName == "Text")
            {
                // If we have an empty string, don't bother continuing.
                if (caller.Text == string.Empty)
                {
                    caller.Value = caller.Text;
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                    return;
                }

                if (caller.Text == null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                    return;
                }

                // If the text is just a string (not a cell ref), leave it as-is and set the value accordingly.
                if (caller.Text[0] != '=')
                {
                    caller.Value = caller.Text;
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                    return;
                }
                else
                {
                    // Make a local copy for our use.
                    string expressionText = caller.Text;

                    // Remove the '='.
                    expressionText = expressionText.Replace('=', ' ');
                    expressionText = expressionText.Replace(" ", string.Empty);

                    // If it is just an empty string, don't do all the useless work.
                    if (expressionText == string.Empty)
                    {
                        caller.Value = "ERROR";
                        PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                        return;
                    }

                    // Initialize an expression tree.
                    ExpressionTree cellExpression = new ExpressionTree(expressionText);

                    // List of cells I need to tell I reference.
                    string cellExpressionString = string.Empty;

                    // Initialize the variables of that expression tree.
                    if (this.InitializeExpressionVariables(cellExpression, caller, out cellExpressionString))
                    {
                        // If our IF is true, one of our variables didn't work. We want the Value of the cell to be an ERROR, rather than a number.
                        PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                        caller.Value = "ERROR";
                        return;
                    }

                    // If our references would result in recursion, don't even evaluate, just leave after changing the expression value to "RECURSIVE".
                    // Aka. "I reference you, do you reference me?"
                    if (SearchForCellRecursion(cellExpressionString, caller.RowIndex, caller.ColumnIndex))
                    {
                        caller.Value = "RECURSIVE";
                        PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                        return;
                    }

                    // We have all of our variables initialized.
                    else
                    {
                        // Try to evaluate the expression.
                        try
                        {
                            // If we succeed, bind the caller.Value to the result of that expression (in double form)
                            double expressionResult = cellExpression.Evaluate();
                            caller.Value = expressionResult.ToString();
                            UpdateCells(caller.RowIndex, caller.ColumnIndex, cellReferencedBy[caller.ColumnIndex, caller.RowIndex], cellExpressionString);
                        }

                        // If for some reason we can't evaluate the expression.
                        catch (ArgumentNullException)
                        {
                            // If through all of that we can't parse the expression, it isn't valid. Just treat it like a string.
                            caller.Value = caller.Text;
                            PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                            return;
                        }
                    }

                    // Update the references to which cell was changed, and then notify the form.
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));

                    // Tell everyone I updated and that they need to change.
                    UpdateByCellRecursion(cellReferencedBy[caller.RowIndex, caller.ColumnIndex]);
                }
            }
        }

        // The following functions are controls for the cellReferencedBy array, an array of strings, where one string maps directly to one cell.
        // The strings may occupy any of the following states.
        // string.Empty (The string has nothing in it) - This cell is not referenced by anybody, and doesn't need to update anyone.
        // x*y,[n] ... Where, x,y are column and row values for references. May contain [n] references. Eg. 1*12,2*33,3*1
        // Cell Position.    // A1       B1      C1
        // Cell TextValu.    // 123     =A1     =B1
        // Cell RefrList.    // (B1)    (C1)     ()

        /*
        /// <summary>
        /// Adds a notation to the cellReferencedBy array, an array in charge of tracking which cells reference which other cells.
        /// Format of use is cellReferencedBy[column, row] is a cell that is referenced by the cells contained in its strings.
        /// addCellReference is also in charge of ensuring that there is no circular references.
        /// </summary>
        /// <param name="userRow">The row of the cell being edited. A1, B1, C1.</param>
        /// <param name="userColumn">The column of the cell being edited. A1, B1, C1.</param>
        /// <param name="refRow">The row of the cell that this cell references (IE the cell we need to subscribe to). (A1), (B1), (). </param>
        /// <param name="refColumn">The column of the cell that this cell references (IE the cell we need to subscribe to). (A1), (B1), ().</param>
        /// <returns>True if the addition would not result in recursion. False if it would.</returns>
        private bool CheckCellRecursion(int userRow, int userColumn, int refRow, int refColumn)
        {
            // Remember since this container echos the dataGridView that columns and rows are swapped.
            // Get a list of strings representing all of the cells that reference the 'ref' cell.
            string cellReferenceStrings = cellReferencedBy[refColumn, refRow];
            string userCellString = userColumn.ToString() + "*" + userRow.ToString();

            // If we are trying to reference ourself, that is 1st order recursion, and not allowed. Return false, there is recursion if we do this reference, so it is not allowable.
            if (userRow == refRow && userColumn == refColumn)
            {
                return false;
            }

            // If attemptCellReferenceRecursion does NOT find an instance of userCellString, that means that of all that reference this cell, and all cells that ref them etc. none of them are this
            // cell, which means it isn't recursion if we were to add this to that list.
            if (SearchForCellRecursion(cellReferenceStrings, userCellString))
            {
                // If there is recursion return false, that we can not add this reference without resulting in recursion.
                return false;
            }

            return true;
        }
        */

        /// <summary>
        /// A recursive check to determine if references, (or refs of their refs, etc.) contain the specified string cellToCheckFor, which would indicate recursion.
        /// </summary>
        /// <param name="cellReferences">A string representing a list of references to search through.</param>
        /// <param name="recursionTargetRowIndex">A row index of a cell to search for recursion on in the given list of references.</param>
        /// <param name="recursionTargetColumnIndex">A column index of a cell to search for recursion on in the given list of references.</param>
        /// <returns>True if recursion is found, otherwise false.</returns>
        private bool SearchForCellRecursion(string cellReferences, int recursionTargetRowIndex, int recursionTargetColumnIndex)
        {
            // I had such an unbelievably hellish time figuring this out I had to draw each cell as a stick figure talking to other stick figures.
            // I then had to WRITE THE CONVERSATIONS IN and program from there.
            // Hi im A1, located at rowIndex and columnIndex. I want to ref A3
            // This is everyone I want to reference.
            string[] cellsIWantToReference = cellReferences.Split('*');

            // I am reffed by A2
            // This is everybody that references me.
            string[] referencesRecursionTarget = cellReferencedBy[recursionTargetColumnIndex, recursionTargetRowIndex].Split('*');

            // If nobody references me I can't be recursive when I change my value, but my changes might affect other people.
            if (referencesRecursionTarget.Length == 1)
            {
                return false;
            }

            // But someone does ref me, its A2
            // If I do want to reference one or more people, I need to check with everyone who references me to see if they, or the people who reference them, are referenced by me.
            bool success = false;

            // For every person that I am reffed by. (just A2)
            foreach (string cellRefString in referencesRecursionTarget)
            {
                // If it is a real cell.
                if (cellRefString != string.Empty)
                {
                    // Here is how I contact the person I am referenced by to see if they are referenced by any of the people I want to reference.
                    int[] cellCoordinates = new int[2];
                    string[] coordinateSplit = cellRefString.Split(',');
                    cellCoordinates[0] = int.Parse(coordinateSplit[0]);
                    cellCoordinates[1] = int.Parse(coordinateSplit[1]);

                    // Here is how I contact the person I am referenced by to see if they are referenced by any of the people I want to reference.
                    // The people I want to reference // Who I am asking.
                    if (Find(cellReferences, cellCoordinates[1], cellCoordinates[0]))
                    {
                        success = true;
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Helper function for SearchForCellRecursion.
        /// </summary>
        /// <param name="cellReference">The list of cell references to search through.</param>
        /// <param name="recursionTargetRowIndex">Target cell row index. If found along with ColumnIndex, indicates a recursive loop.</param>
        /// <param name="recursionTargetColumnIndex">Target cell column index. If found along with RowIndex, indicates a recursive loop.</param>
        /// <returns>False if no recursion found, True if found.</returns>
        private bool Find(string cellReference, int recursionTargetRowIndex, int recursionTargetColumnIndex)
        {
            // Hi this is A1 asking if you are referenced by any of these people.
            // Hi this is A2. I reffed by you, but you want to ref me. Not cool.
            if (cellReference.Contains("*" + recursionTargetColumnIndex + "," + recursionTargetRowIndex))
            {
                return true;
            }

            // Hi I am reffed by A2 this is everyone that references me.
            string[] referencesRecursionTarget = cellReferencedBy[recursionTargetColumnIndex, recursionTargetRowIndex].Split('*');

            bool success = false;

            // For every person that I am reffed by. (just A3)
            foreach (string cellRefString in referencesRecursionTarget)
            {
                // If it is a real cell.
                if (cellRefString != string.Empty)
                {
                    // Here is how I contact the person I am referenced by to see if they are referenced by any of the people I want to reference.
                    int[] cellCoordinates = new int[2];
                    string[] coordinateSplit = cellRefString.Split(',');
                    cellCoordinates[0] = int.Parse(coordinateSplit[0]);
                    cellCoordinates[1] = int.Parse(coordinateSplit[1]);

                    // Here is how I contact the person I am referenced by to see if they are referenced by any of the people I want to reference.
                    // The people I want to reference // Who I am asking.
                    if (Find(cellReference, cellCoordinates[1], cellCoordinates[0]))
                    {
                        success = true;
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Given a string provided by cellReferencedBy[parentCellColumn, parentCellRow], update the cells it is referenced by (and the cells those are reffed by, etc.) that have changed.
        /// To update VALUES change the value of the cell you wish, then call UpdateByCellRecursion.
        /// </summary>
        /// <param name="parentCellRow">The row of the cell that has been updated.</param>
        /// <param name="parentCellColumn">The column of the cell that has been updated.</param>
        /// <param name="oldCellRefs">The old string of cell references.</param>
        /// <param name="newCellRefs">The new string of cell references.</param>
        private void UpdateCells(int parentCellRow, int parentCellColumn, string oldCellRefs, string newCellRefs)
        {
            // Determine which cells will need their references removed.
            // Hi. I am A2. I want to be referenced by the cells in in newCellRefList, not oldCellRefList.
            List<string> oldCellRefList = oldCellRefs.Split('*').ToList<string>();

            // Now I reference you guys.
            List<string> newCellRefList = newCellRefs.Split('*').ToList<string>();

            foreach (string refString in oldCellRefList)
            {
                // If the new list doesn't contain a refString in the old list.
                // If I don't reference you in the new list, I better tell you that so you don't ping me.
                if (!newCellRefList.Contains(refString))
                {
                    // Get the coords of the referenceList referenced by the refString.
                    string[] refStringCoords = refString.Split(',');
                    int[] cellCoordinates = new int[2];
                    cellCoordinates[0] = int.Parse(refStringCoords[0]);
                    cellCoordinates[1] = int.Parse(refStringCoords[1]);

                    // Remove the reference.
                    cellReferencedBy[cellCoordinates[0], cellCoordinates[1]].Replace("*" + refString, string.Empty);
                }
            }

            // At this point every old reference not in our new list has been removed. newCellRefList contains only references that need to be created or have already been created.
            // I told all the old people I don't ref them, now tell the new people that I do.
            foreach (string refString in newCellRefList)
            {
                if (refString != string.Empty)
                {
                    // Get the coords of the referenceList referenced by the refString.
                    string[] refStringCoords = refString.Split(',');
                    int[] cellCoordinates = new int[2];
                    cellCoordinates[0] = int.Parse(refStringCoords[0]);
                    cellCoordinates[1] = int.Parse(refStringCoords[1]);

                    // If the string doesn't already contain the reference...
                    bool s = cellReferencedBy[parentCellColumn, parentCellRow].Contains(refString);
                    string t = cellReferencedBy[cellCoordinates[0], cellCoordinates[1]];
                    if (!s)
                    {
                        // Add it.
                        cellReferencedBy[cellCoordinates[0], cellCoordinates[1]] += ("*" + parentCellColumn + "," + parentCellRow);
                    }

                    // Ugly cleanup code because I keep duping cellReferences for some reason that I can't figure out.
                    string[] cleanup = cellReferencedBy[cellCoordinates[0], cellCoordinates[1]].Split('*');
                    List<string> cleanedup = new List<string> { };
                    cleanedup = cleanup.Distinct().ToList();
                    string output = "*" + string.Join("*", cleanedup);
                    cellReferencedBy[cellCoordinates[0], cellCoordinates[1]] = output;

                    // I have now told all the people who I reference that I reference them. Now we wait to see if I need to update.
                }
            }
        }

        /// <summary>
        /// A recursive check to determine if references, (or refs of their refs, etc.) contain the specified string cellToCheckFor, which would indicate recursion.
        /// </summary>
        /// <param name="cellReferences">A string representing a list of references to search recurse through.</param>
        private void UpdateByCellRecursion(string cellReferences)
        {
            // Split em up, its time to recurse.
            // Tell everyone who references me I changed.
            string[] cellReferencesSplit = cellReferences.Split('*');
            foreach (string cellRefString in cellReferencesSplit)
            {
                // Since I was lazy, the true format for cellReference strings is '*x,y*x,y,'. Which means splitting on * makes an empty string.
                if (cellRefString != string.Empty)
                {
                    // Get the coordinates for the reference strings we need to check for the cells listed in cellReferencesSplit.
                    int[] cellCoordinates = new int[2];
                    string[] coordinateSplit = cellRefString.Split(',');
                    cellCoordinates[0] = int.Parse(coordinateSplit[0]);
                    cellCoordinates[1] = int.Parse(coordinateSplit[1]);

                    this.GetCell(cellCoordinates[0], cellCoordinates[1]).Text = this.GetCell(cellCoordinates[0], cellCoordinates[1]).Text;

                    // Now tell them to tell everyone that are reffed by that they changed.
                    UpdateByCellRecursion(cellReferencedBy[cellCoordinates[0], cellCoordinates[1]]);
                }
            }
        }
    }
}
