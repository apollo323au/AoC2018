using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Day12
{
    class Program
    {
        static void Main(string[] args)
        {
            var plantGrower = new PlantGrower();

            var result1 = plantGrower.Grow1(Resource.Input, Resource.InputPatterns);
            var result2= plantGrower.Grow2(Resource.Input, Resource.InputPatterns);

            Console.WriteLine($"Result1: {result1}");
            Console.WriteLine($"Result2: {result2}");
            Console.ReadLine();
        }
    }

    public class PlantGrower
    {
        public int Grow1(
            string input,
            string inputPatterns)
        {
            var patterns = inputPatterns.Split(new string[] {Environment.NewLine}, StringSplitOptions.None)
                .Select(p => p.Split(new string[] {" => "}, StringSplitOptions.None))
                .Select(p => new Pattern(p[0], p[1].ToCharArray()[0]))
                .ToArray();
            
            var iteration = new PlantIteration(input, 1, 0);

            for (var i = 1; i <= 20; i++)
            {
                var newState = iteration.GetNewState(patterns);
                iteration = new PlantIteration(newState, i + 1, iteration.ZeroIndex);
            }

            var value = CalculateSum(iteration);

            return value;
        }

        public decimal Grow2(
            string input,
            string inputPatterns)
        {
            var patterns = inputPatterns.Split(new string[] {Environment.NewLine}, StringSplitOptions.None)
                .Select(p => p.Split(new string[] {" => "}, StringSplitOptions.None))
                .Select(p => new Pattern(p[0], p[1].ToCharArray()[0]))
                .ToArray();
            
            var iteration = new PlantIteration(input, 1, 0);
            var lastValue = 0;
            var lastDifference = 0;

            for (var i = 1; i <= 200; i++)
            {
                var newState = iteration.GetNewState(patterns);

                iteration = new PlantIteration(newState, i + 1, iteration.ZeroIndex);
                var newValue = CalculateSum(iteration);
                lastDifference = newValue - lastValue;
                Console.WriteLine($"Iteration: {i} - Sum: {newValue}, Difference: {lastDifference}");
                lastValue = newValue;
            }

            var value = ((50000000000M - 200) * lastDifference) + lastValue;

            return value;
        }

        private static int CalculateSum(
            PlantIteration iteration)
        {
            var value = 0;

            for (var i = 0; i < iteration.CurrentState.Length; i++)
            {
                if (iteration.CurrentState[i] == '#')
                    value += i - iteration.ZeroIndex;
            }

            return value;
        }
    }

    public class Pattern
    {
        public string PatternToMatch { get; }
        public char Outcome { get; }

        public Pattern(
            string patternToMatch, char outcome)
        {
            PatternToMatch = patternToMatch;
            Outcome = outcome;
        }
    }

    public class PlantIteration
    {
        public string CurrentState { get; }
        public int Iteration { get; }
        public int ZeroIndex { get; }

        public PlantIteration(
            string currentState,
            int iteration,
            int zeroIndex)
        {
            CurrentState = currentState;

            var endDots = currentState.ToCharArray().Reverse().TakeWhile(i => i == '.').Count();

            for (var i = 0; i < 5 - endDots; i++)
            {
                CurrentState = CurrentState + ".";
            }

            var startDots = currentState.ToCharArray().TakeWhile(i => i == '.').Count();
            if (startDots > 5)
                startDots = 5;

            for (var i = 0; i < 5 - startDots; i++)
            {
                CurrentState = "." + CurrentState;
            }

            Iteration = iteration;
            ZeroIndex = zeroIndex + ( 5 - startDots);
            var heading = "    ";
            for (var i = 0 - ZeroIndex; i < 0; i++)
            {
                heading = heading + " ";
            }
            heading = heading + "0";
        }

        public string GetNewState(
            Pattern[] patterns)
        {
            var currentState = CurrentState.ToCharArray();
            var newState = new char[currentState.Length];
            currentState.CopyTo(newState, 0);

            for (var i = 0; i + 4 < currentState.Length; i++)
            {
                var currentSegment = new string(GetSegment(currentState, i));

                var matchingPattern = patterns.SingleOrDefault(p => currentSegment == p.PatternToMatch);
                if (matchingPattern == null) continue;

                newState[i + 2] = matchingPattern.Outcome;
            }

            return new string(newState);
        }

        private char[] GetSegment(
            char[] currentState,
            int i)
        {
            return currentState.Skip(i).Take(5).ToArray();
        }
    }
}
