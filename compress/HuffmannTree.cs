using System;
using System.Collections;
using System.Collections.Generic;
using FileCompressor.Compress;
using Newtonsoft.Json;

namespace FileCompressor.Compress
{
    public class Node : IComparable<Node>
    {
        public string Symbol { get; set; }
        public int Weight { get; set; }
        public Node? Left { get; set; }
        public Node? Right { get; set; }
        public bool IsLeaf { get; set; }

        public Node(string symbol, int weight)
        {
            Symbol = symbol;
            Weight = weight;
            IsLeaf = true;
        }

        public Node(int weight, Node? left, Node? right)
        {
            Weight = weight;
            Left = left;
            Right = right;
            IsLeaf = false;
        }

        public Node(object nodeKey, int nodeValue)
        {
            throw new NotImplementedException();
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

    public class HuffmannTree
    {
        public Node? Root { get; private set; }

        public HuffmannTree(List<Node> nodes)
        {
            BuildTree(nodes);
        }

        private void BuildTree(List<Node> nodes)
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

        public void TraverseTree(Node? node, string code, Dictionary<string, string> huffmannCodes)
        {
            while (true)
            {
                if (node == null) return;

                if (!string.IsNullOrEmpty(node.Symbol) && node.Symbol != "\0")
                {
                    huffmannCodes[node.Symbol] = code;
                }

                TraverseTree(node.Left, code + "0", huffmannCodes);
                node = node.Right;
                code += "1";
            }
        }
        public string SerializeTree()
        {
            string serializedTree = JsonConvert.SerializeObject(Root, Formatting.Indented);
            return serializedTree;
        }

        public void DeserializeTree(string data)
        { 
            Root = JsonConvert.DeserializeObject<Node>(data);
           
        }
    }
}

public static class HTreePrinter
{
    public static void Print(this Node? root, int spacing = 1, int topMargin = 2, int leftMargin = 2)
    {
        if (root == null) return;
        int rootTop = Console.CursorTop + topMargin;
        var last = new List<NodeInfo>();
        var next = root;
        for (int level = 0; next != null; level++)
        {
            var item = new NodeInfo { Node = next, Text = GetNodeText(next) };
            if (level < last.Count)
            {
                item.StartPos = last[level].EndPos + spacing;
                last[level] = item;
            }
            else
            {
                item.StartPos = leftMargin;
                last.Add(item);
            }
            if (level > 0)
            {
                item.Parent = last[level - 1];
                if (next == item.Parent.Node.Left)
                {
                    item.Parent.Left = item;
                    item.EndPos = Math.Max(item.EndPos, item.Parent.StartPos - 1);
                }
                else
                {
                    item.Parent.Right = item;
                    item.StartPos = Math.Max(item.StartPos, item.Parent.EndPos + 1);
                }
            }
            next = next.Left ?? next.Right;
            for (; next == null; item = item.Parent)
            {
                int top = rootTop + 2 * level;
                Print(item.Text, top, item.StartPos);
                Print("/", top + 1, item.Left.EndPos);
                Print("_", top, item.Left.EndPos + 1, item.StartPos);

                Print("_", top, item.EndPos, item.Right.StartPos - 1);
                Print("\\", top + 1, item.Right.StartPos - 1);
                if (--level < 0) break;
                if (item == item.Parent.Left)
                {
                    item.Parent.StartPos = item.EndPos + 1;
                    next = item.Parent.Node.Right;
                }
                else
                {
                    item.Parent.StartPos += (item.StartPos - 1 - item.Parent.EndPos) / 2;
                }
            }
        }
        Console.SetCursorPosition(0, rootTop + 2 * last.Count - 1);
    }

    private static string GetNodeText(Node node)
    {
        if (node.Left == null && node.Right == null) // Leaf node
        {
            return node.Symbol.ToString(); // Use the symbol for leaf nodes
        }
        else
        {
            return node.Weight.ToString(); // Use the weight for internal nodes
        }
    }

    private static void Print(string s, int top, int left, int right = -1)
    {
        Console.SetCursorPosition(left, top);
        if (right < 0) right = left + s.Length;
        while (Console.CursorLeft < right) Console.Write(s);
    }

    private class NodeInfo
    {
        public Node Node;
        public string Text;
        public int StartPos;
        public int Size { get { return Text.Length; } }
        public int EndPos { get { return StartPos + Size; } set { StartPos = value - Size; } }
        public NodeInfo Parent, Left, Right;
    }
}
