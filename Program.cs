using FileCompressor;
using FileCompressor.compress;

var comp = new Compress();

var files = comp.ReadFile(0);

var file = files[0];

comp.CompressTextFile(file);
