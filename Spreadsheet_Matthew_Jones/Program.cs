/* Matthew R. Jones SID: 11566314
 * Homework 4+ (Spreadsheet Project)
 * Program.cs : Who even knows.
 */
namespace Spreadsheet_Matthew_Jones
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    /// <summary>
    /// Winforms junk.
    /// </summary>
#pragma warning disable SA1400 // Access modifier should be declared. Unless you are winforms.
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
#pragma warning restore SA1400 // Access modifier should be declared