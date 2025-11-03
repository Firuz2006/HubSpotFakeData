using System.Text.Json.Serialization;

namespace ParadigmFakeData.Models.Customer;

public class BaseCustomer
{
    [JsonPropertyName("strCustomerId")]
    public string? CustomerId { get; set; } = null;
}