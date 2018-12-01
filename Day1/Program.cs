using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day1
{
    class Program
    {
        static void Main(string[] args)
        {
            var day1Calculator = new Day1Calculator();
            var result = day1Calculator.Calculate(Resources.Input);
            Console.WriteLine($"Part 1 Result is: {result}");
            var result2 = day1Calculator.CalculatePart2(Resources.Input);
            Console.WriteLine($"Part 2 Result is: {result2}");
            Console.ReadLine();
        }
    }

    public class Day1Calculator
    {
        public int Calculate(
            string input)
        {
            return GetIntegers(input).Sum();
        }

        private static IEnumerable<int> GetIntegers(
            string input)
        {
            var split = input.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);

            var integers = split.Select(int.Parse);
            return integers;
        }

        public int CalculatePart2(
            string input)
        {
            var inputNumbers = GetIntegers(input).ToArray();

            var foundFrequencies = new HashSet<int>();
            var currentFrequency = 0;

            for (var index = 0; index <= inputNumbers.Count(); index++)
            {
                if (index == inputNumbers.Length)
                    index = 0;

                currentFrequency += inputNumbers[index];

                if (foundFrequencies.Contains(currentFrequency))
                    return currentFrequency;

                foundFrequencies.Add(currentFrequency);
            }

            return 0;
        }
    }
}
