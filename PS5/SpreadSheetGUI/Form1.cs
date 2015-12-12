
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using SS;
using SpreadsheetUtilities;

namespace SpreadSheetGUI
{
    /// <summary>
    /// This is the spreadsheet GUI that allows our form interact with user
    /// </summary>
    ///Author: TFBoys
    public partial class SpreadsheetForm : Form
    {
        /// <summary>
        /// This is the colunm number of the form
        /// </summary>
        private int column;
        /// <summary>
        /// This is the row number of the form 
        /// </summary>
        private int row;
        /// <summary>
        /// The tough work of our previous several weeks!
        /// Our spreadsheet
        /// </summary>
        private Spreadsheet sheet;
        /// <summary>
        /// This is used to store the result of the formula
        /// </summary>
        private String value;
        /// <summary>
        /// This is the judger variable in order to check whether the current form has been saved
        /// </summary>
        private bool saved;
        /// <summary>
        /// The read only Func variable Validator. The dafault Validator is s => true
        /// </summary>
        private readonly Func<string, bool> Validator;
        /// <summary>
        /// The read only Func variable Normalizer. The dafault Normalizer is s => s
        /// </summary>
        private readonly Func<string, string> Normalizer;
        /// <summary>
        /// The constant string variable which store the version information
        /// </summary>
        private const string VERSION = "default";

        private readonly object thisLock = new object();
        /// <summary>
        /// This is the constructor of the form. No parameter passed 
        /// </summary>
        public SpreadsheetForm()
        {
            //Initializing the form and variables
            InitializeComponent();
            Validator = s => true;
            Normalizer = s => s;
            sheet = new Spreadsheet();
            saved = false;
            spreadsheetPanel1.GetSelection(out column, out row);
            string cellName = getCellName(column, row);
            cellNameBox.Text = cellName;
        }

        /// <summary>
        /// Occur when "Save" button is clicked, call the "Save" method in Spreadsheet class
        /// The saving type is sprd as default. The optional saving type is txt in order to check the
        /// XML file written correctly. 
        /// </summary>
        /// <exception cref="ArgumentException">Occurs if saving failed</exception>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            //The filter of the saveFileDialog, allows user to choose the saving type they wanted to save
            saveFileDialog1.Filter = "Spreadsheet Files (*.sprd)|*.sprd|Text File (.txt)|*.txt";
            //Show the saving dialog
            saveFileDialog1.ShowDialog();
            //Obtain the file name
            String filepath = saveFileDialog1.FileName;
            try
            {
                //Tring save the current form and switch the saved to true
                sheet.Save(filepath);
                saved = true;
            }
            catch (ArgumentException)
            {
                //If fail to save, show the error message to user
                MessageBox.Show("File could not been saved");
            }
        }

        /// <summary>
        /// Helper method in order to obtain the current coordinate of the form
        /// Using current cell name to obtain the corrdinate(ASCII)
        /// </summary>
        /// <param name="column">The column number of the form with the keyword "out"</param>
        /// <param name="row">The row number of the form with the keyword "out"</param>
        /// <param name="cellName">The cell name</param>
        private void getCord(out int column, out int row, string cellName)
        {
            //Getting the column numer as an integer 
            column = (char)cellName[0] - 65;
            //Parse the row number 
            int.TryParse(cellName.Substring(1), out row);
            row--;
        }

        /// <summary>
        /// Occur when "Open" button is clicked
        /// User can open the defalut sprd file or open all files
        /// </summary>
        /// <exception cref="SpreadsheetReadWriteException">Occur when user select incorrect file</exception>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Using filter to filte sprd file or all files type 
            openFileDialog1.Filter = "Spreadsheet Files (*.sprd)|*.sprd|All Files (*.*)|*.*";
            //Show dialog
            openFileDialog1.ShowDialog();
            //Getting file name
            string filePath = openFileDialog1.FileName;
            try
            {
                //Re-assign a new Spreadsheet using four parameters constructor
                sheet = new Spreadsheet(filePath, Validator, Normalizer, VERSION);
                int tempCol;
                int tempRow;
                //Read the saved file by using for-each loop
                //Getting all coordinates and set the values to the spreadsheetPanel
                foreach (string cellName in sheet.GetNamesOfAllNonemptyCells())
                {
                    getCord(out tempCol, out tempRow, cellName);
                    spreadsheetPanel1.SetValue(tempCol, tempRow, sheet.GetCellValue(cellName).ToString());
                }
            }
            //If fail, SpreadsheetReadWriteException would be catched and an error message box would be shown to user as well
            catch (SpreadsheetReadWriteException)
            {
                MessageBox.Show("Please select a file to open");
            }
        }

        /// <summary>
        /// Helper method in order to obtain the cell name by using the corrdinate
        /// The cell name consists of an upper case character following decimal row number
        /// ie: A1, B3, Z99
        /// </summary>
        /// <param name="column">The column numer</param>
        /// <param name="row">The row number</param>
        /// <returns></returns>
        private string getCellName(int column, int row)
        {
            //Using ASCII code idea
            return new string((char)(column + 65), 1) + "" + (row + 1);
        }

        /// <summary>
        /// This event handler occurs when user clicking the cell
        /// When a cell is clicked and the content box is also changed by user, the value and cell name would also 
        /// changed. This method is the hardest part in this program
        /// If user enter a correct formula but forget to press the enter key, click other cell the formula would still be 
        /// evaluated
        /// </summary>
        /// <param name="sender"></param>
        private void spreadsheetPanel1_SelectionChanged(SS.SpreadsheetPanel sender)
        {
            //Getting the cell name by invoking the getCellName method
            string cellName = getCellName(column, row);
            object[] parameters = new object[] { column, row, contentsBox.Text };
            if (column != 0 && row != 0 || contentsBox.Text != string.Empty)
            {
                backgroundWorker2.RunWorkerAsync(parameters);
            }
            spreadsheetPanel1.GetSelection(out column, out row);
            cellName = getCellName(column, row);
            cellNameBox.Text = cellName;
            string temp;
            spreadsheetPanel1.GetValue(column, row, out temp);
            contentsBox.Text = temp;
            valueBox.Text = temp;
        }

        /// <summary>
        /// If user enter anything in the content box, the selected cell content would also changed same with the content box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contents_TextChanged(object sender, EventArgs e)
        {
            spreadsheetPanel1.SetValue(this.column, this.row, contentsBox.Text);
        }

        /// <summary>
        /// If the enter key was pressed, the formula would be evaluated and the result would be stored in the cell 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contentsPressed(object sender, KeyPressEventArgs e)
        {
            //If the enter key is pressed, the formula would be evaluated
            //and the result would be store in the selected cell
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Return)
            {
                object[] parameters = new object[] { column, row, contentsBox.Text };
                backgroundWorker1.RunWorkerAsync(parameters);
            }
        }

        /// <summary>
        /// Occurs when the "Close" button was clicked
        /// If the form has been changed and not been saved yet, the save dialog would been shown for user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (e is FormClosingEventArgs)
            {
                SpreadsheetForm_FormClosing(sender, (FormClosingEventArgs)e);
            }
            Close();
        }

        /// <summary>
        /// Occurs when help button was clicked, the instruction of the form would be shown to user
        /// The help information include usage of all functions that have been realized in the application
        /// and also contains the author information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Help_Click(object sender, EventArgs e)
        {
            MessageBox.Show("SpreadSheet Help:\n" +
                "1. Navigating the Spreadsheet:"
                + " You can navigate the spreadsheet by clicking the cell of the spreadsheet GUI"
                + "\n2. Editing the Cell Contents:"
                + " You can enter the formula/double number/string in the contents box and you will see the change in the cell"
                + "\nie: =3+2"
                + "\nGetting result: When you click enter key or click other cell, the result would be shown in the cell"
                + "(Expected exception: When entered an invalid formula such as \"=2A+1\", the value box would show the error message)"
                + "\n3. Cell Name: When you click any cell on the spreadsheet, the cell name box would show the cell name to user"
                + "\n4. File Menu: The file menu allows you to create new sheets, open saved sheets, and save the currently active"
                + " spreadsheet. Closing window will close the application"
                + "\n5. Help button: To show the help information for user"
                + "\n6. Author Information: Qixiang Chao & Jin He");
        }

        /// <summary>
        /// Occurs when "New" button was clicked, a new form would appear to user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            FormApplicationContext.getAppContext().RunForm(new SpreadsheetForm());
        }

        /// <summary>
        /// When the right upper corner closing button was clicked and the sheet have been changed without saving,
        /// the save dialog would be shown to user to let user decide whether to save the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpreadsheetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sheet.Changed == true && saved == false)
            {
                DialogResult boxResult = MessageBox.Show("Do you want to save change?", "", MessageBoxButtons.YesNo);
                if (boxResult == DialogResult.Yes)
                {
                    saveToolStripMenuItem_Click_1(sender, e);
                }
            }
        }

        /// <summary>
        /// backgroundworker1_DoWork
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            lock (thisLock)
            {
                object[] parameters = e.Argument as object[];
                int oldCol = (int)parameters[0];
                int oldRow = (int)parameters[1];
                string oldContent = (string)parameters[2];
                this.Invoke(new Action(() =>
                {
                    try
                    {
                        sheet.SetContentsOfCell(getCellName(oldCol, oldRow), oldContent);
                        if (sheet.GetCellValue(getCellName(oldCol, oldRow)) is Formula.FormulaError)
                        {
                            valueBox.Text = "Formula Error";
                            MessageBox.Show("Formula error occur, please check your formula");
                            return;
                        }
                        value = sheet.GetCellValue(getCellName(oldCol, oldRow)).ToString();
                        valueBox.Text = value;
                        spreadsheetPanel1.SetValue(oldCol, oldRow, value);
                    }
                    catch (Formula.FormulaFormatException)
                    {
                        //If user entered an invalid formula, the error message would be shown in the value box
                        valueBox.Text = "Formula Format Error";
                        MessageBox.Show("Please enter a correct formula and continue");
                        return;
                    }
                    catch (CircularException)
                    {
                        //If user entered an invalid formula, the error message would be shown in the value box
                        valueBox.Text = "Formula Error";
                        MessageBox.Show("No circular is allowed");
                        return;
                    }
                }));
            }
        }

        /// <summary>
        /// Clear button used to clear the entire form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearButton_Click(object sender, EventArgs e)
        {
            spreadsheetPanel1.Clear();
            contentsBox.Text = "";
            valueBox.Text = "";
            saved = true;
            spreadsheetPanel1.SetSelection(0, 0);
            spreadsheetPanel1_SelectionChanged(spreadsheetPanel1);
        }

        /// <summary>
        /// backgroundworker2. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            lock (thisLock)
            {
                object[] parameters = e.Argument as object[];
                int oldCol = (int)parameters[0];
                int oldRow = (int)parameters[1];
                string oldContent = (string)parameters[2];
                this.Invoke(new Action(() =>
                {
                    try
                    {
                        sheet.SetContentsOfCell(getCellName(oldCol, oldRow), oldContent);
                        if (sheet.GetCellValue(getCellName(oldCol, oldRow)) is Formula.FormulaError)
                        {
                            valueBox.Text = "Formula Error";
                            MessageBox.Show("Formula error occur, please check your formula");
                            return;
                        }
                        value = sheet.GetCellValue(getCellName(oldCol, oldRow)).ToString();
                        spreadsheetPanel1.SetValue(oldCol, oldRow, value);
                    }
                    catch (Formula.FormulaFormatException)
                    {
                        MessageBox.Show("Please correct the formula and continue");
                        return;
                    }
                    catch (CircularException)
                    {
                        MessageBox.Show("Please correct the formula and continue");
                        return;
                    }
                    catch (InvalidNameException)
                    {
                        return;
                    }
                }));
            }
        }
    }
}