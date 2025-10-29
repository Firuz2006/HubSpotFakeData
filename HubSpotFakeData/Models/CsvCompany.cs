namespace HubSpotFakeData.Models;

public class CsvCompanyDeal(
    string companyDomain,
    string dealStage,
    string dealPipeline,
    string dealName,
    string dealDescription,
    decimal dealAmount,
    DateTime dealCloseDate)
{
    public string CompanyDomain { get; } = companyDomain;
    public string DealStage { get; } = dealStage;
    public string DealPipeline { get; } = dealPipeline;
    public string DealName { get; } = dealName;
    public string DealDescription { get; } = dealDescription;
    public decimal DealAmount { get; } = dealAmount;
    public DateTime DealCloseDate { get; } = dealCloseDate;
}

public class CsvContactDeal(
    string contactEmail,
    string dealStage,
    string dealPipeline,
    string dealName,
    string dealDescription,
    decimal dealAmount,
    DateTime dealCloseDate)
{
    public string ContactEmail { get; } = contactEmail;
    public string DealStage { get; } = dealStage;
    public string DealPipeline { get; } = dealPipeline;
    public string DealName { get; } = dealName;
    public string DealDescription { get; } = dealDescription;
    public decimal DealAmount { get; } = dealAmount;
    public DateTime DealCloseDate { get; } = dealCloseDate;
}