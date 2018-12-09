using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Day5
{
    class Program
    {
        static void Main(string[] args)
        {
            var calc = new Day5Calc();
            var result = calc.Calc1(Resource.Input);
            Console.WriteLine($"Result 1 : {result}");
            var result2 = calc.Calc2(Resource.Input);
            Console.WriteLine($"Result2 : {result2}");
            Console.ReadLine();
        } 
    }

    public class Day5Calc
    {
        char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public int Calc1(
            string input)
        {
            var combinationsToRemove = alpha.Select(i => new string(new[] { i, char.ToLower(i) })).ToList();
            combinationsToRemove.AddRange(alpha.Select(i => new string(new[] { char.ToLower(i), i })));

            var changesMade = false;
            do
            {
                changesMade = false;
                foreach (var combination in combinationsToRemove)
                {
                    var currentLength = input.Length;

                    input = input.Replace(combination, "");

                    changesMade = changesMade || currentLength != input.Length;
                }
            } while (changesMade);

            return input.Length;
        }

        public int Calc2(
            string input)
        {
            var things = alpha.ToDictionary(i => i,
                i => input.Replace(i.ToString(), "").Replace(char.ToLower(i).ToString(), ""));

            return things.Select(i => Calc1(i.Value)).Min();
        }
    }
}
