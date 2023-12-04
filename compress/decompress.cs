using FileCompressor.compress;
using FileCompressor.Compress;

namespace FileCompressor;

public class Decompress
{
    public void DecompressFile(HuffmannTree tree, ReadOutputFolder fileRead, string path)
    {
        string val = "";
            using (BinaryReader reader = new BinaryReader(File.Open(path + "compressed.bin", FileMode.Open)))
            {
                val = reader.ReadString();
            }

            Console.WriteLine(val);
            Console.ReadLine();
        
    }
}