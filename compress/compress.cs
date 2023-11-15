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

    protected List<Node.LeafNode> CreateLeafNodes(IDictionary<char, int> sets)
    {
        List<Node.LeafNode> leafNodes = new List<Node.LeafNode>();
        foreach (var node in sets)
        {
            Node.LeafNode newNode = new Node.LeafNode(null)
            {
                Symbol = node.Key,
                Weight = node.Value
            };
            leafNodes.Add(newNode);
        }

        return leafNodes;
    }
    
    protected void BuildPriorityQueue(List<Node.LeafNode> nodes)
    {
        nodes.Sort();
    }

    protected void BuildTree(List<Node.LeafNode> leafNodes)
    {
        List<Node.InternalNode> interNodes = new List<Node.InternalNode>();
        while (leafNodes.Count > 0)
        {
            if (leafNodes.Count == 1)
            {
                Console.WriteLine($"Last element: {leafNodes[0]}");
            }
            else
            {
                List<Node.LeafNode> removedNodes = leafNodes.GetRange(0, 1);
                leafNodes.RemoveRange(0,1);
                var nodeSum = removedNodes[0].Weight + removedNodes[1].Weight;
                Node.InternalNode newInternalNode = new Node.InternalNode(null, removedNodes.Cast<Node>().ToList());
                newInternalNode.Weight = nodeSum;
                
                interNodes.Add(newInternalNode);
            }
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
                }
                var leafNodes = CreateLeafNodes(charCount);
                
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

