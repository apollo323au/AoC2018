using System;
using System.Collections.Generic;
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

            var result1 = game.GetWinningPlayerOfGame(9, 25);
            var result2 = game.GetWinningPlayerOfGame(10, 1618);
            var result3 = game.GetWinningPlayerOfGame(13, 7999);
            var result4 = game.GetWinningPlayerOfGame(17, 1104);
            var result5 = game.GetWinningPlayerOfGame(21, 6111);
            var result6 = game.GetWinningPlayerOfGame(30, 5807);
            var result7 = game.GetWinningPlayerOfGame(464, 71730);
            var result8 = game.GetWinningPlayerOfGame(464, 71730 * 100);

            Console.WriteLine($"Game1: {result1} - Should be: 32");
            Console.WriteLine($"Game2: {result2} - Should be: 8317");
            Console.WriteLine($"Game3: {result3} - Should be: 146373");
            Console.WriteLine($"Game4: {result4} - Should be: 2764");
            Console.WriteLine($"Game5: {result5} - Should be: 54718");
            Console.WriteLine($"Game6: {result6} - Should be: 37305");
            Console.WriteLine($"Game7: {result7}");
            Console.WriteLine($"Game8: {result8}");

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
