using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day3
{
    class Program
    {
        static void Main(string[] args)
        {
            var day3 = new Day3Calculator();
            var result = day3.SolveProblem1(Resource.Input);

            Console.WriteLine($"Problem 1: {result}");

            var result2 = day3.SolveProblem2(Resource.Input);
            Console.WriteLine($"Problem 2: {result2}");
            Console.ReadLine();
        }
    }

    public class Day3Calculator
    {
        public int SolveProblem1(
            string input)
        {
            var lines = input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            var problemLines = lines.Select(l => new Problem1Line(l)).ToArray();

            var problemPoints = problemLines.SelectMany(p => p.Points.Select(point => point.Ref));

            var groupedPoints = problemPoints
            .GroupBy(p => p);


            return groupedPoints.Count(p => p.Count() > 1);
        }

        public int SolveProblem2(
            string input)
        {
            var lines = input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            var problemLines = lines.Select(l => new Problem1Line(l)).ToArray();

            var problemPoints = problemLines.SelectMany(p => p.Points.Select(point => point.Ref));

            var groupedPoints = problemPoints
                .GroupBy(p => p);

            var onlyfoundonce = new HashSet<string>(groupedPoints.Where(gp => gp.Count() == 1).Select(k => k.Key));

            var claim = problemLines.SingleOrDefault(pl =>
            {
                foreach (var point in pl.Points)
                {
                    if (onlyfoundonce.Contains(point.Ref) == false)
                        return false;
                }

                return true;
            });

            return claim?.ClaimNumber ?? -1;
        }
    }

    public class Problem1Line
    {
        public Problem1Line(
            string line)
        {
            var chars = line.ToCharArray();

            var claimNumberChars = chars.Skip(1).TakeWhile(c => c != ' ').ToArray();
            ClaimNumber = int.Parse(new string(claimNumberChars));

            var charsWithoutClaimNumber = chars.SkipWhile(c => c != '@').Skip(2).ToArray();

            var xy = charsWithoutClaimNumber.TakeWhile(c => c != ':').ToArray();
            var x = int.Parse(new string(xy.TakeWhile(c => c != ',').ToArray()));
            var y = int.Parse(new string(xy.SkipWhile(c => c != ',').Skip(1).ToArray()));

            var wh = charsWithoutClaimNumber.SkipWhile(c => c != ' ').ToArray();
            var w = int.Parse(new string(wh.TakeWhile(c => c != 'x').ToArray()));
            var h = int.Parse(new string(wh.SkipWhile(c => c != 'x').Skip(1).ToArray()));

            Rectangle = new Rectangle(x, y, w, h);

            GeneratePoints();
        }

        public int ClaimNumber { get; set; }
        public Rectangle Rectangle { get; set; }

        public List<Problem1Point> Points { get; set; }
        public void GeneratePoints()
        {
            Points = new List<Problem1Point>();

            for (var x = Rectangle.X; x < Rectangle.X + Rectangle.Width; x++)
            {
                for (var y = Rectangle.Y; y < Rectangle.Y + Rectangle.Height; y++)
                {
                    Points.Add(new Problem1Point(x,y) );
                }
            }
        }
    }

    public class Problem1Point
    {
        public Problem1Point(int x, int y)
        {
            X = x;
            Y = y;
            Ref = $"<X:{X},Y:{Y}>";
        }
        public int X { get; set; }
        public int Y { get; set; }
        public string Ref {get; set; }
    }
}
