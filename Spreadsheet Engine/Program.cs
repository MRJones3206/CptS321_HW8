namespace CptS321
{
    // This entire thing is strictly for testing within the .dll space.
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Testing program to call main on the dll.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Magic juju no worko when project no seto to console application. Seriously, this thing was just so I could watch the debug, you want the ExpressionTreeConsoleApplication program.
        /// </summary>
        /// <param name="args">More magic juju.</param>
        public static void Main(string[] args)
        {
            ExpressionTree testTree1 = new ExpressionTree("A1");
            Console.WriteLine(testTree1.Evaluate());
            Console.WriteLine("Ping");
        }
    }
}
