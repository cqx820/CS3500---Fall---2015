using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyGraphTest
{
    /// <summary>
    /// This is console application which is used to test the "DependencyGraph" class library
    /// </summary>
    class Tester
    {
        /// <summary>
        /// This is the test case that was created by myself.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            SpreadsheetUtilities.DependencyGraph DG = new SpreadsheetUtilities.DependencyGraph();
            //Test "AddDependency" method
            Console.WriteLine("The size of DG is " + DG.Size);//Find the size of DG
            DG.AddDependency("a", "b");
            DG.AddDependency("a", "c");
            DG.AddDependency("d", "c");
            DG.AddDependency("d", "c");//Duplicate cannot add into DG
            Console.WriteLine("The current size of DG is " + DG.Size);
            Console.Write("The dependents of a are: ");
            foreach (string s in DG.GetDependents("a"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependees of a are: ");
            foreach (string s in DG.GetDependees("a"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependents of b are: ");
            foreach (string s in DG.GetDependents("b"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependees of b are: ");
            foreach (string s in DG.GetDependees("b"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependents of c are: ");
            foreach (string s in DG.GetDependents("c"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependees of c are: ");
            foreach (string s in DG.GetDependees("c"))
            {
                Console.Write(s + " ");
            }

            //Test the "HasDependents" method
            Console.WriteLine("\n");
            Console.WriteLine("Test \"HasDependents\" method:");
            Console.WriteLine("True expected: " + DG.HasDependents("a"));
            Console.WriteLine("False expected: " + DG.HasDependents("b"));
            Console.WriteLine("False expected: " + DG.HasDependents("c"));
            Console.WriteLine("True expected: " + DG.HasDependents("d"));

            //Test the "HasDependees" method
            Console.WriteLine("");
            Console.WriteLine("Test \"HasDependees\" method:");
            Console.WriteLine("False expected: " + DG.HasDependees("a"));
            Console.WriteLine("True expected: " + DG.HasDependees("b"));
            Console.WriteLine("True expected: " + DG.HasDependees("c"));
            Console.WriteLine("False expected: " + DG.HasDependees("d"));


            //Test "RemoveDependency" method
            DG.RemoveDependency("a", "c");
            DG.RemoveDependency("d", "c");
            Console.WriteLine("");
            Console.WriteLine("Test the \"RemoveDependency\" method: ");
            Console.Write("The dependents of a are: ");
            foreach (string s in DG.GetDependents("a"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependees of a are: ");
            foreach (string s in DG.GetDependees("a"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependents of b are: ");
            foreach (string s in DG.GetDependents("b"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependees of b are: ");
            foreach (string s in DG.GetDependees("b"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependents of c are: ");
            foreach (string s in DG.GetDependents("c"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependees of c are: ");
            foreach (string s in DG.GetDependees("c"))
            {
                Console.Write(s + " ");
            }

            //Test the ReplaceDependents method
            DG.AddDependency("a", "c");
            DG.AddDependency("c", "d");
            List<string> list = new List<string>();
            list.Add("e");
            list.Add("f");
            DG.ReplaceDependents("a", list);
            Console.WriteLine("\n");
            Console.WriteLine("Test \"ReplaceDependents\" method:");
            Console.Write("The dependents of a are: ");
            foreach (string s in DG.GetDependents("a"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependents of b are: ");
            foreach (string s in DG.GetDependents("b"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependents of c are: ");
            foreach (string s in DG.GetDependents("c"))
            {
                Console.Write(s + " ");
            }

            //Test the ReplaceDependees method
            List<string> newList = new List<string>();
            newList.Add("g");
            newList.Add("h");
            DG.ReplaceDependees("a", newList);
            Console.WriteLine("\n");
            Console.WriteLine("Test \"ReplaceDependents\" method:");
            Console.Write("The dependents of g are: ");
            foreach (string s in DG.GetDependents("g"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependents of h are: ");
            foreach (string s in DG.GetDependents("h"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependees of a are: ");
            foreach (string s in DG.GetDependees("a"))
            {
                Console.Write(s + " ");
            }

            Console.WriteLine("\n");
            DG.ReplaceDependees("p", newList);
            Console.Write("The dependents of g are: ");
            foreach (string s in DG.GetDependents("g"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependents of h are: ");
            foreach (string s in DG.GetDependents("h"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependees of a are: ");
            foreach (string s in DG.GetDependees("a"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependees of p are: ");
            foreach (string s in DG.GetDependees("p"))
            {
                Console.Write(s + " ");
            }

            //Test large dependency graph
            SpreadsheetUtilities.DependencyGraph dg = new SpreadsheetUtilities.DependencyGraph();
            dg.AddDependency("a", "b");
            dg.AddDependency("a", "c");
            dg.AddDependency("a", "d");
            dg.AddDependency("b", "e");
            dg.AddDependency("b", "f");
            dg.AddDependency("c", "g");
            dg.AddDependency("c", "h");
            dg.AddDependency("c", "e");
            dg.AddDependency("d", "q");
            dg.AddDependency("d", "w");
            dg.AddDependency("d", "o");
            dg.AddDependency("e", "l");
            dg.AddDependency("e", "j");
            Console.WriteLine("\n");
            Console.WriteLine("Test large dependency graph");
            Console.Write("The dependents of a are: ");
            foreach (string s in dg.GetDependents("a"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependees of a are: ");
            foreach (string s in dg.GetDependees("a"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependents of b are: ");
            foreach (string s in dg.GetDependents("b"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependees of b are: ");
            foreach (string s in dg.GetDependees("b"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependents of c are: ");
            foreach (string s in dg.GetDependents("c"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependees of c are: ");
            foreach (string s in dg.GetDependees("c"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependents of d are: ");
            foreach (string s in dg.GetDependents("d"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependees of d are: ");
            foreach (string s in dg.GetDependees("d"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependents of e are: ");
            foreach (string s in dg.GetDependents("e"))
            {
                Console.Write(s + " ");
            }
            Console.WriteLine("");
            Console.Write("The dependees of e are: ");
            foreach (string s in dg.GetDependees("e"))
            {
                Console.Write(s + " ");
            }
            Console.Read();
        }
    }
}
