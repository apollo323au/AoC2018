using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Day2
{
    class Program
    {
        static void Main(string[] args)
        {
            var day2Calc = new Day2Calculator();
            var result = day2Calc.SolveProblem1(Resources.Input);

            Console.WriteLine($"Result Problem 1: {result}");

            var result2 = day2Calc.SolveProblem2(Resources.Input);

            Console.WriteLine($"Result Problem 2: {result2}");
            Console.ReadLine();
        }
    }

    public class Day2Calculator
    {
        public int SolveProblem1(
            string input)
        {
            var groupedChars = input.Split(new [] { Environment.NewLine }, StringSplitOptions.None).Select(line => line.ToCharArray().GroupBy(c => c).ToDictionary(i => i, i => i.Count()));

            var twoCharacters = groupedChars.Count(line => line.Values.Any(i => i == 2));
            var threeCharacters = groupedChars.Count(line => line.Values.Any(i => i == 3));

            return twoCharacters * threeCharacters;
        }

        public string SolveProblem2(
            string input)
        {
            var lines = input.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

            var charArrayLines = lines.Select(i => i.ToCharArray().ToList()).ToList();

            foreach (var line in charArrayLines)
            {
                var possibleLines = new List<List<char>>(charArrayLines);
                possibleLines.Remove(line);

                foreach (var possibleLine in possibleLines)
                {
                    if (line.Where((t,i) => t == possibleLine[i]).Count() == line.Count - 1)
                    {
                        return new string(line.Where((
                                t,
                                i) => t == possibleLine[i]).ToArray());
                    }
                }
            }

            return "Not Found";
        }
    }
}
