/* Matthew R. Jones SID: 11566314
 * Homework 5+ (Spreadsheet Project)
 * OperatorNodeFactory.cs : A little widget for creating operator nodes. Also is in charge of managing our funct definitions for our operator interpretation because... convenience.
 */
namespace CptS321
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The base definition that all other node types inherit from.
    /// </summary>
    internal class OperatorNodeFactory
    {
        // Add definitions for operations here.
        private static Dictionary<string, Func<double, double, double>> functionDictionary = new Dictionary<string, Func<double, double, double>>
            {
                { "+", (x, y) => x + y },
                { "-", (x, y) => x - y },
                { "*", (x, y) => x * y },
                { "/", (x, y) => x / y },
                { "!!!", (x, y) => 0.0 },
                { "(", (x, y) => 0.0 },
                { ")", (x, y) => 0.0 },
            };

        /// <summary>
        /// Generates and returns a new instance of the OperatorNode class with a function specified by operatorExpression.
        /// </summary>
        /// <param name="operatorExpression">The operator to include the Func definition for.</param>
        /// <returns>An OperatorNode with the baked in func definition specified by operatorExpression, or the error func if operatorExpression is not found, which returns 0.0 on Execute().</returns>
        public static OperatorNode Generate(string operatorExpression)
        {
            OperatorNode newNode;
            if (functionDictionary.ContainsKey(operatorExpression))
            {
                // Note that associativity is not really explicitly handled, and is sort of baked into the OperatorNode. This might change in later projects once it starts mattering.
                // Edit 3/19: Associativity and precedence is now handled in a shallow (and extremely ugly) way.
                // Normally this would be refactored (because god is it ugly), but I want to wait to do all refactoring on this section when I implement user-defined operations and LINQ syntax
                // so anything pretty I do here right now is honestly just wasted effort.
                if (operatorExpression == "(")
                {
                    newNode = new OperatorNode(functionDictionary[operatorExpression], "NONE", "PARENTHESES_OPEN", operatorExpression, 1000);
                    return newNode;
                }

                if (operatorExpression == ")")
                {
                    newNode = new OperatorNode(functionDictionary[operatorExpression], "NONE", "PARENTHESES_CLOSED", operatorExpression, 1000);
                    return newNode;
                }

                if (operatorExpression == "+")
                {
                    newNode = new OperatorNode(functionDictionary[operatorExpression], "RIGHT", "OPERATOR", operatorExpression, 1);
                    return newNode;
                }

                if (operatorExpression == "-")
                {
                    newNode = new OperatorNode(functionDictionary[operatorExpression], "LEFT", "OPERATOR", operatorExpression, 1);
                    return newNode;
                }

                if (operatorExpression == "*")
                {
                    newNode = new OperatorNode(functionDictionary[operatorExpression], "RIGHT", "OPERATOR", operatorExpression, 2);
                    return newNode;
                }

                if (operatorExpression == "/")
                {
                    newNode = new OperatorNode(functionDictionary[operatorExpression], "LEFT", "OPERATOR", operatorExpression, 2);
                    return newNode;
                }
            }

            newNode = new OperatorNode(functionDictionary["!!!"], "NONE", "OPERATOR", operatorExpression, 0);
            return newNode;
        }
    }

        /*public OperatorNode(Func<double, double, double> inputEvaluateFunction, string newAssociativity = "NONE", string newType = "OPERATOR", string newContents = "")
            : base(newAssociativity = "NONE", newType = "OPERATOR", newContents)
        {
            localEvaluate = inputEvaluateFunction;
        }*/
}
