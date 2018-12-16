using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Day13
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new TrainRunner();
            var testResult = runner.WhereDoTheTrainsCrash(Resource.Input);
            var result2 = runner.WhereIsTheLastCart(Resource.Input);

            Console.WriteLine($"TestResult : {testResult}");
            Console.WriteLine($"TestResult2: {result2}");
            Console.ReadLine();
        }
    }

    public class Train
    {
        public Train(
            int x,
            int y)
        {
            CurrentX = x;
            CurrentY = y;
            NextIntersectionDirection = IntersectionDirection.Left;
        }
        public int CurrentX { get; set; }
        public int CurrentY { get; set; }
        public IntersectionDirection NextIntersectionDirection { get; set; }

        public char GetTrainChar(
            char[][] currentLayout)
        {
            return currentLayout[CurrentY][CurrentX];
        }
    }

    public enum IntersectionDirection
    {
        Left,
        Straight,
        Right
    }


    public class TrainRunner
    {
        public string WhereDoTheTrainsCrash(
            string input)
        {
            var originalLayout = GenerateLayout(input.Replace('>', '-').Replace('<', '-').Replace('^', '|').Replace('v', '|'));

            var currentLayout = GenerateLayout(input);

            var counter = 0;
            var trains = new List<Train>();
            GetTrains(currentLayout, trains);

            while (counter < 100000000)
            {
                //DrawLayout(currentLayout, counter);
                //Console.ReadLine();
                var newCurrentLayout = CopyCurrentLayout(currentLayout);
                var sortedTrains = trains.OrderBy(t => t.CurrentY).ThenBy(t => t.CurrentX).ToArray();

                foreach (var train in sortedTrains)
                {
                    var crashResult = DetectImpendingCrash(train.CurrentX, train.CurrentY, newCurrentLayout, trains);
                    if (crashResult.HasCrash) return $"{crashResult.X},{crashResult.Y}";

                    MoveTrain(train, newCurrentLayout, originalLayout);
                }

                currentLayout = newCurrentLayout;

                counter++;
            }

            return "No crash!";
        }

        public string WhereIsTheLastCart(
            string input)
        {
            var originalLayout = GenerateLayout(input.Replace('>', '-').Replace('<', '-').Replace('^', '|').Replace('v', '|'));

            var currentLayout = GenerateLayout(input);

            var counter = 0;
            var trains = new List<Train>();
            GetTrains(currentLayout, trains);

            while (trains.Count > 1)
            {
                //DrawLayout(currentLayout, counter);
                //Console.ReadLine();
                var newCurrentLayout = CopyCurrentLayout(currentLayout);
                var sortedTrains = trains.OrderBy(t => t.CurrentY).ThenBy(t => t.CurrentX).ToArray();
                var removedTrains = new List<Train>();
                foreach (var train in sortedTrains)
                {
                    if (removedTrains.Contains(train)) continue;

                    var crashResult = DetectImpendingCrash(train.CurrentX, train.CurrentY, newCurrentLayout, trains);
                    if (crashResult.HasCrash)
                    {
                        removedTrains.AddRange(crashResult.InvolvedTrains);

                        crashResult.InvolvedTrains.ForEach(t => newCurrentLayout[t.CurrentY][t.CurrentX] = originalLayout[t.CurrentY][t.CurrentX]);
                    }
                    else
                    {
                        MoveTrain(train, newCurrentLayout, originalLayout);
                    }
                }

                trains = trains.Except(removedTrains).ToList();
                removedTrains.Clear();
                
                currentLayout = newCurrentLayout;

                counter++;
            }

            return $"{trains.First().CurrentX},{trains.First().CurrentY}";
        }

        private void GetTrains(
            char[][] currentLayout,
            List<Train> trains)
        {
            for (var vert = 0; vert < currentLayout.Length; vert++)
            {
                var row = currentLayout[vert];

                for (var horiz = 0; horiz < row.Length; horiz++)
                {
                    if (IsATrain(row[horiz]) == false) continue;
                    trains.Add(new Train(horiz, vert));
                }
            }
        }

        private void DrawLayout(char[][] currentLayout, int iteration)
        {
            Console.Clear();
            Console.WriteLine($"Iteration: {iteration}");

            foreach (var row in currentLayout)
            {
                Console.WriteLine(new string(row));
            }
        }

        private char[][] CopyCurrentLayout(
            char[][] currentLaout)
        {
            var rows = new List<char[]>();

            foreach (var row in currentLaout)
            {
                var newRow = new char[row.Length];
                row.CopyTo(newRow, 0);

                rows.Add(newRow);
            }

            return rows.ToArray();
        }

        private void MoveTrain(
            Train train,
            char[][] currentLayout,
            char[][] originalLayout)
        {
            var trainChar = train.GetTrainChar(currentLayout);
            var oldX = train.CurrentX;
            var oldY = train.CurrentY;

            switch (trainChar)
            {
                case '>':
                    MoveTrainRight(train, currentLayout);
                    break;
                case '<':
                    MoveTrainLeft(train, currentLayout);
                    break;
                case '^':
                    MoveTrainUp(train, currentLayout);
                    break;
                case 'v':
                    MoveTrainDown(train, currentLayout);
                    break;
            }

            currentLayout[oldY][oldX] = originalLayout[oldY][oldX];
        }

        private static void MoveTrainDown(
            Train train,
            char[][] currentLayout)
        {
            var newX = train.CurrentX;
            var newY = train.CurrentY + 1;

            var next = currentLayout[newY][newX];
            if (next == '/') currentLayout[newY][newX] = '<';
            else if (next == '\\') currentLayout[newY][newX] = '>';
            else if (next == '+')
            {
                switch (train.NextIntersectionDirection)
                {
                    case IntersectionDirection.Left:
                        currentLayout[newY][newX] = '>';
                        train.NextIntersectionDirection = IntersectionDirection.Straight;
                        break;
                    case IntersectionDirection.Straight:
                        currentLayout[newY][newX] = 'v';
                        train.NextIntersectionDirection = IntersectionDirection.Right;
                        break;
                    case IntersectionDirection.Right:
                        currentLayout[newY][newX] = '<';
                        train.NextIntersectionDirection = IntersectionDirection.Left;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                currentLayout[newY][newX] = 'v';
            }

            train.CurrentX = newX;
            train.CurrentY = newY;
        }

        private static void MoveTrainUp(
            Train train,
            char[][] currentLayout)
        {
            var newX = train.CurrentX;
            var newY = train.CurrentY - 1;

            var next = currentLayout[newY][newX];
            if (next == '/') currentLayout[newY][newX] = '>';
            else if (next == '\\') currentLayout[newY][newX] = '<';
            else if (next == '+')
            {
                switch (train.NextIntersectionDirection)
                {
                    case IntersectionDirection.Left:
                        currentLayout[newY][newX] = '<';
                        train.NextIntersectionDirection = IntersectionDirection.Straight;
                        break;
                    case IntersectionDirection.Straight:
                        currentLayout[newY][newX] = '^';
                        train.NextIntersectionDirection = IntersectionDirection.Right;
                        break;
                    case IntersectionDirection.Right:
                        currentLayout[newY][newX] = '>';
                        train.NextIntersectionDirection = IntersectionDirection.Left;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                currentLayout[newY][newX] = '^';
            }

            train.CurrentX = newX;
            train.CurrentY = newY;
        }

        private static void MoveTrainLeft(
            Train train,
            char[][] currentLayout)
        {
            var newX = train.CurrentX - 1;
            var newY = train.CurrentY;

            var next = currentLayout[newY][newX];
            if (next == '/') currentLayout[newY][newX] = 'v';
            else if (next == '\\') currentLayout[newY][newX] = '^';
            else if (next == '+')
            {
                switch (train.NextIntersectionDirection)
                {
                    case IntersectionDirection.Left:
                        currentLayout[newY][newX] = 'v';
                        train.NextIntersectionDirection = IntersectionDirection.Straight;
                        break;
                    case IntersectionDirection.Straight:
                        currentLayout[newY][newX] = '<';
                        train.NextIntersectionDirection = IntersectionDirection.Right;
                        break;
                    case IntersectionDirection.Right:
                        currentLayout[newY][newX] = '^';
                        train.NextIntersectionDirection = IntersectionDirection.Left;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                currentLayout[newY][newX] = '<';
            }

            train.CurrentX = newX;
            train.CurrentY = newY;
        }

        private static void MoveTrainRight(
            Train train,
            char[][] currentLayout)
        {
            var newX = train.CurrentX + 1;
            var newY = train.CurrentY;

            var next = currentLayout[newY][newX];
            if (next == '/') currentLayout[newY][newX] = '^';
            else if (next == '\\') currentLayout[newY][newX] = 'v';
            else if (next == '+')
            {
                switch (train.NextIntersectionDirection)
                {
                    case IntersectionDirection.Left:
                        currentLayout[newY][newX] = '^';
                        train.NextIntersectionDirection = IntersectionDirection.Straight;
                        break;
                    case IntersectionDirection.Straight:
                        currentLayout[newY][newX] = '>';
                        train.NextIntersectionDirection = IntersectionDirection.Right;
                        break;
                    case IntersectionDirection.Right:
                        currentLayout[newY][newX] = 'v';
                        train.NextIntersectionDirection = IntersectionDirection.Left;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                currentLayout[newY][newX] = '>';
            }

            train.CurrentX = train.CurrentX + 1;
            train.CurrentY = train.CurrentY;
        }

        private CrashResult DetectImpendingCrash(
            int trainX,
            int trainY,
            char[][] currentLayout,
            List<Train> trains)
        {
            var train = currentLayout[trainY][trainX];

            if (train == '>')
            {
                var next = currentLayout[trainY][trainX + 1];
                if (IsATrain(next)) return new CrashResult(true, new List<Train>() { GetTrain(trainX, trainY, trains), GetTrain(trainX + 1, trainY, trains) }, trainX + 1, trainY);
            }
            else if (train == '<')
            {
                var next = currentLayout[trainY][trainX - 1];
                if (IsATrain(next)) return new CrashResult(true, new List<Train>() { GetTrain(trainX, trainY, trains), GetTrain(trainX - 1, trainY, trains) }, trainX - 1, trainY);

            }
            else if (train == '^')
            {
                var next = currentLayout[trainY - 1][trainX];
                if (IsATrain(next)) return new CrashResult(true, new List<Train>() { GetTrain(trainX, trainY, trains), GetTrain(trainX, trainY - 1, trains) }, trainX, trainY - 1);

            }
            else if (train == 'v')
            {
                var next = currentLayout[trainY + 1][trainX];
                if (IsATrain(next)) return new CrashResult(true, new List<Train>() { GetTrain(trainX, trainY, trains), GetTrain(trainX, trainY + 1, trains) }, trainX, trainY + 1);
            }

            return new CrashResult(false, new List<Train>());
        }

        private Train GetTrain(
            int x,
            int y,
            List<Train> trains)
        {
            var train = trains.SingleOrDefault(t => t.CurrentX == x && t.CurrentY == y);
            if (train == null) throw new ArgumentException();

            return train;
        }

        private bool IsATrain(
            char c)
        {
            if (c == '>') return true;
            if (c == '<') return true;
            if (c == '^') return true;
            if (c == 'v') return true;

            return false;
        }


        private char[][] GenerateLayout(
            string input)
        {
            return input.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                .Select(l => l.ToCharArray())
                .ToArray();
        }
    }

    public class CrashResult
    {
        public bool HasCrash { get; }
        public int X { get; }
        public int Y { get; }
        public List<Train> InvolvedTrains;

        public CrashResult(
            bool hasCrash,
            IEnumerable<Train> involvedTrains,
            int x = -1,
            int y = -1)
        {
            InvolvedTrains = involvedTrains.ToList();
            HasCrash = hasCrash;
            X = x;
            Y = y;
        }
    }
}
