using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Channels;
using FileCompressor.Compress;
using System.Drawing;

namespace FileCompressor.compress;

public class HuffmannCode
{
    protected Dictionary<char, int> CountCharacters(string text)
    {
        Dictionary<char, int> charCount = new Dictionary<char, int>();
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

    protected List<Node> CreateLeafNodes(Dictionary<char, int> charSets)
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

    protected HuffmannTree BuildTree(List<Node> leafNodes)
    {
        HuffmannTree huff = new HuffmannTree(leafNodes);
        return huff;
    }
    
    public Dictionary<char, string> GenerateHuffmannCodes(HuffmannTree tree)
    {
        var huffmannCodes = new Dictionary<char, string>();
        tree.TraverseTree(tree.Root, "", huffmannCodes);
        return huffmannCodes;
    }
}

public delegate string[] ReadOutputFolder(int choice);
public delegate byte[] StringToByte(string str);

public class Compress : HuffmannCode
{
    static string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
    string inputPath = Path.Combine(currentDirectory, @"..\..\..\input\");
    string outputPath = Path.Combine(currentDirectory, @"..\..\..\output\");
    
    public string[] ReadFile(int choice)
    {
        string[] test = new string[3];
        if (choice == 0)
        {
            string fullPath = Path.GetFullPath(inputPath);

            string[] txtFiles = new string[3];
            try
            {
                var textFiles = Directory.EnumerateFiles(fullPath, "*.txt", SearchOption.TopDirectoryOnly);
                var imgFiles = Directory.EnumerateFiles(fullPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(file => file.ToLower().EndsWith(".jpg") ||
                                   file.ToLower().EndsWith(".png") ||
                                   file.ToLower().EndsWith(".jpeg"));
                
                int i = 0;
                foreach (var files in textFiles)
                {
                    txtFiles[i] = files;
                    i++;
                }
                foreach (var files in imgFiles)
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

        if (choice == 1)
        {
            string fullPath = Path.GetFullPath(outputPath);
            string[] binFiles = new string[2];
            try
            {
                var dataFiles = Directory.EnumerateFiles(fullPath, "*.bin", SearchOption.TopDirectoryOnly);

                int i = 0;
                foreach (var files in dataFiles)
                {
                    binFiles[i] = files;
                    i++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return binFiles;
        }

        return test;
    }
     
    public void CompressText(string file)
    {
        if (File.Exists(file))
        {
            try
            {
                string[] lines = File.ReadAllLines(file);
                Dictionary<char, int> charCount = null;
                foreach (var line in lines)
                {
                    charCount = CountCharacters(line);
                }

                if (charCount != null)
                {
                    var leafNodes = CreateLeafNodes(charCount);
                    HuffmannTree finalTree = BuildTree(leafNodes);
                    var codes = GenerateHuffmannCodes(finalTree);

                    string bitString = "";
                    foreach (var line in lines)
                    {
                        foreach (var character in line)
                        {
                            if (codes.TryGetValue(character, out var code))
                            {
                                bitString += code;
                            }
                        }
                    }

                    Console.WriteLine(bitString);

                    using (BinaryWriter writer = new BinaryWriter(File.Open(outputPath + "compressed.bin", FileMode.Create)))
                    {
                        writer.Write(bitString);
                    }

                    Decompress decompress = new Decompress();
                    decompress.DecompressFile(finalTree, ReadFile, outputPath);
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

    public void CompressImage(string file)
    {
        if (File.Exists(file))
        {
            Bitmap bm = new Bitmap(file);

            Dictionary<int, int> pixelFrequencies = new Dictionary<int, int>();

            for (int i = 0; i < bm.Height; i++)
            {
                for (int j = 0; j < bm.Width; j++)
                {
                    if (j >= 0 && j < bm.Width && i >= 0 && i < bm.Height)
                    {
                        Color pixelColor = bm.GetPixel(j, i);
                        int pixelValue = pixelColor.R;
                        
                        if (pixelFrequencies.ContainsKey(pixelValue))
                        {
                            pixelFrequencies[pixelValue]++;
                        }
                        else
                        {
                            pixelFrequencies[pixelValue] = 1;
                        }
                    }
                }
            }

            Dictionary<string, int> finalFrequencies = new Dictionary<string, int>();
            foreach (var entry in pixelFrequencies)
            {
                string pxl = entry.Key.ToString();
                finalFrequencies[pxl] = entry.Value;
            }

            var leafNodes = CreateLeafNodes(finalFrequencies);
            var huffTree = BuildTree(leafNodes);
            var codes = GenerateHuffmannCodes(huffTree);
            

        }
    }
}


