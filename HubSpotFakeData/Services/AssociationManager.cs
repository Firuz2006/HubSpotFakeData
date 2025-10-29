using HubSpotFakeData.Models;

namespace HubSpotFakeData.Services;

/// <summary>
/// Manages associations between companies, contacts, and deals
/// </summary>
public class AssociationManager
{
    private readonly Dictionary<Guid, Company> _companies = new();
    private readonly Dictionary<Guid, Contact> _contacts = new();
    private readonly Dictionary<Guid, Deal> _deals = new();
    private readonly Dictionary<Guid, HashSet<Guid>> _companyContacts = new();
    private readonly Dictionary<Guid, HashSet<Guid>> _contactCompanies = new();
    private readonly Dictionary<Guid, HashSet<Guid>> _companyDeals = new();
    private readonly Dictionary<Guid, HashSet<Guid>> _contactDeals = new();

    /// <summary>
    /// Adds a company to the manager
    /// </summary>
    public void AddCompany(Company company)
    {
        _companies[company.Id] = company;
        _companyContacts.TryAdd(company.Id, new HashSet<Guid>());
        _companyDeals.TryAdd(company.Id, new HashSet<Guid>());
    }

    /// <summary>
    /// Adds a contact to the manager
    /// </summary>
    public void AddContact(Contact contact)
    {
        _contacts[contact.Id] = contact;
        _contactCompanies.TryAdd(contact.Id, new HashSet<Guid>());
        _contactDeals.TryAdd(contact.Id, new HashSet<Guid>());
    }

    /// <summary>
    /// Adds a deal to the manager
    /// </summary>
    public void AddDeal(Deal deal)
    {
        _deals[deal.Id] = deal;
        
        _companyDeals[deal.CompanyId].Add(deal.Id);
        _contactDeals[deal.ContactId].Add(deal.Id);
    }

    /// <summary>
    /// Associates a contact with a company
    /// </summary>
    public void AssociateContactWithCompany(Guid contactId, Guid companyId)
    {
        _companyContacts[companyId].Add(contactId);
        _contactCompanies[contactId].Add(companyId);
    }

    /// <summary>
    /// Gets all companies
    /// </summary>
    public List<Company> GetAllCompanies() => _companies.Values.ToList();

    /// <summary>
    /// Gets all contacts
    /// </summary>
    public List<Contact> GetAllContacts() => _contacts.Values.ToList();

    /// <summary>
    /// Gets all deals
    /// </summary>
    public List<Deal> GetAllDeals() => _deals.Values.ToList();

    /// <summary>
    /// Gets a company by ID
    /// </summary>
    public Company GetCompany(Guid id) => _companies[id];

    /// <summary>
    /// Gets a contact by ID
    /// </summary>
    public Contact GetContact(Guid id) => _contacts[id];

    /// <summary>
    /// Gets contacts associated with a company
    /// </summary>
    public List<Contact> GetContactsForCompany(Guid companyId)
    {
        return _companyContacts[companyId].Select(id => _contacts[id]).ToList();
    }

    /// <summary>
    /// Gets companies associated with a contact
    /// </summary>
    public List<Company> GetCompaniesForContact(Guid contactId)
    {
        return _contactCompanies[contactId].Select(id => _companies[id]).ToList();
    }

    /// <summary>
    /// Gets number of deals for a company
    /// </summary>
    public int GetDealCountForCompany(Guid companyId) => _companyDeals[companyId].Count;

    /// <summary>
    /// Gets number of deals for a contact
    /// </summary>
    public int GetDealCountForContact(Guid contactId) => _contactDeals[contactId].Count;

    /// <summary>
    /// Gets number of companies associated with a contact
    /// </summary>
    public int GetCompanyCountForContact(Guid contactId) => _contactCompanies[contactId].Count;
}

