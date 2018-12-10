using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day10
{
    class Program
    {
        static void Main(string[] args)
        {
            var calc = new Day10Calc();
            var result1 = calc.Calc1(Resource.Input);
            var result2 = calc.Calc2(Resource.Input);

            Console.WriteLine($"Result1: {result1}");
            Console.WriteLine($"Result2: {result2}");

            Console.ReadLine();
        }
    }

    public class Day10Calc
    {
        public int Calc1(string input)
        {
            return 0;
        }

        public int Calc2(string input)
        {
            return 0;
        }
    }
}
