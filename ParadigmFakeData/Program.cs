using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ParadigmFakeData.Models;
using ParadigmFakeData.Services;

namespace ParadigmFakeData;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        
        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        
        try
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════╗");
            Console.WriteLine("║   Paradigm Fake Data Generator         ║");
            Console.WriteLine("╚════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();

            if (args.Length > 0 && args[0].Equals("--delete", StringComparison.OrdinalIgnoreCase))
            {
                await HandleDeleteCommand(serviceProvider, args);
            }
            else
            {
                await RunMainWorkflow(serviceProvider);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Workflow cancelled by user");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("⚠ Workflow cancelled");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Application error occurred");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ Error: {ex.Message}");
            Console.ResetColor();
            Environment.Exit(1);
        }
    }

    private static async Task RunMainWorkflow(ServiceProvider serviceProvider)
    {
        var orchestrator = serviceProvider.GetRequiredService<IWorkflowOrchestrator>();
        await orchestrator.RunWorkflowAsync();
    }

    private static async Task HandleDeleteCommand(ServiceProvider serviceProvider, string[] args)
    {
        if (args.Length < 2)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Usage: ParadigmFakeData --delete <path-to-customers-json>");
            Console.ResetColor();
            return;
        }

        var jsonPath = args[1];
        if (!File.Exists(jsonPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ File not found: {jsonPath}");
            Console.ResetColor();
            return;
        }

        var orchestrator = serviceProvider.GetRequiredService<IWorkflowOrchestrator>();
        await orchestrator.DeleteCustomersAsync(jsonPath);
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        services.AddSingleton<IConfiguration>(_ => configuration);

        var generationSettings = new GenerationSettings();
        configuration.GetSection("GenerationSettings").Bind(generationSettings);
        services.AddSingleton(generationSettings);

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        services.AddHttpClient<IParadigmApiService, ParadigmApiService>();
        
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<ICustomerGenerationService, CustomerGenerationService>();
        services.AddSingleton<ICustomerContactGenerationService, CustomerContactGenerationService>();
        services.AddSingleton<IWorkflowOrchestrator, WorkflowOrchestrator>();
    }
}