using System.Text.Json.Serialization;

namespace ParadigmFakeData.Models;

public class Opportunity
{
    [JsonPropertyName("strName")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("strCustomerID")]
    public string CustomerId { get; set; } = string.Empty;

    [JsonPropertyName("dtmDate")]
    public DateTime Date { get; set; }

    [JsonPropertyName("strOpportunityStage")]
    public string OpportunityStage { get; set; } = "Presentation Scheduled";

    [JsonPropertyName("strOpportunityCategory")]
    public string OpportunityCategory { get; set; } = "string3423";

    [JsonPropertyName("strExternalID")]
    public string ExternalId { get; set; } = "string";

    [JsonPropertyName("strSalesPerson")]
    public string SalesPerson { get; set; } = "ABDRAH001";

    [JsonPropertyName("curAmount")]
    public decimal Amount { get; set; } = 10;

    [JsonPropertyName("strPrimaryOrderNumber")]
    public string PrimaryOrderNumber { get; set; } = "string";

    [JsonPropertyName("strEditLock")]
    public string EditLock { get; set; } = "string";

    [JsonPropertyName("requestRemoveLock")]
    public bool RequestRemoveLock { get; set; } = true;
}

