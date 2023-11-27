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
            Node newNode = new Node(node.Key, node.Value)
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

    protected HuffmannTree BuildTree(List<Node> leafNodes)
    {
        HuffmannTree huff = new HuffmannTree(leafNodes);
        return huff;
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
                
                    HuffmannTree finalTree = BuildTree(leafNodes);
                    Console.BufferHeight = 5000;
                    BTreePrinter.Print(finalTree.Root);
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

