namespace ParadigmFakeData.Services;

public interface IFileService
{
    Task<string> SaveToJsonAsync<T>(T data, string outputPath, string fileName);
    Task<T?> ReadFromJsonAsync<T>(string filePath);
    Task<string> SaveTextAsync(string content, string outputPath, string fileName);
}