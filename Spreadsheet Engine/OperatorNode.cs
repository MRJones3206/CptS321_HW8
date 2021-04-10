/* Matthew R. Jones SID: 11566314
 * Homework 5+ (Spreadsheet Project)
 * OperatorNode.cs : Contains definition for a OperatorNode, a node with an operator and operation associated with it.
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
    internal class OperatorNode : AbstractBaseNode
    {
        private Func<double, double, double> localEvaluate;

        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorNode"/> class.
        /// </summary>
        /// <param name="inputEvaluateFunction">The function that Evaluate() should perform when called.</param>
        /// <param name="newAssociativity">The associativity of the contents of the node. NONE if constant, variable, or a unary operator. LEFT or RIGHT for binary operators with assoc.</param>
        /// <param name="newType">A type to associate with the node. May be CONSTANT, VARIABLE, or OPERATOR. Used to preserve my sanity.</param>
        /// <param name="newContents">The raw contents associated with the node.</param>
        /// <param name="newPrecedence">A numerical representation of precedence of an operator. Hyper-precedent operations '(, )' = 1000, addition/subtraction = 1, multiplication/division = 2.</param>
        public OperatorNode(Func<double, double, double> inputEvaluateFunction, string newAssociativity, string newType, string newContents, int newPrecedence)
            : base(newAssociativity, newType, newContents, newPrecedence)
        {
            localEvaluate = inputEvaluateFunction;
        }

        /// <summary>
        /// Overrides the default implementation of Evaluate(). As the value of newContents is always parsed after construction, will always return... something.
        /// </summary>
        /// <returns>If value parsed successfully, the value of contents, otherwise 0.0.</returns>
        public override double Evaluate()
        {
            return localEvaluate(Left.Evaluate(), Right.Evaluate());
        }
    }
}
