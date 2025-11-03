namespace ParadigmFakeData.Services;

public interface ICustomerContactGenerationService
{
    Task<string> GenerateCustomerContactsAsync(string customersJsonPath, string outputPath);
}

