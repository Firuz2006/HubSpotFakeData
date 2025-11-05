using System.CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ParadigmFakeData.Models;
using ParadigmFakeData.Services;

namespace ParadigmFakeData;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);

        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            PrintBanner();
            var orchestrator = serviceProvider.GetRequiredService<IWorkflowOrchestrator>();
            var rootCommand = CliCommandsExtensions.RegisterCommands(orchestrator);

            return await rootCommand.InvokeAsync(args);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Workflow cancelled by user");
            PrintColored("⚠ Workflow cancelled", ConsoleColor.Yellow);
            return 130;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Application error occurred");
            PrintColored($"✗ Error: {ex.Message}", ConsoleColor.Red);
            return 1;
        }
    }

    private static void PrintBanner()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║   Paradigm Fake Data Generator         ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void PrintColored(string message, ConsoleColor color)
    {
        Console.WriteLine();
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        var configuration = new ConfigurationBuilder().AddUserSecrets<Program>()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        services.AddSingleton<IConfiguration>(_ => configuration);

        var generationSettings = new GenerationSettings();
        configuration.GetSection("GenerationSettings").Bind(generationSettings);

        var databaseSettings = new DatabaseSettings();
        configuration.GetSection("DatabaseSettings").Bind(generationSettings);
        services.AddSingleton(generationSettings);

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        services.AddHttpClient<IParadigmApiService, ParadigmApiService>(client =>
        {
            client.DefaultRequestHeaders.Add("x-api-key", generationSettings.ApiKey);
        });

        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<ICustomerGenerationService, CustomerGenerationService>();
        services.AddSingleton<ICustomerContactGenerationService, CustomerContactGenerationService>();
        services.AddSingleton<IOpportunityGenerationService, OpportunityGenerationService>();
        services.AddSingleton<IWorkflowOrchestrator, WorkflowOrchestrator>();
    }
}