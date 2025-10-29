namespace HubSpotFakeData.Models;

/// <summary>
/// Represents a contact entity
/// </summary>
public class Contact(
    Guid id, 
    string email, 
    string firstName, 
    string lastName, 
    string address, 
    string city, 
    string state, 
    string zip, 
    string phone)
{
    public Guid Id { get; } = id;
    public string Email { get; } = email;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public string Address { get; } = address;
    public string City { get; } = city;
    public string State { get; } = state;
    public string Zip { get; } = zip;
    public string Phone { get; } = phone;
}

