using ParadigmFakeData.Models;

namespace ParadigmFakeData.Services;

public interface IOpportunityGenerationService
{
    Task<List<Opportunity>> GenerateOpportunitiesAsync(List<Customer> customers);
    Task<string> GetDeleteOpportunitiesSqlQueryAsync(List<Opportunity> opportunities);
}