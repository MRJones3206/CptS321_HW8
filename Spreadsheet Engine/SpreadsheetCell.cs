/* Matthew R. Jones SID: 11566314
 * Homework 4+ (Spreadsheet Project)
 * SpreadsheetCell.cs : General abstract container class for a spreadsheet cell.
 */
namespace CptS321
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// A general abstract container class for a spreadsheet cell.
    /// </summary>
    public abstract class SpreadsheetCellAbstract : INotifyPropertyChanged
    {
        /// <summary>
        /// A container for the text value of a cell.
        /// </summary>
        protected string text;

        /// <summary>
        /// A container for the true value of a cell.
        /// </summary>
        protected string value;

        private readonly int rowIndex;
        private readonly int columnIndex;
        private uint color;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadsheetCellAbstract"/> class.
        /// </summary>
        /// <param name="initRowIndex">Initial row index to assign.</param>
        /// <param name="initColumnIndex">Initial column index to assign.</param>
        public SpreadsheetCellAbstract(int initRowIndex, int initColumnIndex)
        {
            color = 0xFFFFFFFF;
            rowIndex = initRowIndex;
            columnIndex = initColumnIndex;
            text = string.Empty;
        }

        /// <summary>
        /// Event magic does a thing.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the rowIndex property.
        /// </summary>
        public int RowIndex
        {
            get { return rowIndex; }
        }

        /// <summary>
        /// Gets or sets the color property.
        /// </summary>
        public uint Color
        {
            get
            {
                return color;
            }

            set
            {
                if (color != value)
                {
                    color = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Color"));
                }
            }
        }

        /// <summary>
        /// Gets the columnIndex property.
        /// </summary>
        public int ColumnIndex
        {
            get { return columnIndex; }
        }

        /// <summary>
        /// Gets or sets the text property.
        /// </summary>
        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                // Removed. There is an interesting bug with spreadsheet updating where if a cell is selected but not changed in value it is not evaluated when the user
                // clicks off of it. This is because the PropertyChanged() chain never happens, and never updates the rest of the sequence.
                /*if (text == value)
                {
                    return;
                }*/

                // It is important to note that a SpreadSheetCell never actually performs its own value calculations. This is handled by the Spreadsheet
                // itself, as it is the most localized object that knows about all of the other cells in the Spreadsheet, which is required for evaluating
                // expressions with references to other cell
                if (text.Contains("~"))
                {
                    text = "ERROR: Can't use '~' in cells!";
                    PropertyChanged(this, new PropertyChangedEventArgs("Text"));
                }

                text = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Text"));
            }
        }

        /// <summary>
        /// Gets (or internally sets) the value property.
        /// </summary>
        public string Value
        {
            get { return value; }
            internal set { this.value = value; }
        }

        /// <summary>
        /// Informs the information layer that a property has been changed manually. Good for updating things.
        /// </summary>
        /// <param name="args">The type of argument to throw.</param>
        public void ThrowPropertyChanged(string args)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(args));
            return;
        }
    }

    /// <summary>
    /// The public(internal)-facing version of SpreadsheetCellAbstract.
    /// </summary>
    internal class SpreadsheetCell : SpreadsheetCellAbstract
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadsheetCell"/> class. This is the public facing version of the abstract SpreadsheetCellAbstract class.
        /// </summary>
        /// <param name="initRowIndex">Row index to initialize a cell with.</param>
        /// <param name="initColumnIndex">Column index to initialize a cell with.</param>
        public SpreadsheetCell(int initRowIndex, int initColumnIndex)
           : base(initRowIndex, initColumnIndex)
            {
        }
    }
}