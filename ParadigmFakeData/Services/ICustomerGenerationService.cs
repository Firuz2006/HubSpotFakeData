using ParadigmFakeData.Models;

namespace ParadigmFakeData.Services;

public interface ICustomerGenerationService
{
    Task<List<Customer>> GenerateCustomersAsync();
    Task<string> GetDeleteCustomersSqlQueryAsync(List<Customer> customers);
}