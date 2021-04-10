using NUnit.Framework;
using CptS321;
namespace SpreadsheetEngineTesting
{
    public class ExpressionTreeTesting
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test, Category("Expression Tree : Bad Inputs")]
        // Test a tree that was given an empty string.
        public void TestEmptyTree()
        {
            Assert.Throws<System.ArgumentNullException>(delegate { ExpressionTree testTree1 = new ExpressionTree(""); ; });
        }

        [Test, Category("Expression Tree : Bad Inputs")]
        // Test a tree that was given only a variable, but did not define it.
        // Note that all variables should default to 0.0 if undefined as per assignment, so this is actually a (technically) valid input.
        public void TestTreeUndefinedVar()
        {
            ExpressionTree testTree1 = new ExpressionTree("A1");
            Assert.Throws<System.ArgumentNullException>(delegate { testTree1.Evaluate(); });
        }

        [Test, Category("Expression Tree : Bad Inputs")]
        // Test a tree that was given a malformed string. Not actually needed in this assignment, more of a placeholder.
        public void TestTreeBadInputs()
        {
            Assert.Pass();
        }

        [Test, Category("Expression Tree : Basic Inputs")]
        // Test a tree with only one value.
        public void TestTreeSingleValue()
        {
            ExpressionTree testTree1 = new ExpressionTree("42.0");
            Assert.AreEqual(42.0, testTree1.Evaluate());
        }

        [Test, Category("Expression Tree : Basic Inputs")]
        // Test a tree with only one variable.
        public void TestTreeSingleVariable()
        {
            ExpressionTree testTree1 = new ExpressionTree("A1");
            testTree1.SetVariable("A1", 42.0);
            Assert.AreEqual(42.0, testTree1.Evaluate());
        }

        [Test, Category("Expression Tree : Mathematical Operators")]
        // Test trees with simple mathematical operations.
        public void TestTreeBasicMathOperators()
        {
            ExpressionTree testTree1 = new ExpressionTree("2+2");
            Assert.AreEqual(4.0, testTree1.Evaluate());
            ExpressionTree testTree2 = new ExpressionTree("4-2");
            Assert.AreEqual(2.0, testTree2.Evaluate());
            ExpressionTree testTree3 = new ExpressionTree("2*2");
            Assert.AreEqual(4.0, testTree3.Evaluate());
            ExpressionTree testTree4 = new ExpressionTree("4/2");
            Assert.AreEqual(2.0, testTree4.Evaluate());
        }

        [Test, Category("Expression Tree : Mathematical Operators")]
        // Test trees with multiple mathematical operations.
        public void TestTreeMultiMatheOperators()
        {
            ExpressionTree testTree1 = new ExpressionTree("2+2+2+2");
            Assert.AreEqual(8.0, testTree1.Evaluate());

            ExpressionTree testTree2 = new ExpressionTree("8-4-2-1");
            Assert.AreEqual(1.0, testTree2.Evaluate());

            ExpressionTree testTree3 = new ExpressionTree("2*2*2*2");
            Assert.AreEqual(16.0, testTree3.Evaluate());

            ExpressionTree testTree4 = new ExpressionTree("64/8/4/2");
            Assert.AreEqual(1.0, testTree4.Evaluate());
        }

        [Test, Category("Expression Tree : Mathematical Operators")]
        // Test trees with multiple mathematical operations and variables.
        public void TestTreeMultiMathWithVariables()
        {
            ExpressionTree testTree1 = new ExpressionTree("A1+2+2+2");
            testTree1.SetVariable("A1", 2);
            Assert.AreEqual(8.0, testTree1.Evaluate());

            ExpressionTree testTree2 = new ExpressionTree("8-4-A1-1");
            testTree2.SetVariable("A1", 2);
            Assert.AreEqual(1.0, testTree2.Evaluate());

            ExpressionTree testTree3 = new ExpressionTree("2*A1*2*2");
            testTree3.SetVariable("A1", 2);
            Assert.AreEqual(16.0, testTree3.Evaluate());

            ExpressionTree testTree4 = new ExpressionTree("64/8/4/A1");
            testTree4.SetVariable("A1", 2);
            Assert.AreEqual(1.0, testTree4.Evaluate());
        }

        [Test, Category("Expression Tree : Precedence Operations")]
        // Test trees where results should change based on operator precedence.
        public void TestTreePrecedenceOperations()
        {
            ExpressionTree testTree1 = new ExpressionTree("2+2*2");
            Assert.AreEqual(6.0, testTree1.Evaluate());

            ExpressionTree testTree2 = new ExpressionTree("8-4/2+1");
            Assert.AreEqual(5.0, testTree2.Evaluate());
        }

        [Test, Category("Expression Tree : Parenthetical Operations")]
        // Test trees where results should change because of parenthetical syntax.
        public void TestTreeParentheticalOperations()
        {
            ExpressionTree testTree1 = new ExpressionTree("(2+2)*2");
            Assert.AreEqual(8.0, testTree1.Evaluate());

            ExpressionTree testTree2 = new ExpressionTree("(8-4)/2+1");
            Assert.AreEqual(3.0, testTree2.Evaluate());
        }
    }
}