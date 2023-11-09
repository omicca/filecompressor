using FileCompressor.compress;

var comp = new Compress();

var files = comp.ReadFile();

var file = files[1];

comp.CompressTextFile(file);