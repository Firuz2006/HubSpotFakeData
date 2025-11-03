using ParadigmFakeData.Models.Customer;

namespace ParadigmFakeData.Services;

public interface IParadigmApiService
{
    Task<List<BaseCustomer>> BatchCreateCustomersAsync(List<BaseCustomer> customers);
    Task BatchCreateCustomerContactsAsync(string jsonPath);
    Task DeleteCustomerAsync(string customerId);
}

