using Microsoft.Extensions.Logging;
using ParadigmFakeData.Models.Customer;

namespace ParadigmFakeData.Services;

public class WorkflowOrchestrator(
    ILogger<WorkflowOrchestrator> logger,
    IFileService fileService,
    ICustomerGenerationService customerGenerationService,
    ICustomerContactGenerationService customerContactGenerationService,
    IParadigmApiService paradigmApiService) : IWorkflowOrchestrator
{
    public async Task RunWorkflowAsync()
    {
        var outputPath = fileService.CreateOutputDirectory();
        logger.LogInformation("Starting workflow in directory: {Path}", outputPath);

        // Step 1: Generate customers
        var customersJsonPath = await GenerateCustomersStep(outputPath);
        
        // Step 2: Post customers to Paradigm
        var updatedCustomersPath = await PostCustomersStep(customersJsonPath, outputPath);
        
        // Step 3: Generate customer contacts
        var contactsJsonPath = await GenerateContactsStep(updatedCustomersPath, outputPath);
        
        // Step 4: Post customer contacts to Paradigm
        await PostContactsStep(contactsJsonPath);
        
        logger.LogInformation("Workflow completed successfully!");
    }

    private async Task<string> GenerateCustomersStep(string outputPath)
    {
        logger.LogInformation("=== STEP 1: GENERATE CUSTOMERS ===");
        
        var customersJsonPath = await customerGenerationService.GenerateCustomersAsync(outputPath);
        
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✓ Customers generated successfully!");
        var displayPath = customersJsonPath.Replace("\\", "/");
        Console.WriteLine($"📄 File: file:///{displayPath}");
        Console.ResetColor();
        Console.WriteLine();
        
        Console.Write("Did you review it and can we post it to Paradigm? (y/n): ");
        var response = Console.ReadLine()?.Trim().ToLower();
        
        if (response != "y")
        {
            logger.LogWarning("User declined to continue. Workflow stopped.");
            throw new OperationCanceledException("User declined to continue");
        }
        
        return customersJsonPath;
    }

    private async Task<string> PostCustomersStep(string customersJsonPath, string outputPath)
    {
        logger.LogInformation("=== STEP 2: POST CUSTOMERS TO PARADIGM ===");
        
        var customers = await fileService.ReadFromJsonAsync<List<BaseCustomer>>(customersJsonPath);
        if (customers == null)
        {
            throw new InvalidOperationException("Failed to read customers");
        }

        var updatedCustomers = await paradigmApiService.BatchCreateCustomersAsync(customers);
        
        var updatedPath = await fileService.SaveToJsonAsync(updatedCustomers, outputPath, "customers_updated.json");
        
        logger.LogInformation("Customers updated with IDs from Paradigm");
        
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✓ Customers posted and updated successfully!");
        var displayPath = updatedPath.Replace("\\", "/");
        Console.WriteLine($"📄 Updated file: file:///{displayPath}");
        Console.ResetColor();
        Console.WriteLine();
        
        Console.Write("Do you want to continue and create customer contacts? (y/n): ");
        var response = Console.ReadLine()?.Trim().ToLower();
        
        if (response != "y")
        {
            logger.LogWarning("User declined to continue. Workflow stopped.");
            throw new OperationCanceledException("User declined to continue");
        }
        
        return updatedPath;
    }

    private async Task<string> GenerateContactsStep(string customersJsonPath, string outputPath)
    {
        logger.LogInformation("=== STEP 3: GENERATE CUSTOMER CONTACTS ===");
        
        var contactsJsonPath = await customerContactGenerationService.GenerateCustomerContactsAsync(customersJsonPath, outputPath);
        
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✓ Customer contacts generated successfully!");
        var displayPath = contactsJsonPath.Replace("\\", "/");
        Console.WriteLine($"📄 File: file:///{displayPath}");
        Console.ResetColor();
        Console.WriteLine();
        
        Console.Write("Do you want to continue and post contacts to Paradigm? (y/n): ");
        var response = Console.ReadLine()?.Trim().ToLower();
        
        if (response != "y")
        {
            logger.LogWarning("User declined to continue. Workflow stopped.");
            throw new OperationCanceledException("User declined to continue");
        }
        
        return contactsJsonPath;
    }

    private async Task PostContactsStep(string contactsJsonPath)
    {
        logger.LogInformation("=== STEP 4: POST CUSTOMER CONTACTS TO PARADIGM ===");
        
        await paradigmApiService.BatchCreateCustomerContactsAsync(contactsJsonPath);
        
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✓ Customer contacts posted successfully!");
        Console.ResetColor();
        Console.WriteLine();
    }

    public async Task DeleteCustomersAsync(string jsonPath)
    {
        logger.LogInformation("=== DELETE CUSTOMERS ===");
        logger.LogInformation("Reading customers from {Path}", jsonPath);
        
        var customers = await fileService.ReadFromJsonAsync<List<BaseCustomer>>(jsonPath);
        if (customers == null || customers.Count == 0)
        {
            logger.LogError("No customers found in {Path}", jsonPath);
            throw new InvalidOperationException("No customers found");
        }

        var customersWithIds = customers.Where(c => !string.IsNullOrEmpty(c.CustomerId)).ToList();
        logger.LogInformation("Found {Count} customers with IDs to delete", customersWithIds.Count);

        var successCount = 0;
        var failCount = 0;

        foreach (var customer in customersWithIds)
        {
            try
            {
                await paradigmApiService.DeleteCustomerAsync(customer.CustomerId!);
                successCount++;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete customer {CustomerId}", customer.CustomerId);
                failCount++;
            }
        }

        logger.LogInformation("Deletion complete: {Success} succeeded, {Failed} failed", successCount, failCount);
        
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✓ Deleted {successCount} customers");
        Console.ResetColor();
        
        if (failCount > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ Failed to delete {failCount} customers");
            Console.ResetColor();
        }
        Console.WriteLine();
    }
}

