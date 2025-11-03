using System.Text.Json;
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
                PropertyNamingPolicy = null
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
                PropertyNamingPolicy = null
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

    public string CreateOutputDirectory()
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var outputPath = Path.Combine(Directory.GetCurrentDirectory(), $"output_{timestamp}");

        Directory.CreateDirectory(outputPath);
        logger.LogInformation("Created output directory: {Path}", outputPath);

        return outputPath;
    }
}