using System.Text.Json.Serialization;

namespace ParadigmFakeData.Models.Customer;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(BaseCustomer), typeDiscriminator: "base")]
[JsonDerivedType(typeof(Customer), typeDiscriminator: "customer")]
[JsonDerivedType(typeof(Company), typeDiscriminator: "company")]
[JsonDerivedType(typeof(CompanyCustomer), typeDiscriminator: "companyCustomer")]
public class BaseCustomer
{
    [JsonPropertyName("strCustomerId")]
    public string? CustomerId { get; set; } = null;
}