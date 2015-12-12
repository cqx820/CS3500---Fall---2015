using SS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using SpreadsheetUtilities;
using System.Threading;
using System.Xml;

/// <summary>
/// This is a test class which is used to test the spreadsheet
/// </summary>

namespace SpreadsheetTester
{
    [TestClass()]
    public class Tester
    {
        /// <summary>
        /// IsValid test: test1-test3
        /// </summary>
        [TestMethod]
        public void test1()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("X1", "a");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void test2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("2X", "x");
        }

        [TestMethod]
        public void test3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=B1+C1");
        }

        /// <summary>
        /// Normalize test: test4-test8
        /// </summary>
        [TestMethod]
        public void test4()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "abc");
            Assert.AreEqual("", sheet.GetCellContents("a1"));
        }

        [TestMethod]
        public void test5()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(s => true, s => s.ToUpper(), "");
            sheet.SetContentsOfCell("a1", "abc");
            Assert.AreEqual("abc", sheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void test6()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("a1", "1");
            sheet.SetContentsOfCell("A1", "abc");
            sheet.SetContentsOfCell("B1", "=a1");
            Assert.AreEqual(1.0, (double)sheet.GetCellValue("B1"));
        }

        [TestMethod]
        public void test7()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(s => true, s => s.ToUpper(), "");
            sheet.SetContentsOfCell("a1", "1");
            sheet.SetContentsOfCell("A1", "2");
            sheet.SetContentsOfCell("B1", "=a1");
            Assert.AreEqual(2.0, (double)sheet.GetCellValue("B1"));
        }

        [TestMethod]
        public void test8()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Assert.IsFalse(sheet.Changed);
            sheet.SetContentsOfCell("A1", "x");
            Assert.IsTrue(sheet.Changed);
        }

        /// <summary>
        /// Formula test: test9-test14
        /// </summary>
        [TestMethod]
        public void test9()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "2.0");
            sheet.SetContentsOfCell("A2", "0.0");
            sheet.SetContentsOfCell("A3", "=A1 / A2");
            Assert.IsInstanceOfType(sheet.GetCellValue("A3"), typeof(Formula.FormulaError));
        }

        [TestMethod]
        public void test10()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "2.0");
            sheet.SetContentsOfCell("A2", "abc");
            sheet.SetContentsOfCell("A3", "=A1 / A2");
            Assert.IsInstanceOfType(sheet.GetCellValue("A3"), typeof(Formula.FormulaError));
        }

        [TestMethod]
        public void test11()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "2.0");
            sheet.SetContentsOfCell("A3", "=A1 / A2");
            Assert.IsInstanceOfType(sheet.GetCellValue("A3"), typeof(Formula.FormulaError));
        }
        [TestMethod]
        public void test12()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "2.0");
            sheet.SetContentsOfCell("A2", "2.0");
            sheet.SetContentsOfCell("A3", "=A1 / A2");
            Assert.AreEqual(1.0, sheet.GetCellValue("A3"));
        }

        [TestMethod]
        public void test13()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=2.0");
            Assert.AreEqual(2.0, sheet.GetCellValue("A1"));
        }

        [TestMethod]
        public void test14()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "2.0");
            sheet.SetContentsOfCell("A2", "3.0");
            sheet.SetContentsOfCell("A3", "4.0");
            sheet.SetContentsOfCell("A4", "5.1");
            sheet.SetContentsOfCell("A5", "14.0");
            sheet.SetContentsOfCell("A6", "7.0");
            sheet.SetContentsOfCell("B1", "=A1*A2");
            sheet.SetContentsOfCell("B2", "=A3+A4");
            sheet.SetContentsOfCell("B3", "=A5/A6");
            Assert.AreEqual(6.0, sheet.GetCellValue("B1"));
            Assert.AreEqual(9.1, sheet.GetCellValue("B2"));
            Assert.AreEqual(2.0, sheet.GetCellValue("B3"));
        }

        /// <summary>
        /// Saving test: test15-test19
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void test15()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.Save("c:\\123\\file.txt");
        }

        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void test16()
        {
            AbstractSpreadsheet sheet = new Spreadsheet("c:\\123\\file.txt", s => true, s => s, "");
        }

        [TestMethod()]
        public void test17()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "x");
            sheet.Save("filename.txt");
            sheet = new Spreadsheet("filename.txt", s => true, s => s, "default");
            Assert.AreEqual("x", sheet.GetCellContents("A1"));
        }


        [TestMethod()]
        [ExpectedException(typeof(SpreadsheetReadWriteException))]
        public void test18()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "x");
            sheet.Save("filename.txt");
            sheet = new Spreadsheet("filename.txt", s => true, s => s, "version");
            Assert.AreEqual("x", sheet.GetCellContents("A1"));
        }

        [TestMethod()]
        public void test19()
        {
            AbstractSpreadsheet sheet = new Spreadsheet(s => true, s => s, "QixiangChao");
            sheet.Save("filename.txt");
            Assert.AreEqual("QixiangChao", new Spreadsheet().GetSavedVersion("filename.txt"));
        }
    }
}