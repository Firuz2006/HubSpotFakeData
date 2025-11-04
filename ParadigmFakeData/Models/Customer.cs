using System.Text.Json.Serialization;

namespace ParadigmFakeData.Models;

public class Customer
{
    [JsonPropertyName("strCustomerId")]
    public string? CustomerId { get; set; } = null;

    [JsonPropertyName("strCompanyName")]
    public string? CompanyName { get; set; } = null;

    [JsonPropertyName("strPrimaryWebSite")]
    public string? WebsiteUrl { get; set; } = null;

    [JsonPropertyName("strFirstName")]
    public string? FirstName { get; set; } = null;

    [JsonPropertyName("strLastName")]
    public string? LastName { get; set; } = null;

    [JsonPropertyName("strPrimaryEmail")]
    public string? PrimaryEmail { get; set; } = null;

    [JsonPropertyName("strPrimaryPhone")]
    public string? PrimaryPhone { get; set; } = null;

    [JsonPropertyName("strExternalID")]
    public string ExternalId { get; set; } = Guid.NewGuid().ToString();
}