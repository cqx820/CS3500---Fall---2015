using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormulaEvaluator
{
    /// <summary>
    /// This method is used to calculate string type expression and get the result.
    /// Static class can only exist static method inside.
    /// This calculator could not accept negative number, float point number 
    /// </summary>
    public static class Evaluator
    {
        /// <summary>
        /// This is the delegate type. String type as parameter and return an integer
        /// </summary>
        /// <param name="v">String</param>
        /// <returns>Integer</returns>
        public delegate int LookUp(String v);
        /// <summary>
        /// This is a calculator that could compute string expression and get the result
        /// For example, the expression "(5+3)*2" would return integer 16
        /// Could not compute negative number
        /// With the LookUp type parameter which could calculate "(5A+3)*2", the program would find the value of "5A" and get the result
        /// </summary>
        /// <param name="exp">The expression that should be calculated</param>
        /// <param name="variableEvalutor">Delegate method that tale string as parameter and return integer</param>
        /// <returns>Integer value as result of expression</returns>
        public static int Evaluate(String exp, LookUp variableEvalutor)
        {
            //Trim the expression
            exp = exp.Trim();
            //If the expression is empty, then throw an exception
            if (exp.Equals(""))
            {
                throw new ArgumentException("Please enter an expression");
            }
            //Create an int type stack and a string type stack
            Stack<int> operandStack = new Stack<int>();
            Stack<string> operatorStack = new Stack<string>();
            //Split the operands and operators in the expression to the string type array
            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
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
                        //Process the operands in the stack
                        Evaluator.operatorProcessor(operandStack, operatorStack);
                    }
                    //Push the token into the operator stack
                    operatorStack.Push(token);
                }
                //If meet multiply and divide symbols, then check the operator stack to decide whether to process the operands in the stack 
                else if (token.Equals("*") || token.Equals("/"))
                {
                    while (operatorStack.Count != 0 && (operatorStack.Peek().Equals("*") || operatorStack.Peek().Equals("/")))
                    {
                        //Process the operands in the stack
                        Evaluator.operatorProcessor(operandStack, operatorStack);
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
                    try
                    {
                        //Process all expression in the () by using while loop
                        while (!operatorStack.Peek().Equals("("))
                        {
                            Evaluator.operatorProcessor(operandStack, operatorStack);
                        }
                        //Pop the top element from the operator stack
                        operatorStack.Pop();
                    }
                    catch
                    {
                        throw new ArgumentException();
                    }
                }
                //If we meet a token except the "+"-"*"/"(")", then we should try to parse this token to the integer
                else
                {
                    int variable;
                    //If successful, then push the parsed integer into the operand stack
                    if (Int32.TryParse(token, out variable))
                    {
                        operandStack.Push(variable);
                    }
                    else
                    {
                        //Otherwise, try to find the value of this token and push the value into the operand stack
                        try
                        {
                            variable = variableEvalutor(token);
                            operandStack.Push(variable);
                        }
                        //If fail, throw an exception
                        catch
                        {
                            throw new ArgumentException("Can not find the value");
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
                //Process the operand stack and operator stack
                Evaluator.operatorProcessor(operandStack, operatorStack);
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
        public static void operatorProcessor(Stack<int> operandStack, Stack<string> operatorStack)
        {
            String Os = operatorStack.Pop();
            if (operandStack.Count < 2)
            {
                throw new ArgumentException();
            }
            //Create two integers and pop the operand stack twice
            int Op1 = operandStack.Pop();
            int Op2 = operandStack.Pop();
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
    }
}