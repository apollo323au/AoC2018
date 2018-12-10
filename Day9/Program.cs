using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day9
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game();

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var result1 = game.GetWinningPlayerOfGame(9, 25);
            stopWatch.Stop();
            var result1Time = stopWatch.ElapsedMilliseconds;
            stopWatch.Restart();
            stopWatch.Start();
            var result2 = game.GetWinningPlayerOfGame(10, 1618);
            stopWatch.Stop();
            var result2Time = stopWatch.ElapsedMilliseconds;
            stopWatch.Restart();
            stopWatch.Start();
            var result3 = game.GetWinningPlayerOfGame(13, 7999);
            stopWatch.Stop();
            var result3Time = stopWatch.ElapsedMilliseconds;
            stopWatch.Restart();
            stopWatch.Start();
            var result4 = game.GetWinningPlayerOfGame(17, 1104);
            stopWatch.Stop();
            var result4Time = stopWatch.ElapsedMilliseconds;
            stopWatch.Restart();
            stopWatch.Start();
            var result5 = game.GetWinningPlayerOfGame(21, 6111);
            stopWatch.Stop();
            var result5Time = stopWatch.ElapsedMilliseconds;
            stopWatch.Restart();
            stopWatch.Start();
            var result6 = game.GetWinningPlayerOfGame(30, 5807);
            stopWatch.Stop();
            var result6Time = stopWatch.ElapsedMilliseconds;
            stopWatch.Restart();
            stopWatch.Start();
            var result7 = game.GetWinningPlayerOfGame(464, 71730);
            stopWatch.Stop();
            var result7Time = stopWatch.ElapsedMilliseconds;
            stopWatch.Restart();
            stopWatch.Start();
            var result8 = game.GetWinningPlayerOfGame(464, 71730 * 100);
            stopWatch.Stop();
            var result8Time = stopWatch.ElapsedMilliseconds;

            Console.WriteLine($"Game1: {result1}, Time taken: {result1Time}ms - Should be: 32");
            Console.WriteLine($"Game2: {result2}, Time taken: {result2Time}ms - Should be: 8317");
            Console.WriteLine($"Game3: {result3}, Time taken: {result3Time}ms - Should be: 146373");
            Console.WriteLine($"Game4: {result4}, Time taken: {result4Time}ms - Should be: 2764");
            Console.WriteLine($"Game5: {result5}, Time taken: {result5Time}ms - Should be: 54718");
            Console.WriteLine($"Game6: {result6}, Time taken: {result6Time}ms - Should be: 37305");
            Console.WriteLine($"Game7: {result7}, Time taken: {result7Time}ms");
            Console.WriteLine($"Game8: {result8}, Time taken: {result8Time}ms");

            Console.ReadLine();
        }
    }

    public class Game
    {
        public double GetWinningPlayerOfGame(
            int playerCount,
            int lastMarble)
        {
            var players = GeneratePlayers(playerCount);

            var playedMarbles = new LinkedList<double>(new[] { 0D });

            var currentMarble = playedMarbles.First;

            for (var nextMarble = 1; nextMarble <= lastMarble; nextMarble++)
            {
                if (nextMarble % 23 == 0)
                {
                    var currentPlayer = players[nextMarble % playerCount];
                    currentPlayer.Score += nextMarble;

                    var newCurrent = currentMarble.GetPrevious().GetPrevious().GetPrevious().GetPrevious().GetPrevious().GetPrevious();
                    var toRemove = newCurrent.GetPrevious();
                    currentPlayer.Score += toRemove.Value;

                    playedMarbles.Remove(toRemove);
                    currentMarble = newCurrent;
                }
                else
                {
                    currentMarble = playedMarbles.AddAfter(currentMarble.GetNext(), nextMarble);
                }
            }

            return players.Max(i => i.Value.Score);
        }

        private Dictionary<int, Player> GeneratePlayers(
            int playerCount)
        {
            var players = new List<Player>();

            for (var i = 0; i < playerCount; i++)
            {
                players.Add(new Player() { PlayerNumber = i });
            }

            return players.ToDictionary(i => i.PlayerNumber, i => i);
        }
    }

    public class Player
    {
        public int PlayerNumber { get; set; }
        public double Score { get; set; }
    }

    public static class Extensions
    {
        public static LinkedListNode<T> GetPrevious<T>(
            this LinkedListNode<T> node)
        {
            return node.Previous ?? node.List.Last;
        }

        public static LinkedListNode<T> GetNext<T>(
            this LinkedListNode<T> node)
        {
            return node.Next ?? node.List.First;
        }
    }
}
