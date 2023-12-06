using FileCompressor;
using FileCompressor.compress;

var comp = new Compress();

var files = comp.ReadFile(0);

foreach (var filez in files)
{
    Console.WriteLine(filez);
}

var file = files[2];

comp.CompressImage(file);
