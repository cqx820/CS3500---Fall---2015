using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using System.Collections.Generic;
using SpreadsheetUtilities;


namespace Test
{
    /// <summary>
    /// The class for testing the spreadsheet
    /// </summary>
    [TestClass]
    public class SpreadSheetTester
    {
        /// <summary>
        /// Initialize new sheet
        /// </summary>
        AbstractSpreadsheet sheet = new Spreadsheet();

        /// <summary>
        /// Test the constructor
        /// </summary>
        [TestMethod]
        public void constructorTest()
        {
            HashSet<string> set = new HashSet<string>(new Spreadsheet().GetNamesOfAllNonemptyCells());
            Assert.AreEqual(0, set.Count);
        }

        /// <summary>
        /// test1
        /// </summary>
        [TestMethod]
        public void test1()
        {
            sheet.SetCellContents("A_1", 1.0);
            Assert.AreEqual("A_1", new List<string>(sheet.GetNamesOfAllNonemptyCells())[0]);
            Assert.AreEqual(1, new List<string>(sheet.GetNamesOfAllNonemptyCells()).Count);
        }

        /// <summary>
        /// test2
        /// </summary>
        [TestMethod]
        public void test2()
        {
            sheet.SetCellContents("x_1", new Formula("x2+1"));
            Assert.AreEqual("x_1", new List<string>(sheet.GetNamesOfAllNonemptyCells())[0]);
            Assert.AreEqual(1, new List<string>(sheet.GetNamesOfAllNonemptyCells()).Count);
        }

        /// <summary>
        /// test3
        /// </summary>
        [TestMethod]
        public void test3()
        {
            sheet.SetCellContents("A2", "helloworld");
            Assert.AreEqual(1, new List<string>(sheet.GetNamesOfAllNonemptyCells()).Count);
            sheet.SetCellContents("A2", "");
            Assert.AreEqual(0, new List<string>(sheet.GetNamesOfAllNonemptyCells()).Count);
        }

        /// <summary>
        /// test4
        /// </summary>
        [TestMethod]
        public void test4()
        {
            sheet.SetCellContents("A2", "helloworld");
            Assert.AreEqual("helloworld", sheet.GetCellContents("A2"));
        }

        /// <summary>
        /// test5
        /// </summary>
        [TestMethod]
        public void test5()
        {
            sheet.SetCellContents("y_15", 1.5);
            Assert.AreEqual(1.5, sheet.GetCellContents("y_15"));
        }

        /// <summary>
        /// exception test. 
        /// expected: InvalidNameException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void test6()
        {
            sheet.SetCellContents("2x", 5.1);
        }

        /// <summary>
        /// exception test. 
        /// expected: InvalidNameException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void test7()
        {
            sheet.SetCellContents("", 5.1);
        }

        /// <summary>
        /// exception test. 
        /// expected: InvalidNameException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void test8()
        {
            sheet.SetCellContents("25", 5.1);
        }

        /// <summary>
        /// exception test. 
        /// expected: InvalidNameException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void test9()
        {
            sheet.SetCellContents(null, 5.1);
        }

        /// <summary>
        /// exception test. 
        /// expected: ArgumentNullException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void test10()
        {
            string test = null;
            sheet.SetCellContents("X5", test);
        }

        /// <summary>
        /// exception test. 
        /// expected: ArgumentNullException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void test11()
        {
            Formula f = null;
            sheet.SetCellContents("X5", f);
        }

        /// <summary>
        /// test12
        /// </summary>
        [TestMethod]
        public void test12()
        {
            sheet.SetCellContents("x2", new Formula("x1 + y1"));
            Assert.AreEqual("x1+y1", sheet.GetCellContents("x2").ToString());
        }

        /// <summary>
        /// test13
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void test13()
        {
            sheet.SetCellContents("Y1", new Formula("X1"));
            sheet.SetCellContents("X1", new Formula("Y1"));
        }

        /// <summary>
        /// test14
        /// stress test
        /// </summary>
        [TestMethod]
        public void test14()
        {
            sheet.SetCellContents("B1", new Formula("A1*2"));
            sheet.SetCellContents("C1", new Formula("B1+3"));
            List<string> list = new List<string>(sheet.SetCellContents("A1", 4));
            Assert.IsTrue(list.Contains("A1"));
            Assert.IsTrue(list.Contains("B1"));
            Assert.IsTrue(list.Contains("C1"));
        }

        /// <summary>
        /// test15
        /// </summary>
        [TestMethod]
        public void test15()
        {
            sheet.SetCellContents("A1", new Formula("d2+d3"));
            sheet.SetCellContents("A1", new Formula("12+c3"));
            Assert.AreEqual("12+c3", sheet.GetCellContents("A1").ToString());
        }

        /// <summary>
        /// test16
        /// </summary>
        [TestMethod]
        public void test16()
        {
            sheet.SetCellContents("A1", 1.5);
            sheet.SetCellContents("A1", 2.0);
            Assert.AreEqual(2.0, sheet.GetCellContents("A1"));
        }
    }
}