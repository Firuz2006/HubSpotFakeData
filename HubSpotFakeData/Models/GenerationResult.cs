namespace HubSpotFakeData.Models;

public class GenerationResult(
    List<CsvCompanyContact> companyContacts,
    List<CsvCompanyDeal> companyDeals,
    List<CsvContactDeal> contactDeals)
{
    public List<CsvCompanyContact> CompanyContacts { get; } = companyContacts;
    public List<CsvCompanyDeal> CompanyDeals { get; } = companyDeals;
    public List<CsvContactDeal> ContactDeals { get; } = contactDeals;
}

