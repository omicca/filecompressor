using FileCompressor.compress;
using FileCompressor.Compress;

namespace FileCompressor;

public class Decompress
{
    public void DecompressFile(HuffmannTree tree, string path)
    {
        Node? currentNode = tree.Root;
        string binaryString = "", decodedString = "";
        using (BinaryReader reader = new BinaryReader(File.Open(path + "compressed-image.bin", FileMode.Open)))
        {
            binaryString = reader.ReadString();
        }

        foreach (var bit in binaryString)
        {
            if (bit == '0')
            {
                currentNode = currentNode?.Left;
            }
            else if (bit == '1')
            {
                currentNode = currentNode?.Right;
            }

            if (currentNode != null && currentNode.IsLeaf)
            {
                decodedString += currentNode.Symbol;
                currentNode = tree.Root;
            }
        }

        Console.WriteLine(decodedString);
        
        Console.ReadLine();
        
    }
}