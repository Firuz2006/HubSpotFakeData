namespace ParadigmFakeData.Services;

public interface IWorkflowOrchestrator
{
    Task<string> GenerateCustomersAsync(string outputPath);
    Task<string> PostCustomersAsync(string customersJsonPath, string outputPath);
    Task GetDeleteCustomersSqlQueryAsync(string jsonPath);

    Task<string> GenerateCustomerContactsAsync(string customersJsonPath, string outputPath);
    Task PostCustomerContactsAsync(string contactsJsonPath);
    Task GetDeleteCustomerContactsSqlQueryAsync(string jsonPath);

    Task GenerateOpportunitiesAsync(string customersJsonPath, string outputPath);
    Task PostOpportunitiesAsync(string opportunitiesJsonPath);
    Task GetOpportunitiesDeleteSqlQueryAsync(string jsonPath);
}