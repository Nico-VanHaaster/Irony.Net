using Irony.Parsing;
using Irony.Portable.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Irony.Portable.ConsoleOutput
{
    class Program
    {
        static void Main(string[] args)
        {

            var grammar = new FullTextSearchGrammar();

            var parser = new Parser(grammar);

            var root = parser.Parse("ALDINGA TRAFFIC");

            //Console.WriteLine("Irony Portable Tester");

            //bool quit = false;

            //while (!quit)
            //{
            //    Console.WriteLine("Enter your term")


            //}


        }
    }
}
