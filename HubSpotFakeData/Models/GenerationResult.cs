namespace HubSpotFakeData.Models;

public class GenerationResult(List<CsvCompany> companies, List<CsvContact> contacts)
{
    public List<CsvCompany> Companies { get; } = companies;
    public List<CsvContact> Contacts { get; } = contacts;
}

