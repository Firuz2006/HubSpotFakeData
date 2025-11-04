using ParadigmFakeData.Models;

namespace ParadigmFakeData.Services;

public interface IParadigmApiService
{
    Task<List<Customer>> BatchCreateCustomersAsync(List<Customer> customers);
    Task BatchCreateCustomerContactsAsync(string jsonPath);
    Task<List<Opportunity>> BatchCreateOpportunitiesAsync(List<Opportunity> opportunities);
    Task DeleteCustomerAsync(string customerId);
}