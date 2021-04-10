/* Matthew R. Jones SID: 11566314
 * Homework 5+ (Spreadsheet Project)
 * ExpressionTree.cs : Externally visible implementation of the ExpressionTree object. Allows user to perform mathematical operations via string parsing.
 */
namespace CptS321
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Container class for an expression tree evaluation. Supports only simple multi-instance single operators at this time.
    /// </summary>
    public class ExpressionTree
    {
        private readonly string expressionContainer;
        private Dictionary<string, double> variableDictionary = new Dictionary<string, double> { };
        private AbstractBaseNode root;
        private HashSet<string> operatorSet = new HashSet<string>() { "+", "-", "*", "/", "(", ")" };
        private HashSet<string> uninitializedVariableList = new HashSet<string> { };

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTree"/> class.
        /// </summary>
        /// <param name="expression">An expression to initialize the expression tree with.</param>
        public ExpressionTree(string expression)
        {
            expressionContainer = expression;

            // Remove whitespace. Just in case there is any.
            expressionContainer = expressionContainer.Replace(" ", string.Empty);

            if (expression == string.Empty)
            {
                throw new ArgumentNullException("The provided expressionstring is empty!");
            }

            // This may involve a lot of string edits, and I want index based editing. Lets make it a stringbuilder for now.
            StringBuilder localExpression = new StringBuilder(expression);

            // Lets pluck out all of our operators and format the string a little bit.
            List<OperatorNode> operationList = new List<OperatorNode> { };
            for (int i = 0; i < expression.Length; i++)
            {
                // If we find an operator, stick it in the list as an OperatorNode and replace it with something super unique.
                // !!! Ed. 3-19 Probably should make sure we aren't allowed to use this in our cells or as a operator itself...
                if (operatorSet.Contains(expression[i].ToString()))
                {
                    operationList.Add(OperatorNodeFactory.Generate(localExpression[i].ToString()));
                    localExpression[i] = '~';
                }
            }

            // Given that we replaced all of the recognised operators with '~', everything else should be a double value or a variable.
            string[] numericStrings = localExpression.ToString().Split('~');

            // Let's get them into their lists.
            double number;

            // Create a list for values.
            List<AbstractBaseNode> valueList = new List<AbstractBaseNode> { };

            foreach (string s in numericStrings)
            {
                // If s is not empty. (Possible if we have a lot of parentheses, but in that case there shouldn't be anything between them).
                if (s != string.Empty)
                {
                    // If our numberic string succeeds in being viewable as a double...
                    if (double.TryParse(s, out number))
                    {
                        // Congradulations it's a double. Stick it in a ConstantNode and slap it on the list.
                        valueList.Add(new ConstantNode("NONE", "CONSTANT", s));
                    }

                    // Otherwise it isn't a double, so we will call it a variable. (Which we can do, because we know inputs to the expression are errorless, and we eliminated all constants and operators).
                    else
                    {
                        // If our variable dictionary doesn't already contain the variable we need, we create one and initialize it to 0.0.
                        // Ed 4/2. Removed per HW7. Undefined values must now throw an exception.
                        // Ed 4/2. Added functionality to allow expressionTree objects to provide a list of uninitialized variables using, getUninitializedVariableStrings().
                        if (!variableDictionary.ContainsKey(s))
                        {
                            uninitializedVariableList.Add(s);
                        }

                        valueList.Add(new VariableNode(ref variableDictionary, "NONE", "VARIABLE", s));
                    }
                }
            }

            // We now have a list of numbers (either constants or variables), and a list of operators.
            // Let's patch everything back together into something we can feed into a shunting yard algorithm.
            // Create a list to hold our string in the form of Operator, Constant, and Variable nodes.
            List<AbstractBaseNode> reformattedString = new List<AbstractBaseNode> { };

            // Reference: In an expression formatted as 2+(3-4)/9 we should have valueLists of { 2, 3, 4, 9 } and operationLists of { +, (, -, ), / }.
            // Since we know all of our equations are well formed, we can make some assumptions about their behavior. Namely (once we ignore parentheses), at no point may two OperatorNodes
            // or Variable/Constant nodes be directly next to eachother. This lets us reconstruct the string from our two lists, provided that we know what we added to our string previously.
            string nextRequiredType = "numeric";

            // While we still have stuff to add to our reformatted string.
            while (valueList.Count() != 0 || operationList.Count() != 0)
            {
                // As long as we have stuff in the operationList, do the following.
                // If we encounter an open parentheses and are expecting a numeric value, or a closed parentheses and are expecting an operator value, add the parentheses instead of anything else.
                // This skips that weird phase where we would try to add a parentheses before our value has been added.
                if (operationList.Count() > 0 && ((operationList[0].Type == "PARENTHESES_OPEN" && nextRequiredType == "numeric") || (operationList[0].Type == "PARENTHESES_CLOSED" && nextRequiredType == "operator")))
                {
                    reformattedString.Add(operationList[0]);
                    operationList.RemoveAt(0);
                }

                // Add either a numeric or operator value.
                else if (nextRequiredType == "numeric")
                {
                    reformattedString.Add(valueList[0]);
                    valueList.RemoveAt(0);
                    nextRequiredType = "operator";
                }
                else if (nextRequiredType == "operator")
                {
                    reformattedString.Add(operationList[0]);
                    operationList.RemoveAt(0);
                    nextRequiredType = "numeric";
                }
            }

            // At this point we should have a list of nicely shuntable objects.
            Stack<AbstractBaseNode> myShuntingStack = new Stack<AbstractBaseNode> { };
            List<AbstractBaseNode> postfixExpression = new List<AbstractBaseNode> { };

            while (reformattedString.Count() != 0)
            {
                // 1. If the incoming symbols is an operand, output it. (in this case stick it on the end of our list of symbols).
                if (reformattedString[0].Type == "VARIABLE" || reformattedString[0].Type == "CONSTANT")
                {
                    postfixExpression.Add(reformattedString[0]);
                    reformattedString.RemoveAt(0);
                }

                // 2. If the incoming symbol is a left parenthesis, push it on the stack.
                else if (reformattedString[0].Type == "PARENTHESES_OPEN")
                {
                    myShuntingStack.Push(reformattedString[0]);
                    reformattedString.RemoveAt(0);
                }

                // 3. If the incoming symbol is a right parenthesis...
                else if (reformattedString[0].Type == "PARENTHESES_CLOSED")
                {
                    // ... discard the right parenthesis ...
                    reformattedString.RemoveAt(0);

                    // ... pop and print the stack symbols until you see a left parenthesis ...
                    while (myShuntingStack.Peek().Type != "PARENTHESES_OPEN")
                    {
                        postfixExpression.Add(myShuntingStack.Pop());
                    }

                    // ... Pop the left parenthesis and discard it.
                    myShuntingStack.Pop();
                }

                // 4. If the incoming symbol is an operator and the stack is empty or contains a left parenthesis on top ...
                else if (reformattedString[0].Type == "OPERATOR" && (myShuntingStack.Count() == 0 || myShuntingStack.Peek().Type == "PARENTHESES_OPEN"))
                {
                    // ... push the incoming operator onto the stack.
                    myShuntingStack.Push(reformattedString[0]);
                    reformattedString.RemoveAt(0);
                }

                // 5. If the incoming symbol is an operator AND
                //    (has either higher precedence than the operator on the top of the stack
                //    OR has the same precedence as the operator on the top of the stack AND is right associative)... push it on the stack.
                else if (reformattedString[0].Type == "OPERATOR" &&
                    ((reformattedString[0].Prescedence > myShuntingStack.Peek().Prescedence)
                    || (reformattedString[0].Prescedence == myShuntingStack.Peek().Prescedence && reformattedString[0].Associativity == "RIGHT")))
                {
                    // ... push it on the stack.
                    myShuntingStack.Push(reformattedString[0]);
                    reformattedString.RemoveAt(0);
                }

                // 6. If the incoming symbol is an operator AND
                //    (has either lower precedence than the operator on the top of the stack
                //    OR has the same precedence as the operator on the top of the stack AND is left associative) ...
                else if (reformattedString[0].Type == "OPERATOR" &&
                    ((reformattedString[0].Prescedence < myShuntingStack.Peek().Prescedence)
                    || (reformattedString[0].Prescedence == myShuntingStack.Peek().Prescedence && reformattedString[0].Associativity == "LEFT")))
                {
                    // ... continue to pop the stack until this is not true ...
                    while (myShuntingStack.Count() != 0 && ((reformattedString[0].Prescedence < myShuntingStack.Peek().Prescedence)
                          || (reformattedString[0].Prescedence == myShuntingStack.Peek().Prescedence && reformattedString[0].Associativity == "LEFT")))
                    {
                        postfixExpression.Add(myShuntingStack.Pop());
                    }

                    // ... Then, push the incoming operator.
                    myShuntingStack.Push(reformattedString[0]);
                    reformattedString.RemoveAt(0);
                }
            }

            // 7. At the end of the expression, pop and print all operators on the stack. (No parentheses should remain).
            while (myShuntingStack.Count() != 0)
            {
                postfixExpression.Add(myShuntingStack.Pop());
            }

            // Make a new stack to hold trees. Wasteful, but nice looking code usually is.
            Stack<AbstractBaseNode> myTreeStack = new Stack<AbstractBaseNode> { };

            /*
           • If the symbol is an operator then pop the last two trees from the
           stack and create a new tree with the operator as the root, last
           element of the stack as right subtree, and the one before last element
           of the stack as left subtree.Push this newly created tree to the stack.The result is the only element of
           the stack./ While we have stuff to stick in a tree...*/

            // Read the postfix expression one symbol at the time ... Stop when no more symbols are left.
            while (postfixExpression.Count() != 0)
            {
                // 1. If the symbol is an operand then create a tree with it and push it to the stack ...
                if (postfixExpression[0].Type == "VARIABLE" || postfixExpression[0].Type == "CONSTANT")
                {
                    myTreeStack.Push(postfixExpression[0]);
                    postfixExpression.RemoveAt(0);
                }

                // 2. If the symbol is an operator ...
                else if (postfixExpression[0].Type == "OPERATOR")
                {
                    // ... then pop the last two trees from the stack ..
                    AbstractBaseNode oneBeforeLastElement = myTreeStack.Pop();
                    AbstractBaseNode lastElement = myTreeStack.Pop();

                    // ... create a new tree with the operator as the root, last element of the stack as right subtree ...
                    postfixExpression[0].Right = oneBeforeLastElement;

                    // ... and the one before last element of the stack as left subtree.
                    postfixExpression[0].Left = lastElement;

                    // ... Push this newly created tree to the stack.
                    myTreeStack.Push(postfixExpression[0]);
                    postfixExpression.RemoveAt(0);
                }
            }

            // The result is the only element of the stack.
            root = myTreeStack.Pop();
        }

        /// <summary>
        /// Gets the expression the tree currently contains.
        /// </summary>
        /// <returns>The expression contained in the class.</returns>
        public string Expression
        {
            get { return expressionContainer; }
        }

        /// <summary>
        /// Set a variable value in the internal dictionary of an ExpressionTree.
        /// </summary>
        /// <param name="variableName">A variable name to pass into the ExpressionTree object.</param>
        /// <param name="variableValue">The value to maintain for the ExpressionTree object.</param>
        public void SetVariable(string variableName, double variableValue)
        {
            // If we don't have the key listed, add it to the dict and assign its value.
            if (!variableDictionary.ContainsKey(variableName))
            {
                variableDictionary.Add(variableName, variableValue);

                // Ed. 4/2. Allow variable to be removed from uninitialized variable list once it has been defined.
                uninitializedVariableList.Remove(variableName);
                return;
            }

            // If we get here we know the key exists, so it was already added by the ExpressionTree(string) constructor. Assign its value.
            variableDictionary[variableName] = variableValue;
        }

        /// <summary>
        /// Gets the uninitializedVariableList, a list of variables that have not yet been initialized. Name is a bit of a misnomer, its actual container is a hashset.
        /// </summary>
        /// <returns>A hashset containing all uninitialized variables for the expression.</returns>
        public HashSet<string> GetUninitializedVariableList()
        {
            return uninitializedVariableList;
        }

        /// <summary>
        /// Evaluate the current ExpressionTree and return a double value representing its value, or 0.0 if an error ocurrs.
        /// </summary>
        /// <returns>A double representing the value of the expression, or 0.0 for error.</returns>
        public double Evaluate()
        {
            try
            {
                double container = root.Evaluate();
                return container;
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException("One or more variables in the given expression is not defined.");
            }
        }
    }
}
