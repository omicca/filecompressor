using FileCompressor.compress;
using FileCompressor.Compress;

namespace FileCompressor;

public class Decompress
{
    public void DecompressFile(HuffmannTree tree, ReadOutputFolder fileRead)
    {
        string[] outputs = fileRead(1);

        foreach (var file in outputs)
        {
            byte[] binaryData = File.ReadAllBytes(file);
            string binaryString = string.Join("", binaryData.Select(b => Convert.ToString(b, 2).PadLeft(0, '0')));
            Console.WriteLine($"Binary data: {binaryString}");
        }
    }

}