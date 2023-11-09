namespace FileCompressor.compress;

public class HuffmannCode
{
    //Count characters in string and return sorted dictionary containing the character and frequency count
    protected IDictionary<char, int> CountCharacters(string text)
    {
        IDictionary<char, int> charCount = new Dictionary<char, int>();
        foreach (var character in text)
        {
            if (charCount.ContainsKey(character))
            {
                charCount[character]++;
            }
            else
            {
                charCount.Add(character, 1);
            }
        }
        var finalCharCount = charCount.OrderBy(entry => entry.Value)
            .ToDictionary(entry => entry.Key, entry => entry.Value);
        
        return finalCharCount;
    }
}

public class Compress : HuffmannCode
{
    public string[] ReadFile()
    {
        string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string filePath = Path.Combine(currentDirectory, @"..\..\..\input\");
        string fullPath = Path.GetFullPath(filePath);

        string[] txtFiles = new string[2];
        try
        {
            var dataFiles = Directory.EnumerateFiles(fullPath, "*.txt", SearchOption.TopDirectoryOnly);

            int i = 0;
            foreach (var files in dataFiles)
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
     
    public void CompressTextFile(string file)
    {
        if (File.Exists(file))
        {
            try
            {
                string[] lines = File.ReadAllLines(file);
                foreach (var line in lines)
                {
                    var charCount = CountCharacters(line);
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

    public void CompressImage()
    {
        
    }
}
