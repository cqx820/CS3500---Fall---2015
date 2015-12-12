using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;
using SpreadsheetUtilities.stringExtension;
using formulaExtension;

/// <summary>
/// This is the SS project 
/// </summary>
namespace SS
{
    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a valid cell name if and only if:
    ///   (1) its first character is an underscore or a letter
    ///   (2) its remaining characters (if any) are underscores and/or letters and/or digits
    /// Note that this is the same as the definition of valid variable from the PS3 Formula class.
    /// 
    /// For example, "x", "_", "x2", "y_15", and "___" are all valid cell  names, but
    /// "25", "2x", and "&" are not.  Cell names are case sensitive, so "x" and "X" are
    /// different cell names.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  (This
    /// means that a spreadsheet contains an infinite number of cells.)  In addition to 
    /// a name, each cell has a contents and a value.  The distinction is important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        /// <summary>
		/// DG contains all dependency data for the spreadsheet.
		/// Invariant: 
		/// no cells are allowed to have a circular dependency
		/// </summary>
        private DependencyGraph DG;
        /// <summary>
		/// keys are the cells' names. values are the cells themselves.
		/// </summary>
        private Dictionary<string, Cell> cells;
        /// <summary>
		/// constrctor
		/// </summary>
        public Spreadsheet()
        {
            //Initialize variables
            this.DG = new DependencyGraph();
            this.cells = new Dictionary<string, Cell>();
        }

        /// <summary>
		/// a class of cell
		/// </summary>
        private class Cell
        {
            /// <summary>
            /// The object cellContents
            /// </summary>
            private object cellContents;
            /// <summary>
            /// /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
            /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
            /// in the grid.)
            /// 
            /// If a cell's contents is a string, its value is that string.
            /// 
            /// If a cell's contents is a double, its value is that double.
            /// 
            /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
            /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
            /// of course, can depend on the values of variables.  The value of a variable is the 
            /// value of the spreadsheet cell it names (if that cell's value is a double) or 
            /// is undefined (otherwise).
            /// </summary>
            /// <param name="cellContents">object type cellContents</param>
            public Cell(object cellContents)
            {
                //Initialize variables
                this.cellContents = cellContents;
            }
            /// <summary>
            /// Get the contents in the cell
            /// </summary>
            /// <returns>return the global variable cellContents</returns>
            public object getContent()
            {
                //return the contents of the cell
                return this.cellContents;
            }

            /// <summary>
            /// set the content of the cell
            /// </summary>
            /// <param name="cellContents">object type cellContents</param>
            public void setContent(object cellContents)
            {
                //set the global variable celContents to the object cellContents
                this.cellContents = cellContents;
            }
        }
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        /// <param name="name">string name</param>
        /// <exception cref="InvalidNameException">If name is null or invalid, throws an InvalidNameException.</exception>
		/// <returns>returns the contents (as opposed to the value) of the named cell.</returns>
        public override object GetCellContents(string name)
        {
            //If name is null or the name is invalid, throw an InvalidNameException
            if (ReferenceEquals(name, null) || !formulaExtension.formulaExtension.isVariable(name))
            {
                throw new InvalidNameException();
            }
            //If the dictionary contains the key name
            if (cells.ContainsKey(name))
            {
                //Returnt the value from the key position
                return cells[name].getContent();
            }
            //Else, return a empty string
            return "";
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        /// <returns>return the keys of the dictionary</returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            //Return all keys of the spreadSheet
            return cells.Keys;
        }

        /// <summary>
        /// If the formula parameter is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.  (No change is made to the spreadsheet.)
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        /// <exception cref="ArgumentNullException"> If the formula parameter is null, throws an ArgumentNullException.</exception>
		/// <exception cref="InvalidNameException"> Otherwise, if name is null or invalid, throws an InvalidNameException.</exception>
		/// <exception cref="CircularException">
        /// </exception>
        /// <param name="name">the string type name</param>
        /// <param name="formula">Formula type formula</param>
        /// <returns>The method returns a
		/// Set consisting of name plus the names of all other cells whose value depends,
		/// directly or indirectly, on the named cell.</returns>
        public override ISet<string> SetCellContents(string name, Formula formula)
        {
            //If the name is null or the name is invalid, then throw an InvalidNameException
            if (ReferenceEquals(name, null) || !formulaExtension.formulaExtension.isVariable(name))
            {
                throw new InvalidNameException();
            }
            //If the formula is null, then throw the ArgumentNullException
            if (ReferenceEquals(formula, null))
            {
                throw new ArgumentNullException();
            }
            //Declare and initialize an new cell
            Cell cell = new Cell(formula);
            //Add the new cell contains formula into the key position in the spreedsheet
            cells[name] = cell;
            //If the name has dependents
            if (DG.HasDependents(name))
            {
                //Remove the pairs by using for-each loop
                foreach (string s in DG.GetDependents(name))
                {
                    DG.RemoveDependency(name, s);
                }
            }
            //Add the variable of the formula into the dependencygraph by using loop
            foreach (string s in formula.GetVariables())
            {
                DG.AddDependency(name, s);
            }
            //Declare and initialize a new HashSet and called GetCellsToRecalculate method to get an Enumerable as parameter
            HashSet<string> set = new HashSet<string>(GetCellsToRecalculate(name));
            return set;
        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        /// <exception cref="ArgumentNullException">If text is null, throws an ArgumentNullException.</exception>
		/// <exception cref="InvalidNameException">Otherwise, if name is null or invalid, throws an InvalidNameException.</exception>
        /// <param name="name">string type name</param>
        /// <param name="text">string type text</param>
        /// <returns>The method returns a
		/// set consisting of name plus the names of all other cells whose value depends, 
		/// directly or indirectly, on the named cell.</returns>
        public override ISet<string> SetCellContents(string name, string text)
        {
            //If the name is null or the name is invalid, then throw an InvalidNameException
            if (ReferenceEquals(name, null) || !formulaExtension.formulaExtension.isVariable(name))
            {
                throw new InvalidNameException();
            }
            //If the formula is null, then throw the ArgumentNullException
            if (ReferenceEquals(text, null))
            {
                throw new ArgumentNullException();
            }
            //Declare and initialize an new cell
            Cell cell = new Cell(text);
            //Add the new cell contains formula into the key position in the spreedsheet
            cells[name] = cell;
            //If the name has the dependents
            if (DG.HasDependents(name))
            {
                //Using for loop to remove the dependents
                foreach (string s in DG.GetDependents(name))
                {
                    DG.RemoveDependency(name, s);
                }
            }
            //If the text is empty, then return a new HashSet
            if (text == "")
            {
                cells.Remove(name);
            }
            //Declare and initialize a new HashSet and called GetCellsToRecalculate method to get an Enumerable as parameter
            HashSet<string> setForReturn = new HashSet<string>(GetCellsToRecalculate(name));
            return setForReturn;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        /// <exception cref="InvalidNameException">If name is null or invalid, throws an InvalidNameException.</exception>
        /// <param name="name">string type name</param>
        /// <param name="number">double type number</param>
        /// <returns>
        /// The method returns a
		/// set consisting of name plus the names of all other cells whose value depends, 
		/// directly or indirectly, on the named cell.</returns>
        public override ISet<string> SetCellContents(string name, double number)
        {
            //If the name is null or the name is invalid, then throw an InvalidNameException
            if (ReferenceEquals(name, null) || !formulaExtension.formulaExtension.isVariable(name))
            {
                throw new InvalidNameException();
            }
            //Declare and initialize an new cell
            Cell cell = new Cell(number);
            //Add the new cell contains formula into the key position in the spreedsheet
            cells[name] = cell;
            //If the name has the dependents
            if (DG.HasDependents(name))
            {
                //Using for loop to remove the dependents
                foreach (string s in DG.GetDependents(name))
                {
                    DG.RemoveDependency(name, s);
                }
            }
            //Declare and initialize a new HashSet and called GetCellsToRecalculate method to get an Enumerable as parameter
            HashSet<string> setForReturn = new HashSet<string>(GetCellsToRecalculate(name));
            return setForReturn;

        }

        /// <summary>
		/// If name is null, throws an ArgumentNullException.
		/// 
		/// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
		/// 
		/// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
		/// values depend directly on the value of the named cell.  In other words, returns
		/// an enumeration, without duplicates, of the names of all cells that contain
		/// formulas containing name.
		/// 
		/// For example, suppose that
		/// A1 contains 3
		/// B1 contains the formula A1 * A1
		/// C1 contains the formula B1 + A1
		/// D1 contains the formula B1 - C1
		/// The direct dependents of A1 are B1 and C1
		/// </summary>
        /// <param name="name">string type name</param>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            //Declare and initialize a new HashSet
            HashSet<string> set = new HashSet<string>();
            //If the name is null or the name is invalid. Throw an InvalidNameException
            if (ReferenceEquals(name, null) || !formulaExtension.formulaExtension.isVariable(name))
            {
                throw new InvalidNameException();
            }
            else
            {
                //If the spreadsheet contains the key name
                if (cells.ContainsKey(name))
                {
                    //Using for-each loop to iterate the dependees of name
                    foreach (string s in DG.GetDependees(name))
                    {
                        //Return the Ienumerable    
                        yield return s;
                    }
                }
            }

            yield break;
        }
    }
}

namespace formulaExtension
{
    /// <summary>
    /// This is the static formulaExtension class which contains one method
    /// </summary>
    public static class formulaExtension
    {
        /// <summary>
        /// The only method contained. This method is used to judge wheteher a string is valid
        /// </summary>
        /// <param name="s">strng type parameter</param>
        /// <returns>bool value</returns>
        public static bool isVariable(string s)
        {
            //Using regular expression to check whether string s is a variable
            return Regex.IsMatch(s, "^[a-zA-Z_](?:[a-zA-Z_]|\\d)*$");
        }
    }
}