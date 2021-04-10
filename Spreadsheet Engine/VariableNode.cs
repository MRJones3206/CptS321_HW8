/* Matthew R. Jones SID: 11566314
 * Homework 5+ (Spreadsheet Project)
 * ConstantNode.cs : Contains definition for a ConstantNode, a node with a constant, concrete value that will never change once created.
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
    internal class VariableNode : AbstractBaseNode
    {
        private Dictionary<string, double> refDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNode"/> class.
        /// </summary>
        /// <param name="referenceDictionary">A reference to the dictionary used by ExpressionTree to evaluate values. Since we might want to evaluate the expression multiple times, it makes
        /// sense to allow the tree to respond to changes in the Variable dictionary dynamically, rather than having to rebuild each time.</param>
        /// <param name="newAssociativity">The associativity of the contents of the node. NONE if constant, variable, or a unary operator. LEFT or RIGHT for binary operators with assoc.</param>
        /// <param name="newType">A type to associate with the node. May be CONSTANT, VARIABLE, or OPERATOR. Used to preserve my sanity.</param>
        /// <param name="newContents">The raw contents associated with the node.</param>
        public VariableNode(ref Dictionary<string, double> referenceDictionary, string newAssociativity, string newType, string newContents)
            : base(newAssociativity, newType, newContents, 0)
        {
            refDictionary = referenceDictionary;
        }

        /// <summary>
        /// Overrides the default implementation of Evaluate(). As the value is always parsed each time it is asked to Evaluate() itself, we can't hardcode its evaluation into a constructor.
        /// </summary>
        /// <returns>If value parsed successfully, the value of contents, otherwise 0.0.</returns>
        public override double Evaluate()
        {
            // If our reference dict contains the key specified by Contents, return its value, otherwise return 0.0.
            if (refDictionary.ContainsKey(Contents))
            {
                return refDictionary[Contents];
            }

            throw new ArgumentNullException("One or more variables in the given expression is not defined.");
        }
    }
}