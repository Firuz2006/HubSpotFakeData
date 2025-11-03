using System.Text.Json.Serialization;

namespace ParadigmFakeData.Models.Customer;

public class Customer
{
    [JsonPropertyName("strFirstName")]
    public string FirstName { get; set; }

    [JsonPropertyName("strLastName")]
    public string LastName { get; set; }

    [JsonPropertyName("strPrimaryEmail")]
    public string PrimaryEmail { get; set; }

    [JsonPropertyName("strPrimaryPhone")]
    public string PrimaryPhone { get; set; }
}