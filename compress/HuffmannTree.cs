using System.Diagnostics;

namespace FileCompressor.compress;

public class Node
{
    public char Symbol { get; set; }
    public int Weight { get; set; }

    public class InternalNode : Node
    {
        public InternalNode(Node? parentNode, List<Node> childNotes)
        {
            ParentNode = parentNode;
            ChildNotes = childNotes;
        }

        private Node? ParentNode { get; set; }
        public List<Node> ChildNotes { get; private set; }

        public void SetParent()
        {
            
        }

        public void SetChildNodes(List<Node> nodes)
        {
            ChildNotes = nodes;
        }

    }

    public class LeafNode : Node, IComparable<LeafNode>
    {
        public LeafNode(Node? parentNode)
        {
            ParentNode = parentNode;
        }

        public Node? ParentNode { get; private set; }

        public int CompareTo(LeafNode? other)
        {
            return this.Weight.CompareTo(other.Weight);
        }
    }

    public void PrintLeafNodes(List<LeafNode> leafNodes)
    {
        foreach (var node in leafNodes)
        {
            Console.WriteLine($"Symbol: {node.Symbol} || Weight: {node.Weight}\n");
        }
    }
}