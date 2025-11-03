using System.Text.Json.Serialization;

namespace ParadigmFakeData.Models.Customer;

public class Company
{
    [JsonPropertyName("strCompanyName")]
    public string CompanyName { get; set; }

    [JsonPropertyName("strPrimaryWebSite")]
    public string WebsiteUrl { get; set; }
}