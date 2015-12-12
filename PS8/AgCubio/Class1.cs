using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgCubio
{
    public class Class1
    {
        public static void main()
        {
            string s = "(move, " + 20 + ", " + 20 + ")\n";
            string[] p = s.Split('(', ')', ' ', ',');
            Console.Write(p);
            Console.Read();
        }

    }
}
