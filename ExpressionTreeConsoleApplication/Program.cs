/* Matthew R. Jones SID: 11566314
 * Homework 5+ (Spreadsheet Project)
 * Program.cs : Wrapper for the console application of our ExpressionTree evaluation.
 */
namespace ExpressionEvaluationConsoleApplication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CptS321;

    /// <summary>
    /// Wrapper for console application.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// C# voodoo that does stuff and things.
        /// </summary>
        /// <param name="args">Arguments for when stuff and things are too stuffy and don't do things.</param>
        public static void Main(string[] args)
        {
            string expressionInMain = string.Empty;
            ExpressionTree myTree = new ExpressionTree(string.Empty);
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Expression : " + expressionInMain);
                PrintJunkToConsole();

                int result;
                if (int.TryParse(Console.ReadLine(), out result))
                {
                    switch (result)
                    {
                        case 1:
                            // Read in new expression.
                            Console.WriteLine("Input an expression.");
                            expressionInMain = Console.ReadLine();
                            myTree = new ExpressionTree(expressionInMain);
                            break;
                        case 2:
                            // Read in new variable name and definition.
                            Console.WriteLine("Input variable name.\n");
                            string varName = Console.ReadLine();

                            Console.WriteLine("Input variable value.\n");
                            string varValue = Console.ReadLine();
                            double numericVarValue;
                            if (double.TryParse(varValue, out numericVarValue))
                            {
                                myTree.SetVariable(varName, double.Parse(varValue));
                                break;
                            }

                            Console.WriteLine("Not a double value. Try again.\n");
                            break;
                        case 3:
                            // Evaluate.
                            Console.WriteLine(myTree.Evaluate());
                            if (expressionInMain == string.Empty)
                            {
                                Console.WriteLine("You might want to try inputting an expression first.\n");
                            }

                            break;
                        case 4:
                            // Exit.
                            exit = true;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// It prints junk to the console.
        /// </summary>
        public static void PrintJunkToConsole()
        {
            Console.WriteLine("1 : Enter new expression.\n2 : Define a variable name.\n3 : Evaluate the given expression.\n4 : Quit\n");
        }
    }
}
