using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax; variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        //Global variable is a string type variable
        private string formula;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            //Initialize some variables which would be used next
            string temp = "";
            int LPattern = 0;
            int RPattern = 0;
            //If the formula is empty, then throw a FormulaFormatException
            if (formula.Length == 0 || formula == null)
            {
                throw new FormulaFormatException("Formula cannot be an empty string");
            }
            //Get the composed formula and put them into an array which called parts
            string[] parts = GetTokens(formula).ToArray();
            //If the start point of the formula is an operator, then throw exception
            if (stringExtension.stringExtension.isOperator(parts[0]) || stringExtension.stringExtension.isOperator(parts[parts.Length - 1]))
            {
                throw new FormulaFormatException("The formula does not begin properly, please check your start point");
            }
            //If the end point of the formula is "(" or ")", then throw exception
            if (parts[0].Equals(")") || parts[parts.Length - 1].Equals("("))
            {
                throw new FormulaFormatException("The formula does not end properly, please check your end point");
            }
            //int i = -1;
            //Handle all elements in the array by using for-loop
            for (int i = 0; i < parts.Length; i++)
            {
                //If "(" was found
                if (parts[i].Equals("("))
                {
                    //This if-statement is used to ensure that no IndexOutOfBounds exception would be catched
                    if (i <= parts.Length - 2)
                    {
                        //If the next element is an operator or ")", throw an exception
                        if (stringExtension.stringExtension.isOperator(parts[i + 1]) || parts[i + 1].Equals(")"))
                        {
                            throw new FormulaFormatException("This is not a correct format formula ( (+ or ())");
                        }
                    }
                    //Else, add this part to the string
                    temp += parts[i];
                    //Increment the open symbol counts
                    LPattern++;
                }
                //If ")" was found
                else if (parts[i].Equals(")"))
                {
                    if (i <= parts.Length - 2)
                    {
                        //If the next element is a double number or a variable or open symbol, then throw an exception
                        if (stringExtension.stringExtension.isFloat(parts[i + 1]) || parts[i + 1].Equals("(") || stringExtension.stringExtension.isVariable(parts[i + 1]))
                        {
                            throw new FormulaFormatException("This is not a correct format formula, ( )( or )x2 or )9)");
                        }
                    }
                    //Else, add this part to the string
                    temp += parts[i];
                    //Increment the close symbol counts
                    RPattern++;
                }
                //If an operator was found
                else if (stringExtension.stringExtension.isOperator(parts[i]))
                {
                    if (i <= parts.Length - 2)
                    {
                        //If the next element is another operator or a close symbol, then throw a exception
                        if (stringExtension.stringExtension.isOperator(parts[i + 1]) || parts[i + 1].Equals(")"))
                        {
                            throw new FormulaFormatException("This is not a correct format formula");
                        }
                    }
                    //Else, add this part to the string
                    temp += parts[i];
                }
                //If a double number was found
                else if (stringExtension.stringExtension.isFloat(parts[i]))
                {
                    if (i <= parts.Length - 2)
                    {
                        //If the next element is another double number or a variabe or an open symbol, then throw an exception
                        if (stringExtension.stringExtension.isFloat(parts[i + 1]) || parts[i + 1].Equals("(") || stringExtension.stringExtension.isVariable(parts[i + 1]))
                        {
                            throw new FormulaFormatException("This is not a correct format formula");
                        }

                    }
                    //ELse, add this part to the string
                    temp += parts[i];
                }
                //If a variable was found
                else if (stringExtension.stringExtension.isVariable(parts[i]))
                {
                    if (i <= parts.Length - 2)
                    {
                        //If the next element is a double number, or another variable or an open symbol, then throw an exception
                        if (stringExtension.stringExtension.isFloat(parts[i + 1]) || parts[i + 1].Equals("(") || stringExtension.stringExtension.isVariable(parts[i + 1]))
                        {
                            throw new FormulaFormatException("This is not a correct format formula");
                        }
                    }
                    //If the normalized variable is valid, then add the normalized variable in the string
                    if (isValid(normalize(parts[i])))
                    {
                        temp += normalize(parts[i]);
                    }
                    //Otherwise, throw an exception
                    else
                    {
                        throw new FormulaFormatException("This is not a valid formula");
                    }
                }
                //If the part does not match any case above, then throw an exception
                else
                {
                    throw new FormulaFormatException("Incorrect variable format");
                }
            }
            //If the count of open symbol and close symbol do not match, then throw an exception
            if (LPattern != RPattern)
            {
                throw new FormulaFormatException("Open and close symbol do not match");
            }
            //Assign the string temp to the global variable
            this.formula = temp;
        }

        /// <summary>
        /// This is the delegate type method
        /// </summary>
        /// <param name="s">string type s</param>
        /// <returns>double value</returns>
        public delegate double Lookup(string s);
        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        /// 
        public object Evaluate(Func<string, double> lookup)
        {
            this.formula = this.formula.Trim();
            //If the expression is empty, then throw an exception
            if (this.formula.Equals(""))
            {
                throw new ArgumentException("Please enter an expression");
            }
            //Create an int type stack and a string type stack
            Stack<double> operandStack = new Stack<double>();
            Stack<string> operatorStack = new Stack<string>();
            //Split the operands and operators in the expression to the string type array
            string[] substrings = GetTokens(this.formula).ToArray();
            //Linear check all elements in the expression by using For-loop
            for (int i = 0; i < substrings.Length; i++)
            {
                String token = substrings[i].Trim();
                //If meet space, then continue to the loop
                if (token.Equals(""))
                {
                    continue;
                }
                //If meet add and minus symbols, then check the opeartor stack to decide whether to process the operands in the stack
                else if (token.Equals("+") || token.Equals("-"))
                {
                    while (operatorStack.Count != 0 && (operatorStack.Peek().Equals("+") || operatorStack.Peek().Equals("-")
                        || operatorStack.Peek().Equals("*") || operatorStack.Peek().Equals("/")))
                    {
                        try
                        {
                            //Process the operands in the stack
                            Formula.operatorProcessor(operandStack, operatorStack);
                        }
                        catch
                        {
                            return new FormulaError("Processing stack failed");
                        }
                    }
                    //Push the token into the operator stack
                    operatorStack.Push(token);
                }
                //If meet multiply and divide symbols, then check the operator stack to decide whether to process the operands in the stack 
                else if (token.Equals("*") || token.Equals("/"))
                {
                    while (operatorStack.Count != 0 && (operatorStack.Peek().Equals("*") || operatorStack.Peek().Equals("/")))
                    {
                        try
                        {
                            //Process the operands in the stack
                            Formula.operatorProcessor(operandStack, operatorStack);
                        }
                        catch
                        {
                            return new FormulaError("Processing stack failed");
                        }
                    }
                    //Push the token into the operate stack
                    operatorStack.Push(token);
                }
                //If meet the "(", then push this token into the operator stack directly
                else if (token.Trim().Equals("("))
                {
                    operatorStack.Push("(");
                }
                //If meet the ")", then we should judge wheter the top of the stack has the "("
                else if (token.Trim().Equals(")"))
                {
                    //Process all expression in the () by using while loop
                    while (!operatorStack.Peek().Equals("("))
                    {
                        try
                        {
                            //Process the operands in the stack
                            Formula.operatorProcessor(operandStack, operatorStack);
                        }
                        catch
                        {
                            return new FormulaError("Processing stack failed");
                        }
                    }
                    //Pop the top element from the operator stack
                    operatorStack.Pop();
                }
                //If we meet a token except the "+"-"*"/"(")", then we should try to parse this token to the integer
                else
                {
                    double variable;
                    //If successful, then push the parsed integer into the operand stack
                    if (Double.TryParse(token, out variable))
                    {
                        operandStack.Push(variable);
                    }
                    else
                    {
                        //Otherwise, try to find the value of this token and push the value into the operand stack
                        try
                        {
                            variable = lookup(token);
                            operandStack.Push(variable);
                        }
                        //If fail, throw an exception
                        catch
                        {
                            return new FormulaError("");
                            // throw new ArgumentException("Can not find the value");
                        }
                    }
                }
            }
            //Handle the remain token by using while-loop
            while (operatorStack.Count != 0)
            {
                //If the operator stack count greater than 0 and operand stack count fewer than two
                //Then, throw the exception
                if (operatorStack.Count > 0 && operandStack.Count < 2)
                {
                    throw new ArgumentException("Incomplete expression");
                }
                try
                {
                    //Process the operands in the stack
                    Formula.operatorProcessor(operandStack, operatorStack);
                }
                catch
                {
                    return new FormulaError("Processing stack failed");
                }
            }
            //If operand stack count equals 1 and operator stack count equals 0
            //Then pop and return the top element from operand stack as result
            if (operandStack.Count == 1 && operatorStack.Count == 0)
            {
                return operandStack.Pop();
            }
            //Otherwise, throw an exception
            else
            {
                throw new ArgumentException("Incomplete expression");
            }
        }

        /// <summary>
        /// Helper method in order to process the operands and operators in these two stacks
        /// </summary>
        /// <param name="operandStack">Integer type stack</param>
        /// <param name="operatorStack">String type stack</param>
        public static void operatorProcessor(Stack<double> operandStack, Stack<string> operatorStack)
        {
            String Os = operatorStack.Pop();
            if (operandStack.Count < 2)
            {
                throw new ArgumentException();
            }
            //Create two integers and pop the operand stack twice
            double Op1 = operandStack.Pop();
            double Op2 = operandStack.Pop();
            //If the token is "+", then Op2 + Op1
            if (Os.Equals("+"))
            {
                //Push the result into the operand stack
                operandStack.Push(Op2 + Op1);
            }
            //If the token is "-", then Op2 - Op1
            else if (Os.Equals("-"))
            {
                //Push the result into the operand stack
                operandStack.Push(Op2 - Op1);
            }
            //If the token is "/", then Op2 / Op1
            else if (Os.Equals("/"))
            {
                //If the denominator is zero, then throw an exception
                if ((Op1 == 0 && Op2 != 0) || (Op1 != 0 && Op2 == 0))
                {
                    throw new ArgumentException("Can not divided by zero");
                }
                //If not, push the result into the operand stack
                else
                {
                    operandStack.Push(Op2 / Op1);
                }
            }
            //If the token is "*", then Op2 * Op1
            else if (Os.Equals("*"))
            {
                //Push the result into the operand stack
                operandStack.Push(Op2 * Op1);
            }
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            //No duplicated variable could exist, so create a new string type HashSet to store non-duplicate variables
            HashSet<string> variables = new HashSet<string>();
            //Using for-each loop to handle all elements in the IEnumerable
            foreach (string s in GetTokens(this.formula))
            {
                //If a variable was found, add it into the HashSet
                if (stringExtension.stringExtension.isVariable(s))
                {
                    variables.Add(s);
                }
            }
            //Return the HashSet
            return variables;
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            //Return the global variable directly
            return this.formula;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens, which are compared as doubles, and variable tokens,
        /// whose normalized forms are compared as strings.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            //If obj is null or obj is not the instance of class Formula or the caller is not the instance of the Formula class, return false
            if (obj == null || !(obj is Formula) || !(this is Formula))
            {
                return false;
            }
            //Cast the obj to the Formula type
            Formula temp = (Formula)obj;
            //Cast the caller this to the Formula type
            Formula tempThis = (Formula)this;
            //Create array to store the elements 
            string[] objArray = GetTokens(temp.ToString()).ToArray();
            string[] thisArray = GetTokens(tempThis.ToString()).ToArray();
            //Creates to double variables used to compare
            double thisDouble = 0.0;
            double objDouble = 0.0;
            //If the length of two array does not equal to each other, then return false
            if (thisArray.Length != objArray.Length)
            {
                return false;
            }
            //Else, using for-loop to handle all elements in these two array
            for (int i = 0; i < objArray.Length; i++)
            {
                //If this is an operator or a open-close symbol, or a variable or a double number
                if (stringExtension.stringExtension.isCombination(thisArray[i]) && stringExtension.stringExtension.isCombination(objArray[i]))
                {
                    //Then try to parse the double in the array and compare to each other
                    if (Double.TryParse(thisArray[i], out thisDouble) && Double.TryParse(objArray[i], out objDouble))
                    {
                        //If two double numbers do not equal, the return false
                        if (thisDouble != objDouble)
                        {
                            return false;
                        }
                    }
                    //If not a double number, then compare each other by using Equals method
                    else if (!objArray[i].Equals(thisArray[i]))
                    {
                        //If does not equals to each other, return false
                        return false;
                    }
                }
            }
            //Otherwise, return true
            return true;
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            //If both f1 and f2 are null, this return true
            if (Object.ReferenceEquals(f1, null) && Object.ReferenceEquals(f2, null))
            {
                return true;
            }
            //If one is null and one is not, return false
            if ((Object.ReferenceEquals(f1, null) && !Object.ReferenceEquals(f2, null)) || (!Object.ReferenceEquals(f2, null) && Object.ReferenceEquals(f2, null)))
            {
                return false;
            }
            //Otherwise, return whether f1 equals to f2
            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            //If both f1 and f2 are null, this return false
            if (Object.ReferenceEquals(f1, null) && Object.ReferenceEquals(f2, null))
            {
                return false;
            }
            //If one is null and one is not, return true
            if ((Object.ReferenceEquals(f1, null) && !Object.ReferenceEquals(f2, null)) || (!Object.ReferenceEquals(f2, null) && Object.ReferenceEquals(f2, null)))
            {
                return true;
            }
            //Otherwise, return whether f1 not equals to f2
            return !f1.Equals(f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            //Return the HashCode of the string
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        public static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }


        /// <summary>
        /// Used to report syntactic errors in the argument to the Formula constructor.
        /// </summary>
        public class FormulaFormatException : Exception
        {
            /// <summary>
            /// Constructs a FormulaFormatException containing the explanatory message.
            /// </summary>
            public FormulaFormatException(String message)
                : base(message)
            {
            }

        }


        /// <summary>
        /// Used as a possible return value of the Formula.Evaluate method.
        /// </summary>
        public struct FormulaError
        {
            /// <summary>
            /// Constructs a FormulaError containing the explanatory reason.
            /// </summary>
            /// <param name="reason"></param>
            public FormulaError(String reason)
                : this()
            {
                Reason = reason;
            }

            /// <summary>
            ///  The reason why this FormulaError was created.
            /// </summary>
            public string Reason { get; private set; }
        }

    }


    /// <summary>
    /// This is the stringExtension 
    /// </summary>
    namespace stringExtension
    {
        /// <summary>
        /// The stringExtension contains a static class which also called stringExtension
        /// The static class contains four static methods
        /// </summary>
        public static class stringExtension
        {
            /// <summary>
            /// This method is used to judge whether a string is an operator
            /// </summary>
            /// <param name="s">The string which is used to be checked</param>
            /// <returns>bool value</returns>
            public static bool isOperator(string s)
            {
                //If s is +, -, * or /, return true
                if (s.Equals("+") || s.Equals("-") || s.Equals("*") || s.Equals("/"))
                {
                    return true;
                }
                //Otherwise, return false
                return false;
            }

            /// <summary>
            /// This method is used to judge whether a string is a double number
            /// </summary>
            /// <param name="s">The string that should be checked</param>
            /// <returns>bool value</returns>
            public static bool isFloat(string s)
            {
                //Initialize
                double d = 0.0;
                //Try to parse the double, and return the result whether is successful
                return Double.TryParse(s, out d);
            }

            /// <summary>
            /// This method is used to judge whether a string is a variable
            /// </summary>
            /// <param name="s">The string that should be checked</param>
            /// <returns>bool value</returns>
            public static bool isVariable(string s)
            {
                //Using regular expression to check whether string s is a variable
                return Regex.IsMatch(s, "^[a-zA-Z]+[0-9]+$");
            }

            /// <summary>
            /// A helper method helps to simplify the function
            /// </summary>
            /// <param name="s">String that should be checked</param>
            /// <returns>bool value</returns>
            public static bool isCombination(string s)
            {
                //If a string is a double number or an operator or a variable or open-close symbol, then return true
                if (isFloat(s) || isOperator(s) || isVariable(s) || s.Equals("(") || s.Equals(")"))
                {
                    return true;
                }
                //Else, return false;
                return false;
            }
        }
    }
}