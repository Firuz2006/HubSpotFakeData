namespace ParadigmFakeData.Services;

public interface IWorkflowOrchestrator
{
    Task<string> GenerateCustomersAsync(string outputPath);
    Task<string> PostCustomersAsync(string customersJsonPath);
    Task GetDeleteCustomersSqlQueryAsync(string jsonPath);

    Task<string> GenerateCustomerContactsAsync(string customersJsonPath);
    Task PostCustomerContactsAsync(string contactsJsonPath);
    Task GetDeleteCustomerContactsSqlQueryAsync(string jsonPath);

    Task<string> GenerateOpportunitiesAsync(string customersJsonPath);
    Task GetOpportunitiesDeleteSqlQueryAsync(string jsonPath);
}