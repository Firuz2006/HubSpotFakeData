namespace HubSpotFakeData.Models;

/// <summary>
/// Represents a CSV row for HubSpot import
/// </summary>
public class CsvRow(
    string companyDomain,
    string companyName,
    string companyAddress,
    string companyCity,
    string companyState,
    string companyZip,
    string companyPhone,
    string contactEmail,
    string contactFirstName,
    string contactLastName,
    string contactAddress,
    string contactCity,
    string contactState,
    string contactZip,
    string contactPhone,
    string dealStage,
    string dealPipeline,
    string dealName,
    string dealDescription,
    decimal dealAmount,
    DateTime dealCloseDate)
{
    public string CompanyDomain { get; } = companyDomain;
    public string CompanyName { get; } = companyName;
    public string CompanyAddress { get; } = companyAddress;
    public string CompanyCity { get; } = companyCity;
    public string CompanyState { get; } = companyState;
    public string CompanyZip { get; } = companyZip;
    public string CompanyPhone { get; } = companyPhone;
    public string ContactEmail { get; } = contactEmail;
    public string ContactFirstName { get; } = contactFirstName;
    public string ContactLastName { get; } = contactLastName;
    public string ContactAddress { get; } = contactAddress;
    public string ContactCity { get; } = contactCity;
    public string ContactState { get; } = contactState;
    public string ContactZip { get; } = contactZip;
    public string ContactPhone { get; } = contactPhone;
    public string DealStage { get; } = dealStage;
    public string DealPipeline { get; } = dealPipeline;
    public string DealName { get; } = dealName;
    public string DealDescription { get; } = dealDescription;
    public decimal DealAmount { get; } = dealAmount;
    public DateTime DealCloseDate { get; } = dealCloseDate;
}

