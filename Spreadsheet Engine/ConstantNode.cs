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
    internal class ConstantNode : AbstractBaseNode
    {
        private double value = 0.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantNode"/> class.
        /// </summary>
        /// <param name="newAssociativity">The associativity of the contents of the node. NONE if constant, variable, or a unary operator. LEFT or RIGHT for binary operators with assoc.</param>
        /// <param name="newType">A type to associate with the node. May be CONSTANT, VARIABLE, or OPERATOR. Used to preserve my sanity.</param>
        /// <param name="newContents">The raw contents associated with the node.</param>
        public ConstantNode(string newAssociativity, string newType, string newContents)
            : base(newAssociativity, newType, newContents, 0)
        {
            if (!double.TryParse(newContents, out value))
            {
                value = 0.0;
            }
        }

        /// <summary>
        /// Overrides the default implementation of Evaluate(). As the value of newContents is always parsed after construction, will always return... something.
        /// </summary>
        /// <returns>If value parsed successfully, the value of contents, otherwise 0.0.</returns>
        public override double Evaluate()
        {
            return value;
        }
    }
}
