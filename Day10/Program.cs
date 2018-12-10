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
            var leftBoundary = -300;
            var topBoundary = 300;
            var rightBoundary = 300;
            var bottomBoundary = -300;

            var lines = input.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);

            var definitions = lines.Select(ParseLine).ToList();
            var currentStep = 0;

            while (definitions.Any(d =>
                d.IsWithinArea(leftBoundary, topBoundary, rightBoundary, bottomBoundary) == false))
            {
                definitions.ForEach(d => d.MovePointBySteps(1));
                currentStep++;
            }
            
            DrawGrid(GeneratePointArray(definitions));

            Console.WriteLine("?");
            var consoleInput = Console.ReadLine();

            while (consoleInput != ":")
            {
                try
                {
                    var steps = int.Parse(consoleInput);
                    definitions.ForEach(d => d.MovePointBySteps(steps));
                    currentStep += steps;

                    Console.Clear();
                    
                    DrawGrid(GeneratePointArray(definitions));
                }
                catch 
                {
                }
                
                Console.WriteLine("?");
                consoleInput = Console.ReadLine();
            }

            return 0;
        }

        private char[][] GeneratePointArray(
            List<StarDefinition> definitions)
        {
            var pointArray = GenerateGrid(600, 600);

            foreach (var def in definitions)
            {
                pointArray[def.CurrentY + 300][def.CurrentX + 300] = 'X';
            }

            return pointArray;
        }

        private void DrawGrid(
            char[][] grid)
        {
            grid.Select(c => new string(c)).ToList().ForEach(Console.WriteLine);
        }

        private char[][] GenerateGrid(
            int width,
            int height
            )
        {
            var grid = new List<char[]>();

            for (var i = 0; i <= height; i++)
            {
                var line = new List<char>();

                for (var j = 0; j <= width; j++)
                {
                    line.Add('.');
                }

                grid.Add(line.ToArray());
            }

            return grid.ToArray();
        }
        public int Calc2(string input)
        {
            return 0;
        }

        private StarDefinition ParseLine(
            string line)
        {
            var removedElements = line.Replace("position=<", "").Replace("> velocity=<", ",").Replace(">", "");

            var split = removedElements.Split(',');
                
                var splitInt = split.Select(i => int.Parse(i)).ToArray();

            return new StarDefinition(splitInt[0], splitInt[1], splitInt[2], splitInt[3]);
        }
    }

    public class StarDefinition
    {
        public StarDefinition(
            int originalX,
            int originalY,
            int moveX,
            int moveY)
        {
            OriginalX = originalX;
            OriginalY = originalY;
            MoveX = moveX;
            MoveY = moveY;

            CurrentX = originalX;
            CurrentY = originalY;
        }

        public int OriginalX { get;set; }
        public int OriginalY { get; set; }

        public int CurrentX { get;set; }
        public int CurrentY { get; set; }

        public int MoveX { get;set; }
        public int MoveY { get;set; }

        public void MovePointToStep(
            int step)
        {
            CurrentX = OriginalX + (MoveX * step);
            CurrentY = OriginalY + (MoveY * step);
        }

        public void MovePointBySteps(
            int step)
        {
            CurrentX += MoveX * step;
            CurrentY += MoveY * step;
        }

        public bool IsWithinArea(
            int left,
            int top,
            int right,
            int bottom)
        {
            if (CurrentX < left) return false;
            if (CurrentX > right) return false;
            if ( CurrentY < bottom) return false;
            if (CurrentY > top) return false;

            return true;
        }
    }

}
