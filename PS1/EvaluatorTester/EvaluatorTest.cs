using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvaluatorTest
{
    /// <summary>
    /// This is a test class which could test the expression and find the result
    /// Can also test the exception that expected
    /// </summary>
    class EvaluatorTest
    {
        /// <summary>
        /// lookUp method which has a string type parameter and return an integer as result
        /// </summary>
        /// <param name="x"></param>
        /// <returns>Integer</returns>
        public static int lookUp(string x)
        {
            return 4;
        }

        public static IEnumerable<int> Scramble(int lo, int hi)
        {
            if (lo <= hi)
            {
                int mid = (lo + hi) / 2;
                yield return mid;
                foreach (int n in Scramble(mid + 1, hi))
                {
                    yield return n;
                }
                foreach (int n in Scramble(lo, mid - 1))
                {
                    yield return n;
                }
            }
        }
        public static int F(ref int x, ref int y)
        {
            x = 2;
            return x + y;
        }
        public static int G()
        {
            int a = 7;
            return F(ref a, ref a);
        }
        public static void swap(ref int n, ref int m)
        {
            int temp = n;
            n = m;
            m = temp;
        }

        public static IEnumerable<int> g()
        {
            for (int i = 0; i < 4; i++)
            {
                yield return i;
                Console.WriteLine(8 - i);
            }
            Console.WriteLine(4);
        }

        public static void f()
        {
            foreach (int n in g())
            {
                Console.WriteLine(n);
                if (n > 1) break;
            }
            //Console.ReadLine();
        }
        public static void Main()
        {
            f();
            //int[] A = { 2, 9 };
            //Console.WriteLine("Before " + A[0] + " " + A[1]);
            //swap(ref A[0], ref A[1]);
            //Console.WriteLine(" After " + A[0] + " " + A[1]);
            //foreach (int n in Scramble(0, 6))
            //{
            //    Console.Write(n);
            //}
            //Delcare variable
            //FormulaEvaluator.Evaluator.LookUp lok = lookUp;
            //string test1 = "5";
            //string test2 = "2 *3";
            //string test3 = "2";
            //string test4 = "A2";
            //string test5 = "A5 + 8";
            //string test6 = "3-2";
            //string test7 = "8/2";
            //string test8 = "3*5 +8";
            //string test9 = "3+5*8";
            //string test10 = "(3+5)*8";
            //string test11 = "3*(8+5)";
            //string test12 = "3+(10+8)";
            //string test13 = "3+(12+3*2)";
            //string test14 = "3+5*2-(3*4+8)*2+2";
            //string test15 = "3 * 2 +";
            //string test16 = "3*(2+3";
            //string test17 = "3+(5+(9+(2+3)))";
            //string test18 = "A5 - A5 +A5 *A5/A5";
            //string test19 = "20/0";
            //string test20 = "-5+3";
            //string test21 = "";

            ////Test1, test expression "5 + 2"
            //try
            //{
            //    int test = FormulaEvaluator.Evaluator.Evaluate(test1, lok);
            //    Console.WriteLine(test + " First test successful");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("First test failed");
            //}

            ////Test2, test expression"2*3"
            //try
            //{
            //    int test = FormulaEvaluator.Evaluator.Evaluate(test2, lok);
            //    Console.WriteLine(test + " Second test successful");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("Second test failed");
            //}

            ////Test3, test expression "2"
            //try
            //{
            //    int test = FormulaEvaluator.Evaluator.Evaluate(test3, lok);
            //    Console.WriteLine(test + " Third test successful");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("Third test failed");
            //}

            ////Test4, test expression "A2" which would be looked up
            //try
            //{
            //    int test = FormulaEvaluator.Evaluator.Evaluate(test4, lok);
            //    Console.WriteLine(test + " Fourth test successful");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("Fourth test failed");
            //}

            ////Test5, test expression "A2 + 8"
            //try
            //{
            //    int test = FormulaEvaluator.Evaluator.Evaluate(test5, lok);
            //    Console.WriteLine(test + " Fifth test successful");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("Fifth test failed");
            //}

            ////Test6, test expression "3-2"
            //try
            //{
            //    int test = FormulaEvaluator.Evaluator.Evaluate(test6, lok);
            //    Console.WriteLine(test + " Sixth test successful");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("Sixth test failed");
            //}

            ////Test7, test expression "8/2"
            //try
            //{
            //    int test = FormulaEvaluator.Evaluator.Evaluate(test7, lok);
            //    Console.WriteLine(test + " Seventh test successful");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("Seventh test failed");
            //}

            ////Test8, test expression "3*5+8"
            //try
            //{
            //    int test = FormulaEvaluator.Evaluator.Evaluate(test8, lok);
            //    Console.WriteLine(test + " Eighth test successful");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("Eighth test failed");
            //}

            ////Test9, test expression "3+5*8"
            //try
            //{
            //    int test = FormulaEvaluator.Evaluator.Evaluate(test9, lok);
            //    Console.WriteLine(test + " Ninth test successful");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("Ninth test failed");
            //}

            ////Test10, test expression "(3+5)*8"
            //try
            //{
            //    int test = FormulaEvaluator.Evaluator.Evaluate(test10, lok);
            //    Console.WriteLine(test + " Tenth test successful");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("Tenth test failed");
            //}

            ////Test11, test expression "3*(8+5)"
            //try
            //{
            //    int test = FormulaEvaluator.Evaluator.Evaluate(test11, lok);
            //    Console.WriteLine(test + " Eleventh test successful");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("ELeventh test failed");
            //}

            ////Test12, test expression "3+(10+8)"
            //try
            //{
            //    int test = FormulaEvaluator.Evaluator.Evaluate(test12, lok);
            //    Console.WriteLine(test + " Twelfth test successful");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("Twelfth test failed");
            //}

            ////Test13, test expression "3+(12+3*2)"
            //try
            //{
            //    int test = FormulaEvaluator.Evaluator.Evaluate(test13, lok);
            //    Console.WriteLine(test + " Thirteenth test successful");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("Thirteenth test failed");
            //}

            ////Test14, test expression "3+5*2-(3*4+8)*2+2"
            //try
            //{
            //    int test = FormulaEvaluator.Evaluator.Evaluate(test14, lok);
            //    Console.WriteLine(test + " Fourteenth test successful");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("Fourteenth test failed");
            //}

            ////Test15, test expression "3 * 2 +" and should catch the expected exception
            //try
            //{
            //    FormulaEvaluator.Evaluator.Evaluate(test15, lok);
            //    Console.WriteLine("Fifteenth test is failed");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("Fifteenth test exception is succesful");
            //}

            ////Test16, test expression "3*(2+3" and should catch the expected exception
            //try
            //{
            //    FormulaEvaluator.Evaluator.Evaluate(test16, lok);
            //    Console.WriteLine("Sixteenth test failed");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("Sixteenth test exception is successful");
            //}

            ////Test17, test expression "3+(5+(9+(2+3)))"
            //try
            //{
            //    int test = FormulaEvaluator.Evaluator.Evaluate(test17, lok);
            //    Console.WriteLine(test + " Seventeenth test successful");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("Seventeenth test failed");
            //}

            ////Test18, test expression "A5 - A5 +A5 *A5/A5"
            //try
            //{
            //    int test = FormulaEvaluator.Evaluator.Evaluate(test18, lok);
            //    Console.WriteLine(test + " Eighteenth test successful");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("Eighteenth test failed");
            //}

            ////Test19, test expression "20/0" and should catch expected exception
            //try
            //{
            //    FormulaEvaluator.Evaluator.Evaluate(test19, lok);
            //    Console.WriteLine("Ninteenth test failed");
            //}
            //catch (ArgumentException e)
            //{
            //    Console.WriteLine("Ninteenth test exception is successful");
            //}

            ////Test20, test the expression "-5+3" and should catch expected exception
            //try
            //{
            //    FormulaEvaluator.Evaluator.Evaluate(test20, lok);
            //    Console.WriteLine("Twentieth test exception is failed");
            //}
            //catch
            //{
            //    Console.WriteLine("Twentieth test exception is successful");
            //}

            ////Test21, test the expression "" and should catch expected exception
            //try
            //{
            //    FormulaEvaluator.Evaluator.Evaluate(test21, lok);
            //    Console.WriteLine("Twenty-first test exception is failed");
            //}
            //catch(ArgumentException e)
            //{
            //    Console.WriteLine("Twenty-first test exception is successful");
            //}
            Console.Read();
        }
    }
}

