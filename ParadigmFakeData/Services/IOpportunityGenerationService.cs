namespace ParadigmFakeData.Services;

public interface IOpportunityGenerationService
{
    Task<string> GenerateAndPostOpportunitiesAsync(string outputPath);
}

