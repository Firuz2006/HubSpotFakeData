using System.Text.Json.Serialization;

namespace ParadigmFakeData.Models;

public class CustomerContact
{
    [JsonPropertyName("strCustomerID")]
    public required string CustomerId { get; set; }


    [JsonPropertyName("strContactID")]
    public string ContactId { get; set; } = "";

    [JsonPropertyName("strFirstName")]
    public string FirstName { get; set; } = "";

    [JsonPropertyName("strLastName")]
    public string LastName { get; set; } = "";

    [JsonPropertyName("strPrimaryEmail")]
    public string PrimaryEmail { get; set; } = "";

    [JsonPropertyName("strPrimaryPhone")]
    public string PrimaryPhone { get; set; } = "";
}