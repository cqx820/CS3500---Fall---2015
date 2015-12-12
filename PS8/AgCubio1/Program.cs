using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgCubio1
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = "(move, " + 20 + ", " + 20 + ")\n";
            string[] p = s.Split('(', ')', ' ', ',');
            Console.Write(p);
            Console.Read();
        }
    }
}
