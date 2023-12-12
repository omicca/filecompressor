using System.Drawing;
using FileCompressor.compress;
using FileCompressor.Compress;

namespace FileCompressor;

public class Decompress
{
    public void DecompressText(HuffmannTree tree, string path)
    {
        Node? currentNode = tree.Root;
        string binaryString = "", decodedString = "";
        using (BinaryReader reader = new BinaryReader(File.Open(path + "compressed-image.bin", FileMode.Open)))
        {
            binaryString = reader.ReadString();
        }

        Console.WriteLine(tree.Root.Left.Left.Left.Left.Left.IsLeaf);

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
    
    public void DecompressImage(HuffmannTree tree, string path, int height, int width)
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
            else if (currentNode != null && currentNode.IsLeaf)
            {
                decodedString += currentNode.Symbol;
                currentNode = tree.Root;
            }
        }
        ReconstructImage(decodedString, tree, height, width);
    }

    public void ReconstructImage(string decodedString, HuffmannTree tree, int height, int width)
    {
        Node currentNode = tree.Root;
        Bitmap bm = new Bitmap(height, width);

        int pixelIndex = 0;

        foreach (var bit in decodedString)
        {
            currentNode = (bit == 0) ? currentNode.Left : currentNode.Right;

            if (currentNode.IsLeaf)
            {
                int pixelValue = int.Parse(currentNode.Symbol);
                int x = pixelIndex % width;
                int y = pixelIndex / width;
                
                bm.SetPixel(x, y, Color.FromArgb(pixelValue, pixelValue,pixelValue));

                pixelIndex++;

                currentNode = tree.Root;
            }
        }
        bm.Save("reconstructed_image.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
    }
}