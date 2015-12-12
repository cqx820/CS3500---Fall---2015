using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.IO;

/// <summary>
/// This is the SS project 
/// </summary>

namespace SS
{
    // PARAGRAPHS 2 and 3 modified for PS5.
    /// <summary>
    /// An AbstractSpreadsheet object represents the state of a simple spreadsheet.  A 
    /// spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a cell name if and only if it consists of one or more letters,
    /// followed by one or more digits AND it satisfies the predicate IsValid.
    /// For example, "A15", "a15", "XY032", and "BC7" are cell names so long as they
    /// satisfy IsValid.  On the other hand, "Z", "X_", and "hello" are not cell names,
    /// regardless of IsValid.
    /// 
    /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
    /// must be normalized with the Normalize method before it is used by or saved in 
    /// this spreadsheet.  For example, if Normalize is s => s.ToUpper(), then
    /// the Formula "x3+a5" should be converted to "X3+A5" before use.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important.
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
        /// A class of cell
        /// </summary>
        private class Cell
        {
            /// <summary>
			/// The contents of the cell
			/// </summary>
            private object content;

            /// <summary>
			/// The value of the cell
			/// </summary>
            private object value;

            /// <summary>
            /// The basic constructor, which contains two parameters.
            /// </summary>
            /// <param name="content">object type content</param>
            /// <param name="lookup">delegate type method</param>
            public Cell(object content, Func<string, double> lookup)
            {
                //Initialize variables
                this.content = content;
                Lookup = lookup;
            }

            /// <summary>
            /// Lookup contains setter and getter
            /// </summary>
            public Func<string, double> Lookup
            {
                get;
                private set;
            }

            /// <summary>
            /// The method used to get the value of the cell
            /// </summary>
            /// <returns>The value of the cell</returns>
            public object getValue()
            {
                //Return the value of cell
                return this.value;
            }

            /// <summary>
            /// Set the current cell value to the new value
            /// </summary>
            /// <param name="value">Object type value</param>
            public void setValue(object value)
            {
                this.value = value;
            }

            /// <summary>
            /// The content getter of the cell
            /// </summary>
            /// <returns>The content of the cell</returns>
            public object getContents()
            {
                //Return the content of the cell
                return this.content;
            }

            /// <summary>
            /// The content setter of the cell. If the content is formula, then evaluate the value and set
            /// </summary>
            /// <param name="content">Object type content</param>
            public void setContents(object content)
            {
                //Assign the value of the content to this value
                this.value = content;
                //If the content is formula, convert the content type to Formula type and evaluate the result
                if (content is Formula)
                {
                    //Assign the result to the value
                    this.value = (content as Formula).Evaluate(Lookup);
                }
                //Assign the new content to the current content
                this.content = content;
            }
        }

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
        /// The zero parameter constructor, the defalut isValid is true, no normalizing and version is default
        /// </summary>
        public Spreadsheet() : base(s => true, s => s, "default")
        {
            //Initialize variables
            Changed = false;
            DG = new DependencyGraph();
            cells = new Dictionary<string, Cell>();
        }

        // ADDED FOR PS5
        /// <summary>
        /// Constructs an abstract spreadsheet by recording its variable validity test,
        /// its normalization method, and its version information.  The variable validity
        /// test is used throughout to determine whether a string that consists of one or
        /// more letters followed by one or more digits is a valid cell name.  The variable
        /// equality test should be used thoughout to determine whether two variables are
        /// equal.
        /// </summary>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            //Initialize variables
            Changed = false;
            DG = new DependencyGraph();
            cells = new Dictionary<string, Cell>();
        }

        /// <summary>
        /// To read a saved spreadsheet from a file, then construct a new spreadsheet
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="isValid"></param>
		/// <param name="normalize"></param>
		/// <param name="version"></param>
        public Spreadsheet(string filePath, Func<string, bool> isValid, Func<string, string> normalize, string version) : base(isValid, normalize, version)
        {
            //Initialize variables
            Changed = false;
            DG = new DependencyGraph();
            cells = new Dictionary<string, Cell>();
            //If the filePath is null, then throw the SpreadsheetReadWriteException
            if (ReferenceEquals(filePath, null))
            {
                throw new SpreadsheetReadWriteException("Path cannot be found");
            }
            //If the version is mismatched, then throw the SpreadsheetReadWriteException
            if (!version.Equals(GetSavedVersion(filePath)))
            {
                throw new SpreadsheetReadWriteException("The version did not match");
            }
            //Create two new string type variables
            string tempName = "";
            string tempContent = "";
            //Create the xml reader
            using (XmlReader reader = XmlReader.Create(filePath))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "spreadsheet":
                                Version = reader.GetAttribute("spreadsheet");
                                break;
                            case "cell":
                                reader.Read();
                                tempName = reader.ReadElementContentAsString();
                                tempContent = reader.ReadElementContentAsString();
                                SetContentsOfCell(tempName, tempContent);
                                break;
                            default:
                                throw new SpreadsheetReadWriteException("The format of XML is incorrect");
                        }
                    }
                }
            }
        }

        /// <summary>
		/// True if this spreadsheet has been changed              
		/// False if no change
		/// </summary>
        public override bool Changed
        {
            get;
            protected set;
        }

        /// <summary>
		/// Look up the dependency information with the name and return the value
		/// </summary>
		/// <param name="name">String type name that in the cell</param>
		/// <returns>The double type value</returns>
        public double LookUp(string name)
        {
            if (cells.ContainsKey(name))
            {
                if (cells[name].getValue() is double)
                {
                    return (double)(cells[name].getValue());
                }
            }
            throw new ArgumentNullException();
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
            //Normalizing the name
            name = Normalize(name);
            //Check the args, to decide whether to throw an InvalidNameException
            if (ReferenceEquals(name, null) || !Regex.IsMatch(Normalize(name), @"[a-zA-Z]+\d+") || !IsValid(name))
            {
                throw new InvalidNameException();
            }
            if (!cells.ContainsKey(name))
            {
                return "";
            }
            return cells[name].getContents();
        }

        // ADDED FOR PS5
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            //Check the args, to decide whether to throw an InvalidNameException
            if (ReferenceEquals(name, null) || !Regex.IsMatch(Normalize(name), @"[a-zA-Z]+\d+") || !IsValid(name))
            {
                throw new InvalidNameException();
            }
            if (cells.ContainsKey(name))
            {
                return cells[name].getValue();
            }
            return "";
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        /// <returns>return the keys of the dictionary</returns>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return cells.Keys;
        }

        // ADDED FOR PS5
        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            try
            {
                try
                {
                    using (XmlReader reader = XmlReader.Create(filename))
                    {
                        while (reader.Read())
                        {
                            if (reader.IsStartElement())
                            {
                                if (reader.Name.Equals("spreadsheet"))
                                {
                                    return reader.GetAttribute("version");
                                }
                                else
                                {
                                    throw new SpreadsheetReadWriteException("Cannot read the version");
                                }
                            }
                        }
                    }
                }
                catch (XmlException)
                {
                    throw new SpreadsheetReadWriteException("XML file cannot be parsed");
                }
            }
            //Catch some exceptions and re-throw exception
            catch (IOException ex)
            {
                if (ex is FileNotFoundException)
                {
                    throw new SpreadsheetReadWriteException("File cannot be found");
                }
                if (ex is DirectoryNotFoundException)
                {
                    throw new SpreadsheetReadWriteException("Directory cannot be found");
                }
            }
            //If file name is invalid, throw exception
            if (ReferenceEquals(filename, null) || filename.Equals(""))
            {
                throw new SpreadsheetReadWriteException("XML file cannot be empty");
            }
            throw new SpreadsheetReadWriteException("Unknown error occur");
        }

        // ADDED FOR PS5
        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>
        /// cell name goes here
        /// </name>
        /// <contents>
        /// cell contents goes here
        /// </contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.ToString() should be written as the contents.  
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            object content;
            Cell temp;
            try
            {
                using (XmlWriter writer = XmlWriter.Create(filename))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", Version);
                    foreach (string s in GetNamesOfAllNonemptyCells())
                    {
                        cells.TryGetValue(s, out temp);
                        content = temp.getContents();
                        if (content is string)
                        {
                            writer.WriteStartElement("cell");
                            writer.WriteElementString("name", s);
                            writer.WriteElementString("contents", (String)content);
                            writer.WriteEndElement();
                        }
                        else if (content is double)
                        {
                            writer.WriteStartElement("cell");
                            writer.WriteElementString("name", s);
                            writer.WriteElementString("contents", ((double)content).ToString());
                            writer.WriteEndElement();
                        }
                        else if (content is Formula)
                        {
                            writer.WriteStartElement("cell");
                            writer.WriteElementString("name", s);
                            writer.WriteElementString("contents", "=" + ((Formula)content).ToString());
                            writer.WriteEndElement();
                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Dispose();
                }
            }
            //Catch some exceptions and re-throw exception
            catch (IOException ex)
            {
                if (ex is FileNotFoundException)
                {
                    throw new SpreadsheetReadWriteException("File cannot be found");
                }
                if (ex is DirectoryNotFoundException)
                {
                    throw new SpreadsheetReadWriteException("Directory cannot be found");
                }
            }
            //If file name is invalid, throw exception
            if (ReferenceEquals(filename, null) || filename.Equals(""))
            {
                throw new SpreadsheetReadWriteException("XML file cannot be empty");
            }
        }

        // ADDED FOR PS5
        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes content.
        /// 
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            //If the name is null or the name is invalid, then throw an InvalidNameException
            if (ReferenceEquals(name, null) || !Regex.IsMatch(Normalize(name), @"[a-zA-Z]+\d+") || !IsValid(name))
            {
                throw new InvalidNameException();
            }
            //If the formula is null, then throw the ArgumentNullException
            if (ReferenceEquals(content, null))
            {
                throw new ArgumentNullException();
            }
            //If content is empty, remove the name from cell, remove the dependents of name in DG
            if (content.Equals(""))
            {
                cells.Remove(name);
                if (DG.HasDependents(name))
                {
                    foreach (string s in DG.GetDependents(name))
                    {
                        DG.RemoveDependency(name, s);
                    }
                }
                return new HashSet<string>(GetCellsToRecalculate(name));
            }
            if (char.Parse(content.Substring(0, 1)) == '=')
            {
                return SetCellContents(name, new Formula(content.Substring(1), Normalize, IsValid));
            }
            double num;
            if (Double.TryParse(content, out num))
            {
                return SetCellContents(name, num);
            }
            return SetCellContents(name, content);
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
            //If name is null, throw an ArgumentNullException
            if (ReferenceEquals(name, null))
            {
                throw new ArgumentNullException();
            }
            //If the name is null or the name is invalid. Throw an InvalidNameException
            if (!Regex.IsMatch(Normalize(name), @"[a-zA-Z]+\d+") || !IsValid(name))
            {
                throw new InvalidNameException();
            }
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
            yield break;
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
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            //If the name is null or the name is invalid, then throw an InvalidNameException
            if (ReferenceEquals(name, null) || !Regex.IsMatch(Normalize(name), @"[a-zA-Z]+\d+") || !IsValid(name))
            {
                throw new InvalidNameException();
            }
            //If the formula is null, then throw the ArgumentNullException
            if (ReferenceEquals(formula, null))
            {
                throw new ArgumentNullException();
            }
            //Normalizing the name
            name = Normalize(name);
            //Declare and initialize an new cell
            Cell cell = new Cell(formula, LookUp);
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
            IEnumerable<string> temp = DG.GetDependents(name);
            //Add the variable of the formula into the dependencygraph by using loop
            foreach (string s in formula.GetVariables())
            {
                try
                {
                    DG.AddDependency(name, s);
                }
                catch
                {
                    DG.ReplaceDependents(name, temp);
                }
            }
            foreach (string s in GetCellsToRecalculate(name))
            {
                cells[s].setContents(cells[s].getContents());
            }
            //Change changed to true
            Changed = true;
            HashSet<string> setForReturn = new HashSet<string>(GetCellsToRecalculate(name));
            setForReturn.Add(name);
            return setForReturn;
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
        protected override ISet<string> SetCellContents(string name, string text)
        {
            //If the name is null or the name is invalid, then throw an InvalidNameException
            if (ReferenceEquals(name, null) || !Regex.IsMatch(Normalize(name), @"[a-zA-Z]+\d+") || !IsValid(name))
            {
                throw new InvalidNameException();
            }
            //If the formula is null, then throw the ArgumentNullException
            if (ReferenceEquals(text, null))
            {
                throw new ArgumentNullException();
            }
            //Normalizing name
            name = Normalize(name);
            //Declare and initialize an new cell
            Cell cell = new Cell(text, LookUp);
            //Add the new cell contains formula into the key position in the spreedsheet
            cells[name] = cell;
            //If text is empty, remove the name from cell
            if (text.Equals(""))
            {
                cells.Remove(name);
            }
            if (DG.HasDependents(name))
            {
                //Remove the pairs by using for-each loop
                foreach (string s in DG.GetDependents(name))
                {
                    DG.RemoveDependency(name, s);
                }
            }
            foreach (string s in GetCellsToRecalculate(name))
            {
                cells[s].setContents(cells[s].getContents());
            }
            //Change changed to true
            Changed = true;
            HashSet<string> setForReturn = new HashSet<string>(GetCellsToRecalculate(name));
            setForReturn.Add(name);
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
        protected override ISet<string> SetCellContents(string name, double number)
        {
            //If the name is null or the name is invalid, then throw an InvalidNameException
            if (ReferenceEquals(name, null) || !Regex.IsMatch(Normalize(name), @"[a-zA-Z]+\d+") || !IsValid(name))
            {
                throw new InvalidNameException();
            }
            //Normalizing name
            name = Normalize(name);
            Cell cell = new Cell(number, LookUp);
            cells[name] = cell;
            if (DG.HasDependents(name))
            {
                foreach (string s in DG.GetDependents(name))
                {
                    DG.RemoveDependency(name, s);
                }
            }
            foreach (string s in GetCellsToRecalculate(name))
            {
                cells[s].setContents(cells[s].getContents());
            }
            Changed = true;
            HashSet<string> setForReturn = new HashSet<string>(GetCellsToRecalculate(name));
            return setForReturn;
        }
    }
}