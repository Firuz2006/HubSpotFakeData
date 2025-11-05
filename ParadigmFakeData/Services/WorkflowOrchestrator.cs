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

        var customers = await customerGenerationService.GenerateCustomersAsync();

        var customersJsonPath = await fileService.SaveToJsonAsync(customers, outputPath, "customers.json");

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✓ Customers generated successfully!");
        var displayPath = customersJsonPath.Replace("\\", "/");
        Console.WriteLine($"📄 File: file:///{displayPath}");
        Console.ResetColor();
        Console.WriteLine();

        return customersJsonPath;
    }

    public async Task<string> PostCustomersAsync(string customersJsonPath)
    {
        logger.LogInformation("=== POST CUSTOMERS TO PARADIGM ===");

        var customers = await fileService.ReadFromJsonAsync<List<Customer>>(customersJsonPath);
        if (customers == null) throw new InvalidOperationException("Failed to read customers");

        var updatedCustomers = await paradigmApiService.BatchCreateCustomersAsync(customers);

        var updatedPath = await SaveJsonNextToInputAsync(customersJsonPath, updatedCustomers, "customer_posted.json");

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

        var customers = await ReadJsonListAsync<Customer>(jsonPath);

        var deleteQuery = await customerGenerationService.GetDeleteCustomersSqlQueryAsync(customers);

        var saved = await SaveTextNextToInputAsync(jsonPath, deleteQuery, "delete_customers.sql");

        Console.WriteLine("Delete customers SQL query generated successfully!");
        var displayPath = saved.Replace("\\", "/");
        Console.WriteLine($"📄 File: file:///{displayPath}");
        Console.ResetColor();
        Console.WriteLine();
    }

    public async Task<string> GenerateCustomerContactsAsync(string customersJsonPath)
    {
        logger.LogInformation("=== GENERATE CUSTOMER CONTACTS ===");

        var customers = await ReadJsonListAsync<Customer>(customersJsonPath);

        var contacts = await customerContactGenerationService.GenerateCustomerContactsAsync(customers);

        var contactsPath = await SaveJsonNextToInputAsync(customersJsonPath, contacts, "customer_contacts.json");

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✓ Customer contacts generated successfully!");
        var displayPath = contactsPath.Replace("\\", "/");
        Console.WriteLine($"📄 File: file:///{displayPath}");
        Console.ResetColor();
        Console.WriteLine();

        return contactsPath;
    }

    public async Task PostCustomerContactsAsync(string contactsJsonPath)
    {
        logger.LogInformation("=== POST CUSTOMER CONTACTS TO PARADIGM ===");

        var contacts = await ReadJsonListAsync<CustomerContact>(contactsJsonPath);
        if (contacts == null || contacts.Count == 0)
            throw new InvalidOperationException("Failed to read customer contacts");

        var posted = await paradigmApiService.BatchCreateCustomerContactsAsync(contacts);

        var saved = await SaveJsonNextToInputAsync(contactsJsonPath, posted, "customer_contacts_posted.json");

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✓ Customer contacts posted successfully!");
        var displayPath = saved.Replace("\\", "/");
        Console.WriteLine($"📄 Posted file: file:///{displayPath}");
        Console.ResetColor();
        Console.WriteLine();
    }

    public async Task GetDeleteCustomerContactsSqlQueryAsync(string jsonPath)
    {
        logger.LogInformation("=== DELETE CUSTOMER_CONTACTS ===");
        logger.LogInformation("Reading Contacts from {Path}", jsonPath);

        var customerContacts = await ReadJsonListAsync<CustomerContact>(jsonPath);
        if (customerContacts.Count == 0)
        {
            logger.LogError("No CustomerContacts found in {Path}", jsonPath);
            throw new InvalidOperationException("No customers found");
        }

        var deleteQuery =
            await customerContactGenerationService.GetDeleteCustomerContactSqlQueryAsync(customerContacts);

        var saved = await SaveTextNextToInputAsync(jsonPath, deleteQuery, "delete_customer_contacts.sql");

        Console.WriteLine("Delete customers SQL query generated successfully!");
        var displayPath = saved.Replace("\\", "/");
        Console.WriteLine($"📄 File: file:///{displayPath}");
        Console.ResetColor();
        Console.WriteLine();
    }

    public async Task<string> GenerateOpportunitiesAsync(string customerJsonPath)
    {
        logger.LogInformation("=== GENERATE AND POST OPPORTUNITIES ===");
        var customers = await ReadJsonListAsync<Customer>(customerJsonPath);

        var opportunities = await opportunityGenerationService.GenerateOpportunitiesAsync(customers);

        // Post to API and get returned/updated opportunities
        var posted = await paradigmApiService.BatchCreateOpportunitiesAsync(opportunities);

        var opportunitiesPath = await SaveJsonNextToInputAsync(customerJsonPath, posted, "opportunities.json");

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✓ Opportunities generated and posted successfully!");
        var displayPath = opportunitiesPath.Replace("\\", "/");
        Console.WriteLine($"📄 File: file:///{displayPath}");
        Console.ResetColor();
        Console.WriteLine();

        return opportunitiesPath;
    }

    public async Task PostOpportunitiesAsync(string opportunitiesJsonPath)
    {
        logger.LogInformation("=== POST OPPORTUNITIES TO PARADIGM ===");

        var opportunities = await ReadJsonListAsync<Opportunity>(opportunitiesJsonPath);
        if (opportunities == null || opportunities.Count == 0)
            throw new InvalidOperationException("Failed to read opportunities");

        var posted = await paradigmApiService.BatchCreateOpportunitiesAsync(opportunities);

        var saved = await SaveJsonNextToInputAsync(opportunitiesJsonPath, posted, "opportunities_posted.json");

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("✓ Opportunities posted successfully!");
        var displayPath = saved.Replace("\\", "/");
        Console.WriteLine($"📄 Posted file: file:///{displayPath}");
        Console.ResetColor();
        Console.WriteLine();
    }

    public async Task GetOpportunitiesDeleteSqlQueryAsync(string jsonPath)
    {
        logger.LogInformation("=== DELETE OPPORTUNITIES ===");
        logger.LogInformation("Reading Contacts from {Path}", jsonPath);

        var opportunities = await ReadJsonListAsync<Opportunity>(jsonPath);
        if (opportunities.Count == 0)
        {
            logger.LogError("No opportunities found in {Path}", jsonPath);
            throw new InvalidOperationException("No opportunities found");
        }

        var deleteQuery = await opportunityGenerationService.GetDeleteOpportunitiesSqlQueryAsync(opportunities);

        var saved = await SaveTextNextToInputAsync(jsonPath, deleteQuery, "delete_opportunities.sql");

        Console.WriteLine("Delete customers SQL query generated successfully!");
        var displayPath = saved.Replace("\\", "/");
        Console.WriteLine($"📄 File: file:///{displayPath}");
        Console.ResetColor();
        Console.WriteLine();
    }

    // Helpers
    private async Task<List<T>> ReadJsonListAsync<T>(string jsonPath)
    {
        var list = await fileService.ReadFromJsonAsync<List<T>>(jsonPath);
        return list ?? [];
    }

    private async Task<string> SaveJsonNextToInputAsync<T>(string inputPath, T data, string fileName)
    {
        var inputFile = new FileInfo(inputPath);
        var directoryFullName =
            inputFile.Directory?.FullName ?? throw new InvalidOperationException("Invalid file path");
        return await fileService.SaveToJsonAsync(data, directoryFullName, fileName);
    }

    private async Task<string> SaveTextNextToInputAsync(string inputPath, string content, string fileName)
    {
        var inputFile = new FileInfo(inputPath);
        var directoryFullName =
            inputFile.Directory?.FullName ?? throw new InvalidOperationException("Invalid file path");
        return await fileService.SaveTextAsync(content, directoryFullName, fileName);
    }
}