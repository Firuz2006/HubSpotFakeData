namespace HubSpotFakeData.Models;

/// <summary>
/// Represents a company entity
/// </summary>
public class Company(
    Guid id, 
    string domain, 
    string name, 
    string address, 
    string city, 
    string state, 
    string zip, 
    string phoneNumber)
{
    public Guid Id { get; } = id;
    public string Domain { get; } = domain;
    public string Name { get; } = name;
    public string Address { get; } = address;
    public string City { get; } = city;
    public string State { get; } = state;
    public string Zip { get; } = zip;
    public string PhoneNumber { get; } = phoneNumber;
}

