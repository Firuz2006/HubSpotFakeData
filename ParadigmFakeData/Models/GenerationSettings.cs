namespace ParadigmFakeData.Models;

public class GenerationSettings
{
    public int CompanyCount { get; set; }
    public int CustomerCount { get; set; }
    public int CompanyCustomerCount { get; set; }
    public int OpportunityCount { get; set; }
    public string CustomerJsonPath { get; set; } = string.Empty;
    public string ApiKey { get; set; }
}