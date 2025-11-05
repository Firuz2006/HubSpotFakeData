using ParadigmFakeData.Models;

namespace ParadigmFakeData.Services;

public interface IOpportunityGenerationService
{
    Task<string> GenerateOpportunitiesAsync(List<Customer> customers, string outputPath);
    Task<string> GetDeleteOpportunitiesSqlQueryAsync(List<Opportunity> opportunities);
}