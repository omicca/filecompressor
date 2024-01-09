using FileCompressor.Compress;
using System.Drawing;
using System.Text;

namespace FileCompressor.compress;

public class HuffmannCode
{
    internal Dictionary<string, int> CountCharacters(string text)
    {
        Dictionary<string, int> charCount = new Dictionary<string, int>();
        foreach (var character in text)
        {
            string charKey = character.ToString();
            if (charCount.ContainsKey(charKey))
            {
                charCount[charKey]++;
            }
            else
            {
                charCount.Add(charKey, 1);
            }
        }
        var finalCharCount = charCount.OrderBy(entry => entry.Value)
            .ToDictionary(entry => entry.Key, entry => entry.Value);
        
        return finalCharCount;
    }

    protected internal List<Node> CreateLeafNodes(Dictionary<string, int> charSets)
    {
        List<Node> leafNodes = new List<Node>();
        foreach (var (key, value) in charSets)
        {
            Node newNode = new Node(key, value)
            {
                Symbol = key,
                Weight = value
            };
            leafNodes.Add(newNode);
        }

        return leafNodes;
    }

    protected internal HuffmannTree BuildTree(List<Node> leafNodes)
    {
        HuffmannTree huff = new HuffmannTree(leafNodes);
        return huff;
    }
    
    public Dictionary<string, string> GenerateHuffmannCodes(HuffmannTree tree)
    {
        var huffmannCodes = new Dictionary<string, string>();
        tree.TraverseTree(tree.Root, "", huffmannCodes);
        return huffmannCodes;
    }
}

public class Compress
{
    private readonly HuffmannCode huffmannEncoder = new HuffmannCode();
    
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
                                   file.ToLower().EndsWith(".jpeg") ||
                                   file.ToLower().EndsWith(".bmp"));
                
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
                Dictionary<string, int> charCount = null;
                foreach (var line in lines)
                {
                    charCount = huffmannEncoder.CountCharacters(line);
                }

                if (charCount != null)
                {
                    var leafNodes = huffmannEncoder.CreateLeafNodes(charCount);
                    HuffmannTree finalTree = huffmannEncoder.BuildTree(leafNodes);
                    var codes = huffmannEncoder.GenerateHuffmannCodes(finalTree);

                    string bitString = "";
                    foreach (var line in lines)
                    {
                        foreach (var character in line)
                        {
                            string charKey = character.ToString();
                            if (codes.TryGetValue(charKey, out var code))
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
                    decompress.DecompressText(finalTree, outputPath);
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
                if (finalFrequencies.ContainsKey(entry.Key.ToString()))
                {
                    finalFrequencies[entry.Key.ToString()]++;
                }
                else
                {
                    finalFrequencies[entry.Key.ToString()] = entry.Value;
                }
            }

            var leafNodes = huffmannEncoder.CreateLeafNodes(finalFrequencies);
            var huffTree = huffmannEncoder.BuildTree(leafNodes);
            var codes = huffmannEncoder.GenerateHuffmannCodes(huffTree);
            
            StringBuilder encodedData = new StringBuilder();
            foreach (var pixel in finalFrequencies)
            {
                if (codes.TryGetValue(pixel.Key, out var code))
                {
                    encodedData.Append(code);
                }
                else
                {
                    return;
                }
            }

            string huff = encodedData.ToString();
            Console.WriteLine(huff + "\n\n");

            using BinaryWriter writer = new BinaryWriter(File.Open(outputPath + "compressed-image.bin", FileMode.Create));
            writer.Write(encodedData.ToString());
            writer.Close();
            
            Decompress decompress = new Decompress();
            decompress.DecompressImage(huffTree, outputPath, bm.Height, bm.Width);
        }
    }
}


