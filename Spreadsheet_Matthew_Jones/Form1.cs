/* Matthew R. Jones SID: 11566314
 * Homework 4+ (Spreadsheet Project)
 * Form1.cs : Controller for form behavior and init declarations for form components.
 */
#pragma warning disable SA1101 // Prefix local calls with this
namespace Spreadsheet_Matthew_Jones
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using CptS321;
    using Spreadsheet_Engine;

    /// <summary>
    /// Winforms Form declaration.
    /// </summary>
    public partial class Form1 : Form
    {
        private Spreadsheet spreadsheet = new Spreadsheet(26, 50);

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// Winforms form initializer.
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            // Clean up my mess and init the grid with some letters and numbers on its rows.
            InitDataGridView();
            spreadsheet.PropertyChanged += this.ValueChange;
        }

        /// <summary>
        /// Given an object sender, which will always be a Spreadsheet object, and event arguments e, update a specific cell value in the UI
        /// specified by the sender object with the value found in the same location in the backend Spreadsheet object.
        /// </summary>
        /// <param name="sender">The object calling the PropertyChanged event.</param>
        /// <param name="e">A string representing the property changing.</param>
        private void ValueChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Color")
            {
                Spreadsheet caller = sender as Spreadsheet;
                SpreadsheetCellAbstract callingCell = caller.GetCell(caller.CallerColumn, caller.CallerRow);
                Color newColor = Color.FromArgb((int)callingCell.Color);
                this.dataGridView1[caller.CallerColumn, caller.CallerRow].Style.BackColor = newColor;
            }
            else if (e.PropertyName == "Value")
            {
                Spreadsheet caller = sender as Spreadsheet;

                // This whole mess is because we don't know which cell exactly we are supposed to be changing the UI layer text for, so we gotta ask the Spreadsheet.
                SpreadsheetCellAbstract callingCell = caller.GetCell(caller.CallerColumn, caller.CallerRow);
                this.dataGridView1[caller.CallerColumn, caller.CallerRow].Value = callingCell.Value;
            }
        }

        /// <summary>
        /// Given a cell object, once that object has begun to be edited, notify the spreadsheet.
        /// </summary>
        /// <param name="sender">The object calling the property changed event.</param>
        /// <param name="targetCell">A container containing the row and column values of the cell being edited.</param>
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs targetCell)
        {
            // Once we have begun editing, the DGV should display the text contents of the cell, not what they evaluate to.
            this.dataGridView1[targetCell.ColumnIndex, targetCell.RowIndex].Value = spreadsheet.GetCell(targetCell.ColumnIndex, targetCell.RowIndex).Text;
    }

        /// <summary>
        /// Given a cell object, once that object has finished being edited, notify the spreadsheet.
        /// </summary>
        /// <param name="sender">The object calling the property changed event.</param>
        /// <param name="targetCell">A container containing the row and column values of the cell being edited.</param>
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs targetCell)
        {
            // Once we have finished editing, the DGV should display the VALUE of the cell contents, not the text.
            // To do this, push the value of the DGV cell into the spreadsheets cell.Text setter. This will make the spreadsheet do evaluation (if needed), and update its value.
            // The spreadsheet will then notify the DGV of a value change (if necessary) for that and any other affected cell, and will update them through ValueChange(...).
            //
            // Ed. 4/9. Now also in charge of creating an undo-redo object and notifying the spreadsheet, seeing as this is the ONLY OBJECT that can actually see
            // both the prior and final states of any of the cells simultaneously.
            // .
            // This holds what used to be in the string.
            string oldContent = spreadsheet.GetCell(targetCell.ColumnIndex, targetCell.RowIndex).Text;

            // This holds what will be in the string once the updates are pushed through.
            string newContent = (string)this.dataGridView1[targetCell.ColumnIndex, targetCell.RowIndex].Value;

            // Push the update to the spreadsheet backend.
            spreadsheet.GetCell(targetCell.ColumnIndex, targetCell.RowIndex).Text = (string)this.dataGridView1[targetCell.ColumnIndex, targetCell.RowIndex].Value;

            // Add a record in undoredo.
            if (oldContent != newContent)
            {
                spreadsheet.AddUndoRedoAction(new UndoRedoTextChange("cell text edit.", oldContent, newContent, targetCell.ColumnIndex, targetCell.RowIndex));
            }

            // Update the DGV so it shows values, not text.
            this.dataGridView1[targetCell.ColumnIndex, targetCell.RowIndex].Value = spreadsheet.GetCell(targetCell.ColumnIndex, targetCell.RowIndex).Value;
        }

        /// <summary>
        /// On-click arguments for cell contents. Not sure what this is tbh.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The sender parameters.</param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // No idea what this is for.
        }

        /// <summary>
        /// Private helper class for initializing the dataGridView1 datagrid to something we actually want to use.
        /// </summary>
        private void InitDataGridView()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            string columns = "ABCDEFGHIJLMNOPQRSTUVWXYZ~";
            foreach (char c in columns)
            {
                dataGridView1.Columns.Add(c.ToString(), c.ToString());
            }

            for (int rows = 0; rows < 50; rows++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[rows].HeaderCell.Value = (rows + 1).ToString();
            }

            dataGridView1.CellBeginEdit += this.dataGridView1_CellBeginEdit;
            dataGridView1.CellEndEdit += this.dataGridView1_CellEndEdit;
        }

        /// <summary>
        /// Do some cool stuff to the grid (indirectly).
        /// </summary>
        /// <param name="sender">Winforms stuff.</param>
        /// <param name="e">Winforms stuff mk2.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            spreadsheet.GetCell(0, 0).Text = "Hello World!";
            spreadsheet.GetCell(25, 49).Text = "End of the line, pardner.";
            Random random = new Random();
            for (int i = 0; i < 500; i++)
            {
                // Why hello there.
                spreadsheet.GetCell(random.Next(0, 25), random.Next(0, 49)).Text = "Hello World!";
            }

            for (int i = 0; i < 25; i++)
            {
                // Show off a bit.
                spreadsheet.GetCell(1, i).Text = "I am B" + (i + 1);
                spreadsheet.GetCell(2, i).Text = "=A1";
                spreadsheet.GetCell(3, i).Text = "C" + (i + 1) + " is a copycat!";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Note that some of this is egregiously stolen from the MSDN.
            ColorDialog myDialog = new ColorDialog();

            // Keeps the user from selecting a custom color.
            myDialog.AllowFullOpen = true;

            // Allows the user to get help. (The default is false.)
            myDialog.ShowHelp = false;
            DataGridViewSelectedCellCollection selectedCells = this.dataGridView1.SelectedCells;

            // Update the text box color if the user clicks OK.
            if (myDialog.ShowDialog() == DialogResult.OK)
            {
                // Since for some reason my UndoRedo section doesn't think winforms objects exist, make commandstrings here.
                List<string> commandList = new List<string> { };
                foreach (DataGridViewCell cell in selectedCells)
                {
                    string command = "COLOR~" + spreadsheet.GetCell(cell.ColumnIndex, cell.RowIndex).Color + "~" + (uint)myDialog.Color.ToArgb() + "~" + cell.ColumnIndex + "~" + cell.RowIndex;
                    commandList.Add(command);
                    spreadsheet.GetCell(cell.ColumnIndex, cell.RowIndex).Color = (uint)myDialog.Color.ToArgb();
                }

                spreadsheet.AddUndoRedoAction(new UndoRedoCellColorChange("cell color change.", commandList));
            }
        }

        /// <summary>
        /// When the menu item 'Edit' is clicked, what do you want to happen.
        /// </summary>
        /// <param name="sender">The menuItem object.</param>
        /// <param name="e">Arguments.</param>
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;
            string undoDescription = spreadsheet.UndoDescription();
            string redoDescription = spreadsheet.RedoDescription();
            if (undoDescription != string.Empty)
            {
                undoToolStripMenuItem.Enabled = true;
                undoToolStripMenuItem.Text = "Undo " + undoDescription;
            }

            if (redoDescription != string.Empty)
            {
                redoToolStripMenuItem.Enabled = true;
                redoToolStripMenuItem.Text = "Redo " + redoDescription;
            }
        }

        /// <summary>
        /// When the menu item 'Edit' is clicked, what do you want to happen.
        /// </summary>
        /// <param name="sender">The menuItem object.</param>
        /// <param name="e">Arguments.</param>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            spreadsheet.DoUndo();
        }

        /// <summary>
        /// When the menu item 'Edit' is clicked, what do you want to happen.
        /// </summary>
        /// <param name="sender">The menuItem object.</param>
        /// <param name="e">Arguments.</param>
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            spreadsheet.DoRedo();
        }

        /// <summary>
        /// Don't use this, it was created on accident.
        /// </summary>
        /// <param name="sender">The menuItem object.</param>
        /// <param name="e">Arguments.</param>
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            return;
        }
    }
}
#pragma warning restore SA1101 // Prefix local calls with this