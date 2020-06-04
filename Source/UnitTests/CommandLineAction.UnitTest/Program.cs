using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandLineAction.UnitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 5; i++)
            {
               Console.Write($"Console write {i}");
            }

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"Console WriteLine {i}");
            }
        }
    }
}
