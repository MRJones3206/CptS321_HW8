/* Matthew R. Jones SID: 11566314
 * Homework 5+ (Spreadsheet Project)
 * AbstractBaseNode.cs : Contains definition for the base type of node all OperatorNodes, VariableNodes, and ConstantNodes inherit from.
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
    internal abstract class AbstractBaseNode
    {
        private string associativity = "NONE";
        private string type = "ABSTRACT";
        private string contents = string.Empty;
        private int prescedence;
        private AbstractBaseNode left = null;
        private AbstractBaseNode right = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractBaseNode"/> class.
        /// Currently this does nothing. It doesn't need to, as it will never be called (directly). The child nodes should be in charge of all initialization.
        /// </summary>
        /// <param name="newAssociativity">The associativity of the contents of the node. NONE if constant, variable, or a unary operator. LEFT or RIGHT for binary operators with assoc.</param>
        /// <param name="newType">A type to associate with the node. May be CONSTANT, VARIABLE, or OPERATOR. Used to preserve my sanity. Ed. 3/19 now includes PARENTHESES as a shortcut for parenthetical operation containment.</param>
        /// <param name="newContents">The raw contents associated with the node.</param>
        /// /// <param name="newPrescedence">A numerical representation of precedence of an operator. Hyper-precedent operations '(, )' = 1000, addition/subtraction = 1, multiplication/division = 2.</param>
        internal AbstractBaseNode(string newAssociativity, string newType, string newContents, int newPrescedence)
        {
            associativity = newAssociativity;
            type = newType;
            contents = newContents;
            prescedence = newPrescedence;
        }

        /// <summary>
        /// Gets the associativity property.
        /// </summary>
        public string Associativity
        {
            get { return associativity; }
        }

        /// <summary>
        /// Gets the type property.
        /// </summary>
        public string Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets the contents property.
        /// </summary>
        public string Contents
        {
            get { return contents; }
        }

        /// <summary>
        /// Gets the prescedence property.
        /// </summary>
        public int Prescedence
        {
            get { return prescedence; }
        }

        /// <summary>
        /// Gets or sets the left node.
        /// </summary>
        public AbstractBaseNode Left
        {
            get { return left; }
            internal set { left = value; }
        }

        /// <summary>
        /// Gets or sets the right node.
        /// </summary>
        public AbstractBaseNode Right
        {
            get { return right; }
            internal set { right = value; }
        }

        /// <summary>
        /// Returns the value of the expression at this node position to a higher node. Implementation changes based on node type.
        /// </summary>
        /// <returns>The value of the node expression at this position.</returns>
        public virtual double Evaluate()
        {
            return 0.0;
        }
    }
}
