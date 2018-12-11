using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace Day11
{
    class Program
    {
        static void Main(string[] args)
        {

            var calc = new Day11Calc();
            var cell1 = calc.GetPowerLevel(3, 5, 8);
            Console.WriteLine($"Test1: {cell1}. Should be 4.");
            var cell = calc.GetPowerLevel(122, 79, 57);
            Console.WriteLine($"Test1: {cell}. Should be -5.");
            var cell2 = calc.GetPowerLevel(217, 196, 39);
            Console.WriteLine($"Test1: {cell2}. Should be 0.");
            var cell3 = calc.GetPowerLevel(101, 153, 71);
            Console.WriteLine($"Test1: {cell3}. Should be 4.");

            var result = calc.Calc1();
            Console.WriteLine($"Result1: {result}");
            var result2 = calc.Calc2();
            Console.WriteLine($"Result2 : {result2}");
            Console.ReadLine();
        }
    }

    public class Day11Calc
    {
        public string Calc1()
        {
            var cells = GenerateCells();
            var bestSquare = GenerateSquares(cells, 3);

            return $"{bestSquare.X},{bestSquare.Y}";
        }

        public string Calc2()
        {
            var cells = GenerateCells();
            SquareOfCells bestSquare = null;

            var bag = new ConcurrentBag<SquareOfCells>();
            Parallel.For(1, 300, size => bag.Add(GenerateSquares(cells, size)));

            var allSquares = bag.ToList();
            bestSquare = allSquares.OrderByDescending(s => s.Level).FirstOrDefault();
            //for (var size = 1; size <= 300; size++)
            //{
            //    var newBest = GenerateSquares(cells, size);
            //    if (bestSquare == null || bestSquare.Level < newBest.Level)
            //        bestSquare = newBest;
            //}

            return $"{bestSquare.X},{bestSquare.Y},{bestSquare.Size}";
        }

        public int GetPowerLevel(int x, int y, int serialNumber = 1133)
        {
            var rackId = x + 10;
            var startPowerLevel = rackId * y;
            var increasedPowerLevel = startPowerLevel + serialNumber;
            var nextPowerLevel = increasedPowerLevel * rackId;

            var reversedDigits = nextPowerLevel.ToString().ToCharArray().Reverse().ToArray();

            var hundredsDigit = reversedDigits.Length >= 3 ? int.Parse(reversedDigits[2].ToString()) : 0;

            return hundredsDigit - 5;
        }

        private SquareOfCells GenerateSquares(int[,] cells, int size)
        {
            SquareOfCells bestSquare = null;

            for (var x = 1; x + size - 1 <= 300; x++)
            {
                for (var y = 1; y + size - 1 <= 300; y++)
                {
                    var totalLevel = 0;

                    for (var xCoord = x; xCoord < x + size; xCoord++)
                    {
                        for (var yCoord = y; yCoord < y + size; yCoord++)
                        {
                            totalLevel += cells[xCoord-1, yCoord-1];
                        }
                    }
                   
                    if (bestSquare == null || bestSquare.Level < totalLevel)
                        bestSquare = new SquareOfCells(totalLevel, size, x,y);
                }
            }

            return bestSquare;
        }

        private int[,] GenerateCells()
        {
            var cells = new int[300, 300];

            for (var x = 1; x <= 300; x++)
            {
                for (var y = 1; y <= 300; y++)
                {
                    cells[x-1, y-1] = GetPowerLevel(x, y);
                }
            }

            return cells;
        }
    }

    public class SquareOfCells
    {
        public SquareOfCells(int level, int size, int x, int y)
        {
            Level = level;
            Size = size;
            X = x;
            Y = y;
        }
        public int Size { get; }
        public int X { get; }
        public int Y { get; }
        public int Level { get; set; }
    }
}
