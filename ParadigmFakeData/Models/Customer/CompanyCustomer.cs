using System.Text.Json.Serialization;

namespace ParadigmFakeData.Models.Customer;

public class CompanyCustomer
{
    [JsonPropertyName("strCompanyName")]
    public string CompanyName { get; set; }

    [JsonPropertyName("strPrimaryWebSite")]
    public string WebsiteUrl { get; set; }


    [JsonPropertyName("strFirstName")]
    public string FirstName { get; set; }

    [JsonPropertyName("strLastName")]
    public string LastName { get; set; }

    [JsonPropertyName("strPrimaryEmail")]
    public string PrimaryEmail { get; set; }

    [JsonPropertyName("strPrimaryPhone")]
    public string PrimaryPhone { get; set; }
}