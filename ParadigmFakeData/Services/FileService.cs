using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace ParadigmFakeData.Services;

public class FileService(ILogger<FileService> logger) : IFileService
{
    public async Task<string> SaveToJsonAsync<T>(T data, string outputPath, string fileName)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = null,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            };

            var json = JsonSerializer.Serialize(data, options);
            var fullPath = Path.Combine(outputPath, fileName);

            await File.WriteAllTextAsync(fullPath, json);
            logger.LogInformation("Saved JSON to: {Path}", fullPath);

            return fullPath;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save JSON to {Path}", outputPath);
            throw;
        }
    }

    public async Task<T?> ReadFromJsonAsync<T>(string filePath)
    {
        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                IncludeFields = true
            };

            var data = JsonSerializer.Deserialize<T>(json, options);
            logger.LogInformation("Read JSON from: {Path}", filePath);

            return data;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to read JSON from {Path}", filePath);
            throw;
        }
    }

    public async Task<string> SaveTextAsync(string content, string outputPath, string fileName)
    {
        try
        {
            var fullPath = Path.Combine(outputPath, fileName);
            await File.WriteAllTextAsync(fullPath, content);
            logger.LogInformation("Saved text to: {Path}", fullPath);
            return fullPath;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save text to {Path}", outputPath);
            throw;
        }
    }
}