using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Collections.Generic;
using toUpperExtension;

/// <summary>
/// This is used to test the methods and exceptions in Formula class
/// </summary>
namespace FormulaTester
{
    /// <summary>
    /// Tester class that contains several methods to test the methods and exceptions in Formula class 
    /// </summary>
    [TestClass]
    public class Tester
    {
        /// <summary>
        /// Constructor tester
        /// </summary>
        [TestMethod]
        public void constructorTester2()
        {
            Formula f1 = new Formula("x33 + y3");
            Assert.AreEqual("x33+y3", f1.ToString());
        }

        [TestMethod]
        public void constructorTest3()
        {
            Formula f1 = new Formula("2.0 * (x2)");
            Assert.AreEqual("2.0*(x2)", f1.ToString());
        }

        /// <summary>
        /// Test4-8: Test Evaluate method
        /// </summary>
        [TestMethod]
        public void test4()
        {
            Formula f1 = new Formula("y1 * 3 - 8 / 2 + 4 * (8 - 9 * 2) / 2 * x7");
            Assert.AreEqual(-12, (double)f1.Evaluate(s => (s == "x7") ? 1 : 4));
        }

        [TestMethod]
        public void test5()
        {
            Formula f1 = new Formula("x1+(x2+(x3+(x4+(x5+x6))))");
            Assert.AreEqual(6, (double)f1.Evaluate(s => 1));
        }

        [TestMethod]
        public void test6()
        {
            Formula f1 = new Formula("((((x1+x2)+x3)+x4)+x5)+x6");
            Assert.AreEqual(12, (double)f1.Evaluate(s => 2));
        }

        [TestMethod]
        public void test7()
        {
            Formula f1 = new Formula("a4-a4*a4/a4");
            Assert.AreEqual(0, (double)f1.Evaluate(s => 3));
        }

        [TestMethod]
        public void test8()
        {
            Formula f1 = new Formula("(2+3+5) / 4");
            Assert.AreEqual(2.5, (double)f1.Evaluate(s => 1));
        }

        [TestMethod]
        public void extraTest1()
        {
            Formula f1 = new Formula("5e+1+2");
            Assert.AreEqual(52, (double)f1.Evaluate(s => 1));
        }

        [TestMethod]
        public void extraTest2()
        {
            Formula f1 = new Formula("2+3*5+(3+4*8)*5+2");
            Assert.AreEqual(194, (double)f1.Evaluate(s => 1));
        }

        [TestMethod]
        public void extraTest3()
        {
            Formula f1 = new Formula("2+3*(3+5)");
            Assert.AreEqual(26, (double)f1.Evaluate(s => 1));
        }

        [TestMethod]
        public void extraTest4()
        {
            Formula f1 = new Formula("2+(3+5*9)");
            Assert.AreEqual(50, (double)f1.Evaluate(s => 1));
        }

        /// <summary>
        /// Test9-10: Test override Equals method
        /// </summary>
        [TestMethod]
        public void test9()
        {
            Formula f1 = new Formula("X2 + Y4");
            Formula f2 = new Formula("X2 + Y4");
            Assert.IsTrue(f1.Equals(f2));
        }

        [TestMethod]
        public void test10()
        {
            Formula f1 = new Formula("2.0 + x7");
            Formula f2 = new Formula("2.000 + x7");
            Assert.IsTrue(f1.Equals(f2));
        }

        /// <summary>
        /// Test11-12: Test GetHashCode method
        /// </summary>
        [TestMethod]
        public void test11()
        {
            Formula f1 = new Formula("x2 + 3");
            Formula f2 = new Formula("x2+ 3");
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());
        }

        [TestMethod]
        public void test12()
        {
            Formula f1 = new Formula("x3 + 3");
            Formula f2 = new Formula("x3 + 3 *4");
            Assert.IsFalse(f1.GetHashCode() == f2.GetHashCode());
        }

        /// <summary>
        /// Test13-15: Test GetVariables method
        /// </summary>
        [TestMethod]
        public void test13()
        {
            Formula f1 = new Formula("x2 + x2 -3");
            List<string> list = new List<string>(f1.GetVariables());
            Assert.AreEqual(1, list.Count);
        }

        [TestMethod]
        public void test14()
        {
            Formula f1 = new Formula("x2 +x3+y4+5");
            List<string> list = new List<string>(f1.GetVariables());
            Assert.AreEqual(3, list.Count);
        }

        [TestMethod]
        public void test15()
        {
            Formula f1 = new Formula("5 + 3");
            List<string> list = new List<string>(f1.GetVariables());
            Assert.AreEqual(0, list.Count);
        }

        /// <summary>
        /// Test16-26: Exceptions tester
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(Formula.FormulaFormatException))]
        public void test16()
        {
            Formula f = new Formula("*");
        }

        [TestMethod]
        [ExpectedException(typeof(Formula.FormulaFormatException))]
        public void test17()
        {
            Formula f = new Formula("2*3+");
        }

        [TestMethod]
        [ExpectedException(typeof(Formula.FormulaFormatException))]
        public void test18()
        {
            Formula f = new Formula(")8");
        }

        [TestMethod]
        [ExpectedException(typeof(Formula.FormulaFormatException))]
        public void test19()
        {
            Formula f = new Formula("(2+5)+8)");
        }

        [TestMethod]
        [ExpectedException(typeof(Formula.FormulaFormatException))]
        public void test20()
        {
            Formula f = new Formula("+8-6");
        }

        [TestMethod]
        [ExpectedException(typeof(Formula.FormulaFormatException))]
        public void test21()
        {
            Formula f = new Formula("9++10");
        }

        [TestMethod]
        [ExpectedException(typeof(Formula.FormulaFormatException))]
        public void test22()
        {
            Formula f = new Formula("6 0");
        }

        [TestMethod]
        [ExpectedException(typeof(Formula.FormulaFormatException))]
        public void test23()
        {
            Formula f = new Formula("");
        }

        [TestMethod]
        [ExpectedException(typeof(Formula.FormulaFormatException))]
        public void test24()
        {
            Formula f = new Formula("5-3(3)2");
        }

        [TestMethod]
        [ExpectedException(typeof(Formula.FormulaFormatException))]
        public void test25()
        {
            Formula f = new Formula("2x + 3");
        }

        [TestMethod]
        [ExpectedException(typeof(Formula.FormulaFormatException))]
        public void test26()
        {
            Formula f = new Formula("2x");
        }

        [TestMethod]
        public void extraExceptionTest()
        {
            Formula f = new Formula("3 / x2");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(Formula.FormulaError));
        }

        [TestMethod()]
        public void extraExceptionTest1()
        {
            Formula f = new Formula("10/0");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(Formula.FormulaError));
        }

        /// <summary>
        /// Test27-28: The operator tester
        /// </summary>
        [TestMethod]
        public void test27()
        {
            Formula f1 = null;
            Formula f2 = null;
            Assert.IsTrue(f1 == f2);
            Assert.IsFalse(f1 != f2);
        }

        [TestMethod]
        public void test28()
        {
            Formula f1 = null;
            Formula f2 = new Formula("x2 + 3");
            Formula f3 = new Formula("x2 + 3");
            Formula f4 = new Formula("y2 + 3");
            Assert.IsTrue(f1 != f2);
            Assert.IsFalse(f1 == f2);
            Assert.IsTrue(f2 == f3);
            Assert.IsFalse(f2 == f4);
            Assert.IsTrue(f2 != f4);
            Assert.IsFalse(f2 != f3);
        }
        /// <summary>
        /// Test29-30: The normalizor and validator tester
        /// </summary>
        [TestMethod]
        public void Test29()
        {
            Formula f1 = new Formula("x2 + x8", s => toUpperExtension.toUpperExtension.toUpper(s), s => true);
            Assert.AreEqual("X2+X8", f1.ToString());
        }

        [TestMethod]
        public void Test30()
        {
            Formula f1 = new Formula("x2 + x8", s => toUpperExtension.toUpperExtension.toUpper(s), s => true);
            Formula f2 = new Formula("X2 + X8");
            Assert.IsTrue(f1.Equals(f2));
        }
    }
}

/// <summary>
/// This is an extension which would be used in test case
/// </summary>
/// Author: Qixiang Chao
namespace toUpperExtension
{
    /// <summary>
    /// This is an static class which called toUpperExtension and contains a static method
    /// </summary>
    public static class toUpperExtension
    {
        /// <summary>
        /// This method is used for the test case. It would return the upper case variable
        /// For example, the toUpper("x5") would return "X5"
        /// </summary>
        /// <param name="s">The string that should be handled</param>
        /// <returns>A casted string</returns>
        public static string toUpper(this string s)
        {
            List<string> list = new List<string>();
            list.Add(s);
            string[] array = list.ToArray();
            string temp = array[0].ToUpper();
            for (int i = 1; i < array.Length; i++)
            {
                temp += array[i];
            }
            return temp;
        }
    }
}



