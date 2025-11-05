using ParadigmFakeData.Models;

namespace ParadigmFakeData.Services;

public interface ICustomerContactGenerationService
{
    Task<string> GenerateCustomerContactsAsync(string customersJsonPath, string outputPath);
    Task<string> GetDeleteCustomerContactSqlQueryAsync(List<CustomerContact> customerContacts);
}