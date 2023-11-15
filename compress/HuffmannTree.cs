namespace FileCompressor.compress;

public class Node
{
    public char Symbol { get; set; }
    public int Weight { get; set; }

    public class InternalNode : Node
    {
        public InternalNode(Node? parentNode, List<Node?> childNotes)
        {
            ParentNode = parentNode;
            ChildNotes = childNotes;
        }

        public Node? ParentNode { get; private set; }
        public List<Node?> ChildNotes { get; private set; }
    }

    public class LeafNode : Node
    {
        public LeafNode(Node? parentNode)
        {
            ParentNode = parentNode;
        }

        public Node? ParentNode { get; private set; }

    }

    public void PrintLeafNodes(List<LeafNode> leafNodes)
    {
        foreach (var node in leafNodes)
        {
            Console.WriteLine($"Symbol: {node.Symbol} || Weight: {node.Weight}\n");
        }
    }
}