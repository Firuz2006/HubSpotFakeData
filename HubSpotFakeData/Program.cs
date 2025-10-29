using HubSpotFakeData.Models;
using HubSpotFakeData.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HubSpotFakeData;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var serviceProvider = ConfigureServices();

        try
        {
            var logger = serviceProvider.GetRequiredService<ILogger<DataGenerationService>>();
            var mode = ParseGenerationMode(args);

            logger.LogInformation("HubSpot Fake Data Generator");
            logger.LogInformation("Mode: {Mode}", mode);
            logger.LogInformation("Started at: {Time}", DateTime.Now);

            var dataGenerator = serviceProvider.GetRequiredService<IDataGenerationService>();
            var csvExporter = serviceProvider.GetRequiredService<ICsvExportService>();

            logger.LogInformation("Generating data...");
            var result = dataGenerator.Generate(mode);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            logger.LogInformation("Exporting to CSV...");
            var companiesPath = await csvExporter.ExportCompaniesToCsvAsync(result.Companies, timestamp);
            var contactsPath = await csvExporter.ExportContactsToCsvAsync(result.Contacts, timestamp);

            logger.LogInformation("Completed successfully!");
            logger.LogInformation("Companies file: {FilePath}", Path.GetFullPath(companiesPath));
            logger.LogInformation("Contacts file: {FilePath}", Path.GetFullPath(contactsPath));
            logger.LogInformation("Total company rows: {Count}", result.Companies.Count);
            logger.LogInformation("Total contact rows: {Count}", result.Contacts.Count);
        }
        catch (Exception ex)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<DataGenerationService>>();
            logger.LogError(ex, "An error occurred during execution");
            throw;
        }
    }

    private static ServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        services.AddTransient<IDataGenerationService, DataGenerationService>();
        services.AddTransient<ICsvExportService, CsvExportService>();

        return services.BuildServiceProvider();
    }

    private static GenerationMode ParseGenerationMode(string[] args)
    {
        if (args.Length == 0)
        {
            return GenerationMode.Test;
        }

        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "--mode" && i + 1 < args.Length)
            {
                var modeString = args[i + 1];
                if (Enum.TryParse<GenerationMode>(modeString, true, out var mode))
                {
                    return mode;
                }

                throw new ArgumentException($"Invalid mode: {modeString}. Valid modes are: Test, Production");
            }
        }

        return GenerationMode.Test;
    }
}