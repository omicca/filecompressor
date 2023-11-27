using System.Threading.Channels;
using FileCompressor.Compress;

namespace FileCompressor.compress;

public class HuffmannCode
{
    protected IDictionary<char, int> CountCharacters(string text)
    {
        IDictionary<char, int> charCount = new Dictionary<char, int>();
        foreach (var character in text)
        {
            if (charCount.ContainsKey(character))
            {
                charCount[character]++;
            }
            else
            {
                charCount.Add(character, 1);
            }
        }
        var finalCharCount = charCount.OrderBy(entry => entry.Value)
            .ToDictionary(entry => entry.Key, entry => entry.Value);
        
        return finalCharCount;
    }

    protected List<Node> CreateLeafNodes(IDictionary<char, int> charSets)
    {
        List<Node> leafNodes = new List<Node>();
        foreach (var node in charSets)
        {
            Node newNode = new Node(null)
            {
                Symbol = node.Key,
                Weight = node.Value
            };
            leafNodes.Add(newNode);
        }

        return leafNodes;
    }
    
    protected void BuildPriorityQueue(List<Node> nodes)
    {
        nodes.Sort();
    }

    protected void BuildTree(List<Node> leafNodes)
    {
        List<Node> interNodes = new List<Node>();
        while (leafNodes.Count > 1)
        {
            List<Node> removedNodes = leafNodes.GetRange(0, 2);
            leafNodes.RemoveRange(0, 2);
            var nodeSum = removedNodes[0].Weight + removedNodes[1].Weight;

            Node newInternalNode = new Node(null, removedNodes.Cast<Node>().ToList())
            {
                Symbol = (char)4,
                Weight = nodeSum
            };

            interNodes.Add(newInternalNode);
        }
        
        if (leafNodes.Count == 1)
        {
            Node rootNode = new Node(interNodes);
            Console.WriteLine(rootNode.Weight);
        }

        foreach (var node in interNodes)
        {
            Console.WriteLine($"Internal nodes: {node.Symbol} - {node.Weight}");
            node.PrintChildNodes();
        }
    }

}


public class Compress : HuffmannCode
{
    public string[] ReadFile()
    {
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string filePath = Path.Combine(currentDirectory, @"..\..\..\input\");
        string fullPath = Path.GetFullPath(filePath);

        string[] txtFiles = new string[2];
        try
        {
            var dataFiles = Directory.EnumerateFiles(fullPath, "*.txt", SearchOption.TopDirectoryOnly);

            int i = 0;
            foreach (var files in dataFiles)
            {
                txtFiles[i] = files;
                i++;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return txtFiles;
    }
     
    public void CompressTextFile(string file)
    {
        if (File.Exists(file))
        {
            try
            {
                string[] lines = File.ReadAllLines(file);
                IDictionary<char, int> charCount = null;
                foreach (var line in lines)
                {
                    charCount = CountCharacters(line);
                    foreach (var node in charCount)
                    {
                        Console.WriteLine(node);
                    }
                }

                if (charCount != null)
                {
                    var leafNodes = CreateLeafNodes(charCount);
                
                    List<Node> finalTree = BuildTree(leafNodes);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"{e}: Error occured while reading file.");
            }
        }
        else
        {
            Console.WriteLine("File does not exist.");
        }
    }

    public void CompressImage()
    {
        
    }
}

