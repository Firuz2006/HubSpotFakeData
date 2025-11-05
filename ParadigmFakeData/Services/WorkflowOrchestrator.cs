using Microsoft.Extensions.Logging;
using ParadigmFakeData.Models;

namespace ParadigmFakeData.Services;

public class WorkflowOrchestrator(
    ILogger<WorkflowOrchestrator> logger,
    IFileService fileService,
    ICustomerGenerationService customerGenerationService,
    ICustomerContactGenerationService customerContactGenerationService,
    IOpportunityGenerationService opportunityGenerationService,
    IParadigmApiService paradigmApiService) : IWorkflowOrchestrator
{
    public async Task<string> GenerateCustomersAsync(string outputPath)
    {
        logger.LogInformation("=== GENERATE CUSTOMERS ===");

        var customersJsonPath = await customerGenerationService.GenerateCustomersAsync(outputPath);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✓ Customers generated successfully!");
        var displayPath = customersJsonPath.Replace("\\", "/");
        Console.WriteLine($"📄 File: file:///{displayPath}");
        Console.ResetColor();
        Console.WriteLine();

        return customersJsonPath;
    }

    public async Task<string> PostCustomersAsync(string customersJsonPath, string outputPath)
    {
        logger.LogInformation("=== POST CUSTOMERS TO PARADIGM ===");

        var customers = await fileService.ReadFromJsonAsync<List<Customer>>(customersJsonPath);
        if (customers == null) throw new InvalidOperationException("Failed to read customers");

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

        return updatedPath;
    }

    public async Task GetDeleteCustomersSqlQueryAsync(string jsonPath)
    {
        logger.LogInformation("=== DELETE CUSTOMERS ===");
        logger.LogInformation("Reading customers from {Path}", jsonPath);

        var customers = await fileService.ReadFromJsonAsync<List<Customer>>(jsonPath);
        if (customers == null || customers.Count == 0)
        {
            logger.LogError("No customers found in {Path}", jsonPath);
            throw new InvalidOperationException("No customers found");
        }

        var deleteQuery = await customerGenerationService.GetDeleteCustomersSqlQueryAsync(customers);

        var fileInfo = new FileInfo(jsonPath);
        var directoryFullName =
            fileInfo.Directory?.FullName ?? throw new InvalidOperationException("Invalid file path");

        fileInfo = new FileInfo(Path.Combine(directoryFullName, "delete_customers.sql"));

        await File.WriteAllTextAsync(fileInfo.FullName, deleteQuery);

        Console.WriteLine("Delete customers SQL query generated successfully!");
        var displayPath = fileInfo.Replace("\\", "/");
        Console.WriteLine($"📄 File: file:///{displayPath}");
        Console.ResetColor();
        Console.WriteLine();
    }

    public async Task<string> GenerateCustomerContactsAsync(string customersJsonPath, string outputPath)
    {
        logger.LogInformation("=== GENERATE CUSTOMER CONTACTS ===");

        var contactsJsonPath =
            await customerContactGenerationService.GenerateCustomerContactsAsync(customersJsonPath, outputPath);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✓ Customer contacts generated successfully!");
        var displayPath = contactsJsonPath.Replace("\\", "/");
        Console.WriteLine($"📄 File: file:///{displayPath}");
        Console.ResetColor();
        Console.WriteLine();

        return contactsJsonPath;
    }

    public async Task PostCustomerContactsAsync(string contactsJsonPath)
    {
        logger.LogInformation("=== POST CUSTOMER CONTACTS TO PARADIGM ===");

        await paradigmApiService.BatchCreateCustomerContactsAsync(contactsJsonPath);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✓ Customer contacts posted successfully!");
        Console.ResetColor();
        Console.WriteLine();
    }

    public async Task GetDeleteCustomerContactsSqlQueryAsync(string jsonPath)
    {
        logger.LogInformation("=== DELETE CUSTOMER_CONTACTS ===");
        logger.LogInformation("Reading Contacts from {Path}", jsonPath);

        var customerContacts = await fileService.ReadFromJsonAsync<List<CustomerContact>>(jsonPath);
        if (customerContacts == null || customerContacts.Count == 0)
        {
            logger.LogError("No CustomerContacts found in {Path}", jsonPath);
            throw new InvalidOperationException("No customers found");
        }

        var deleteQuery =
            await customerContactGenerationService.GetDeleteCustomerContactSqlQueryAsync(customerContacts);

        var fileInfo = new FileInfo(jsonPath);
        var directoryFullName =
            fileInfo.Directory?.FullName ?? throw new InvalidOperationException("Invalid file path");

        fileInfo = new FileInfo(Path.Combine(directoryFullName, "delete_customers.sql"));

        await File.WriteAllTextAsync(fileInfo.FullName, deleteQuery);

        Console.WriteLine("Delete customers SQL query generated successfully!");
        var displayPath = fileInfo.Replace("\\", "/");
        Console.WriteLine($"📄 File: file:///{displayPath}");
        Console.ResetColor();
        Console.WriteLine();
    }

    public async Task GenerateOpportunitiesAsync(string customerJsonPath, string outputPath)
    {
        logger.LogInformation("=== GENERATE AND POST OPPORTUNITIES ===");
        var customers = await fileService.ReadFromJsonAsync<List<Customer>>(customerJsonPath) ?? [];

        var opportunitiesJsonPath =
            await opportunityGenerationService.GenerateOpportunitiesAsync(customers, outputPath);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✓ Opportunities generated and posted successfully!");
        var displayPath = opportunitiesJsonPath.Replace("\\", "/");
        Console.WriteLine($"📄 File: file:///{displayPath}");
        Console.ResetColor();
        Console.WriteLine();
    }

    public async Task PostOpportunitiesAsync(string opportunitiesJsonPath)
    {
        logger.LogInformation("=== POST OPPORTUNITIES TO PARADIGM ===");

        var opportunities = await fileService.ReadFromJsonAsync<List<Opportunity>>(opportunitiesJsonPath);
        if (opportunities == null) throw new InvalidOperationException("Failed to read opportunities");

        await paradigmApiService.BatchCreateOpportunitiesAsync(opportunities);

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✓ Opportunities posted successfully!");
        Console.ResetColor();
        Console.WriteLine();
    }


    public async Task GetOpportunitiesDeleteSqlQueryAsync(string jsonPath)
    {
        logger.LogInformation("=== DELETE OPPORTUNITIES ===");
        logger.LogInformation("Reading Contacts from {Path}", jsonPath);

        var customerContacts = await fileService.ReadFromJsonAsync<List<Opportunity>>(jsonPath);
        if (customerContacts == null || customerContacts.Count == 0)
        {
            logger.LogError("No CustomerContacts found in {Path}", jsonPath);
            throw new InvalidOperationException("No customers found");
        }

        var deleteQuery =
            await opportunityGenerationService.GetDeleteOpportunitiesSqlQueryAsync(customerContacts);

        var fileInfo = new FileInfo(jsonPath);
        var directoryFullName =
            fileInfo.Directory?.FullName ?? throw new InvalidOperationException("Invalid file path");

        fileInfo = new FileInfo(Path.Combine(directoryFullName, "delete_customers.sql"));

        await File.WriteAllTextAsync(fileInfo.FullName, deleteQuery);

        Console.WriteLine("Delete customers SQL query generated successfully!");
        var displayPath = fileInfo.Replace("\\", "/");
        Console.WriteLine($"📄 File: file:///{displayPath}");
        Console.ResetColor();
        Console.WriteLine();
    }
}