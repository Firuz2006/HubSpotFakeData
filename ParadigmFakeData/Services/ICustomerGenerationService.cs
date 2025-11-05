using ParadigmFakeData.Models;

namespace ParadigmFakeData.Services;

public interface ICustomerGenerationService
{
    Task<string> GenerateCustomersAsync(string outputPath);
    Task<string> GetDeleteCustomersSqlQueryAsync(List<Customer> customers);
}