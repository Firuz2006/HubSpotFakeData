using Bogus;
using Microsoft.Extensions.Logging;
using ParadigmFakeData.Models;

namespace ParadigmFakeData.Services;

public class OpportunityGenerationService(
    ILogger<OpportunityGenerationService> logger,
    IFileService fileService,
    IParadigmApiService apiService,
    GenerationSettings settings) : IOpportunityGenerationService
{
    public async Task<string> GenerateAndPostOpportunitiesAsync(string outputPath)
    {
        logger.LogInformation("Starting opportunity generation...");

        var customerIds = await LoadCustomerIdsAsync();
        
        if (customerIds.Count == 0)
        {
            logger.LogWarning("No customer IDs found. Cannot generate opportunities.");
            return string.Empty;
        }

        var opportunities = GenerateOpportunities(settings.OpportunityCount, customerIds);

        logger.LogInformation("Generated {Count} opportunities, posting to API...", opportunities.Count);

        await apiService.BatchCreateOpportunitiesAsync(opportunities);

        var filePath = await fileService.SaveToJsonAsync(opportunities, outputPath, "opportunities.json");

        logger.LogInformation("Posted {Count} opportunities and saved to {Path}", opportunities.Count, filePath);

        return filePath;
    }

    private async Task<List<string>> LoadCustomerIdsAsync()
    {
        try
        {
            var allCustomerIds = new List<string>();
            var customerJsonPaths = FindAllCustomersJson(settings.CustomerJsonPath);
            
            if (customerJsonPaths.Count == 0)
            {
                logger.LogWarning("No customers_updated.json files found in {Path}", settings.CustomerJsonPath);
                return new List<string>();
            }

            foreach (var jsonPath in customerJsonPaths)
            {
                var customers = await fileService.ReadFromJsonAsync<List<Customer>>(jsonPath);
                
                if (customers == null || customers.Count == 0)
                {
                    logger.LogWarning("No customers loaded from {Path}", jsonPath);
                    continue;
                }

                var customerIds = customers
                    .Where(c => !string.IsNullOrEmpty(c.CustomerId))
                    .Select(c => c.CustomerId!)
                    .ToList();

                logger.LogInformation("Loaded {Count} customer IDs from {Path}", customerIds.Count, jsonPath);
                allCustomerIds.AddRange(customerIds);
            }

            logger.LogInformation("Loaded total {Count} customer IDs from {FileCount} files", allCustomerIds.Count, customerJsonPaths.Count);

            return allCustomerIds;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load customer IDs from {Path}", settings.CustomerJsonPath);
            return new List<string>();
        }
    }

    private List<string> FindAllCustomersJson(string basePath)
    {
        var customerFiles = new List<string>();
        
        if (!Directory.Exists(basePath))
        {
            logger.LogWarning("Directory does not exist: {Path}", basePath);
            return customerFiles;
        }

        var outputDirs = Directory.GetDirectories(basePath, "output_*");

        foreach (var dir in outputDirs)
        {
            var updatedPath = Path.Combine(dir, "customers_updated.json");
            if (File.Exists(updatedPath))
            {
                logger.LogInformation("Found customers file: {Path}", updatedPath);
                customerFiles.Add(updatedPath);
                continue;
            }

            var customersPath = Path.Combine(dir, "customers.json");
            if (File.Exists(customersPath))
            {
                logger.LogInformation("Found customers file: {Path}", customersPath);
                customerFiles.Add(customersPath);
            }
        }

        if (customerFiles.Count == 0)
        {
            logger.LogWarning("No customers.json or customers_updated.json files found in {Path}", basePath);
        }

        return customerFiles;
    }

    private List<Opportunity> GenerateOpportunities(int count, List<string> customerIds)
    {
        var faker = new Faker<Opportunity>()
            .RuleFor(o => o.Name, f => f.Commerce.ProductName())
            .RuleFor(o => o.CustomerId, f => f.PickRandom(customerIds))
            .RuleFor(o => o.Date, _ => DateTime.UtcNow);

        return faker.Generate(count);
    }
}

