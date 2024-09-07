namespace Orleans.Extensions;

public static class FileExtension
{
    public static string ReadFileCurrentUsingDirectory(string fileName)
    {
        var currentDirectoryPath = Directory.GetCurrentDirectory();
        var filePath = Path.Combine(currentDirectoryPath, fileName);
        return ReadFile(filePath);
    }
    public static string ReadFile(string filePath) => File.ReadAllText(filePath);
}