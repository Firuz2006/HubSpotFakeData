namespace HubSpotFakeData.Models;

/// <summary>
/// Represents a deal entity
/// </summary>
public class Deal(
    Guid id, 
    string name, 
    string stage, 
    string pipeline, 
    Guid companyId, 
    Guid contactId,
    string description,
    decimal amount,
    DateTime closeDate)
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public string Stage { get; } = stage;
    public string Pipeline { get; } = pipeline;
    public Guid CompanyId { get; } = companyId;
    public Guid ContactId { get; } = contactId;
    public string Description { get; } = description;
    public decimal Amount { get; } = amount;
    public DateTime CloseDate { get; } = closeDate;
}

