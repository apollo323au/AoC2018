using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day8
{
    class Program
    {
        static void Main(string[] args)
        {
            var calc = new Day8Calc();
            var result1 = calc.Calc1(Resource.Input);
            var result2 = calc.Calc2(Resource.Input);

            Console.WriteLine($"Result1: {result1}");
            Console.WriteLine($"Result2: {result2}");

            Console.ReadLine();
        }
    }

    public class Day8Calc
    {
        public int Calc1(
            string input)
        {
            var elements = input.Split(' ').Select(int.Parse).ToList();

            var metadataElements = new List<int>();

            GetNextNode(elements, metadataElements);

            return metadataElements.Sum();
        }

        private Node GetNextNode(
            List<int> elements,
            List<int> metadataElements)
        {
            var nodeDefinition = elements.Take(2).ToArray();
            elements.RemoveRange(0, 2);
            var node = new Node { NumberOfChildNodes = nodeDefinition[0], NumberOfMetadataElements = nodeDefinition[1] };

            for (var nodeIndex = 0; nodeIndex < node.NumberOfChildNodes; nodeIndex++)
            {
                node.ChildNodes.Add(GetNextNode(elements, metadataElements));
            }

            if (node.NumberOfMetadataElements != 0)
            {
                var metaDataItems = elements.Take(node.NumberOfMetadataElements).ToArray();
                elements.RemoveRange(0, node.NumberOfMetadataElements);

                metadataElements.AddRange(metaDataItems);
                node.MetadataElements.AddRange(metaDataItems);
            }

            return node;
        }

        public int Calc2(
            string input)
        {
            var elements = input.Split(' ').Select(int.Parse).ToList();

            var metadataElements = new List<int>();

            var node = GetNextNode(elements, metadataElements);

            return node.Value;
        }
    }

    public class Node
    {
        public int NumberOfChildNodes { get; set; }
        public int NumberOfMetadataElements { get; set; }

        public List<int> MetadataElements { get; set; } = new List<int>();
        public List<Node> ChildNodes { get; set; } = new List<Node>();

        public int Value
        {
            get
            {
                if (NumberOfChildNodes == 0) return MetadataElements.Sum();

                return MetadataElements.Where(i => i != 0 && i <= ChildNodes.Count)
                    .ToArray()
                    .Select(i => ChildNodes[i-1].Value)
                    .Sum();
            }
        }
    }
}
