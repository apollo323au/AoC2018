using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Day6
{
    class Program
    {
        static void Main(string[] args)
        {
            var calc = new Day6Calc();
            var result = calc.Calc1(Resource.Input);
            Console.WriteLine($"Result1 : {result}");

            var result2 = calc.Calc2(Resource.Input);
            Console.WriteLine($"Result2: {result2}");
            Console.ReadLine();
        }
    }

    public class Day6Calc
    {
        public int Calc1(
            string input)
        {
            var lines = input.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);
            var points = lines.Select(l =>
                {
                    var split = l.Split(new string[] {", "}, StringSplitOptions.None);
                    return new Point(int.Parse(split[0]), int.Parse(split[1]));
                })
                .Select(i => new PointReference() { Id = Guid.NewGuid(), Point = i})
                .ToArray();

            var validPoints = GetValidPoints(points);

            var allPoints = CalculateNumberOfClosestPoints(points);

            return allPoints.Where(i => validPoints.Select(p => p.Id).Contains(i.Key.Id)).Max(i => i.Value);
        }

        private Dictionary<PointReference,int> CalculateNumberOfClosestPoints(
            PointReference[] points)
        {
            var pointDistances = GetPointsWithDistances(points);

            return points.ToDictionary(i => i, i => pointDistances.Count(pd => pd.Closest == i.Id));
        }

        private List<PointDistance> GetPointsWithDistances(
            PointReference[] points)
        {
            var maxX = points.Max(x => x.Point.X);
            var minX = points.Min(x => x.Point.X);
            var maxY = points.Max(x => x.Point.Y);
            var minY = points.Min(x => x.Point.Y);

            var pointDistances = new List<PointDistance>();

            for (var x = minX; x <= maxX; x++)
            {
                for (var y = minY; y <= maxY; y++)
                {
                    var pd = new PointDistance() {Point = new Point(x, y)};

                    pd.DistancesToPoints = points.ToDictionary(i => i, i => GetDistance(pd.Point, i.Point));
                    pointDistances.Add(pd);
                }
            }

            return pointDistances;
        }

        private int GetDistance(
            Point pointA,
            Point pointB)
        {
            return Math.Abs(pointA.X - pointB.X) + Math.Abs(pointA.Y - pointB.Y);
        }

        private PointReference[] GetValidPoints(
            PointReference[] points)
        {
            var maxX = points.Max(x => x.Point.X);
            var minX = points.Min(x => x.Point.X);
            var maxY = points.Max(x => x.Point.Y);
            var minY = points.Min(x => x.Point.Y);

            var infinitePoints = points.Where(p => p.Point.X == maxX || p.Point.X == minX || p.Point.Y == minY || p.Point.Y == maxY).ToArray();
            
            return points.Except(infinitePoints).ToArray();
        }

        public int Calc2(
            string input)
        {
            var lines = input.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);
            var points = lines.Select(l =>
                {
                    var split = l.Split(new string[] {", "}, StringSplitOptions.None);
                    return new Point(int.Parse(split[0]), int.Parse(split[1]));
                })
                .Select(i => new PointReference() { Id = Guid.NewGuid(), Point = i})
                .ToArray();

            var pointDistances = GetPointsWithDistances(points);

            var validPoints = pointDistances.Where(pd => pd.DistancesToPoints.Sum(dp => dp.Value) < 10000).ToArray();

            return validPoints.Length;
        }
    }

    public class PointDistance
    {
        public PointDistance()
        {
            DistancesToPoints = new Dictionary<PointReference, int>();
        }
        public Point Point { get; set; }
        public Dictionary<PointReference, int> DistancesToPoints {get; set; }
        private Guid? _closest;
        public Guid Closest
        {
            get
            {
                if (_closest.HasValue == false)
                {

                    var closestPoints = DistancesToPoints.Where(i => i.Value == DistancesToPoints.Min(d => d.Value))
                        .ToArray();
                    _closest = closestPoints.Length > 1 ? Guid.Empty : closestPoints.First().Key.Id;
                }

                return _closest.Value;
            }
        }

        private int? _distance;

        public int Distance
        {
            get
            {
                if (_distance.HasValue == false)
                {
                    var closestPoints = DistancesToPoints.Where(i => i.Value == DistancesToPoints.Min(d => d.Value))
                        .ToArray();
                    _distance = closestPoints.First().Value;
                }

                return _distance.Value;
            }
        }


    }

    public class PointReference
    {
        public Point Point { get; set; }
        public Guid Id { get;set; }
    }
}
