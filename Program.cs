using System.Threading.Channels;
using FileCompressor;

Compress comp = new Compress();

string[] files = comp.ReadFile();

foreach (var file in files)
{
    comp.CompressTextFile(file);
}