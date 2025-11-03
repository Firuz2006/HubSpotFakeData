namespace ParadigmFakeData.Services;

public interface ICustomerGenerationService
{
    Task<string> GenerateCustomersAsync(string outputPath);
}