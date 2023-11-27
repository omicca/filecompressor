using System;
using System.Collections.Generic;

namespace FileCompressor.Compress
{
    public class Node : IComparable<Node>
    {
        public char Symbol { get; set; }
        public int Weight { get; set; }
        public Node? Left { get; set; }
        public Node? Right { get; set; }

        public Node(char symbol, int weight)
        {
            Symbol = symbol;
            Weight = weight;
        }

        public Node(int weight, Node? left, Node? right)
        {
            Weight = weight;
            Left = left;
            Right = right;
        }

        public int CompareTo(Node other)
        {
            return Weight.CompareTo(other.Weight);
        }

        public void PrintNode()
        {
            Console.WriteLine($"Symbol: {Symbol}, Weight: {Weight}");
        }
    }

    public class HuffmanTree
    {
        public Node? Root { get; set; }

        public HuffmanTree(List<Node?> nodes)
        {
            BuildTree(nodes);
        }

        private void BuildTree(List<Node?> nodes)
        {
            while (nodes.Count > 1)
            {
                nodes.Sort();
                var left = nodes[0];
                var right = nodes[1];
                nodes.Remove(left);
                nodes.Remove(right);

                var parent = new Node(left.Weight + right.Weight, left, right);
                nodes.Add(parent);
            }

            Root = nodes[0];
        }
    }
}
