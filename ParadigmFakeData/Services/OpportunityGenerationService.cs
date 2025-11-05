using Bogus;
using Microsoft.Extensions.Logging;
using ParadigmFakeData.Models;

namespace ParadigmFakeData.Services;

public class OpportunityGenerationService(
    ILogger<OpportunityGenerationService> logger,
    GenerationSettings settings,
    DatabaseSettings databaseSettings) : IOpportunityGenerationService
{
    public Task<List<Opportunity>> GenerateOpportunitiesAsync(List<Customer> customers)
    {
        logger.LogInformation("Starting opportunity generation...");

        var customerIds = customers
            .Where(c => !string.IsNullOrEmpty(c.CustomerId))
            .Select(c => c.CustomerId!)
            .ToList();

        if (customerIds.Count == 0)
        {
            logger.LogWarning("No customer IDs found. Cannot generate opportunities.");
            return Task.FromResult(new List<Opportunity>());
        }

        var opportunities = GenerateOpportunities(settings.OpportunityCount, customerIds);

        logger.LogInformation("Generated {Count} opportunities", opportunities.Count);

        return Task.FromResult(opportunities);
    }

    public Task<string> GetDeleteOpportunitiesSqlQueryAsync(List<Opportunity> opportunities)
    {
        logger.LogInformation("Generating SQL delete query for {Count} Opportunities...", opportunities.Count);

        var contactIds = opportunities
            .Where(c => !string.IsNullOrEmpty(c.Name))
            .Select(c => $"'{c.Name}'")
            .ToList();

        if (contactIds.Count == 0)
            throw new InvalidOperationException("No customer contact IDs found. Cannot generate delete query.");

        var sqlQuery =
            $"DELETE FROM {databaseSettings.OpportunitiesTableName} WHERE {databaseSettings.OpportunitiesNameColName} IN ({string.Join(", ", contactIds)});";

        logger.LogInformation("Generated SQL delete query.");

        return Task.FromResult(sqlQuery);
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