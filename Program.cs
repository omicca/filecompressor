using FileCompressor.compress;

var comp = new Compress();

var files = comp.ReadFile();

var file = files[0];

comp.CompressTextFile(file);
