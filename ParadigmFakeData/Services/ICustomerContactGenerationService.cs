using ParadigmFakeData.Models;

namespace ParadigmFakeData.Services;

public interface ICustomerContactGenerationService
{
    Task<List<CustomerContact>> GenerateCustomerContactsAsync(List<Customer> customers);
    Task<string> GetDeleteCustomerContactSqlQueryAsync(List<CustomerContact> customerContacts);
}