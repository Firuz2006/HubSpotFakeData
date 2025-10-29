using Bogus;
using HubSpotFakeData.Models;
using Microsoft.Extensions.Logging;

namespace HubSpotFakeData.Services;

/// <summary>
/// Service for generating fake data using Bogus library
/// </summary>
public class DataGenerationService(ILogger<DataGenerationService> logger) : IDataGenerationService
{
    private readonly ILogger<DataGenerationService> _logger = logger;
    private static readonly string[] DealStages =
    [
        "appointmentscheduled",
        "qualifiedtobuy",
        "presentationscheduled",
        "decisionmakerboughtin",
        "contractsent",
        "closedwon",
        "closedlost"
    ];

    /// <summary>
    /// Generates CSV rows based on the specified mode
    /// </summary>
    public List<CsvRow> Generate(GenerationMode mode)
    {
        return mode switch
        {
            GenerationMode.Test => GenerateTestData(),
            GenerationMode.Production => GenerateProductionData(),
            _ => throw new ArgumentException($"Invalid generation mode: {mode}")
        };
    }

    private List<CsvRow> GenerateTestData()
    {
        _logger.LogInformation("Generating test data - 20 records with all association patterns");

        var associationManager = new AssociationManager();
        var csvRows = new List<CsvRow>();

        var companyFaker = new Faker<Company>()
            .CustomInstantiator(f => new Company(
                Guid.NewGuid(),
                f.Internet.DomainName(),
                f.Company.CompanyName(),
                f.Address.StreetAddress(),
                f.Address.City(),
                f.Address.StateAbbr(),
                f.Address.ZipCode(),
                f.Phone.PhoneNumber()
            ));

        var contactFaker = new Faker<Contact>()
            .CustomInstantiator(f => new Contact(
                Guid.NewGuid(),
                f.Internet.Email(),
                f.Name.FirstName(),
                f.Name.LastName(),
                f.Address.StreetAddress(),
                f.Address.City(),
                f.Address.StateAbbr(),
                f.Address.ZipCode(),
                f.Phone.PhoneNumber()
            ));

        // Create 3 companies
        var companyA = companyFaker.Generate();
        var companyB = companyFaker.Generate();
        var companyC = companyFaker.Generate();

        associationManager.AddCompany(companyA);
        associationManager.AddCompany(companyB);
        associationManager.AddCompany(companyC);

        // Create 4 contacts
        var contact1 = contactFaker.Generate();
        var contact2 = contactFaker.Generate();
        var contact3 = contactFaker.Generate();
        var contact4 = contactFaker.Generate();

        associationManager.AddContact(contact1);
        associationManager.AddContact(contact2);
        associationManager.AddContact(contact3);
        associationManager.AddContact(contact4);

        // Associate contacts with companies
        associationManager.AssociateContactWithCompany(contact1.Id, companyA.Id);
        associationManager.AssociateContactWithCompany(contact1.Id, companyB.Id); // Many-to-many
        associationManager.AssociateContactWithCompany(contact2.Id, companyA.Id);
        associationManager.AssociateContactWithCompany(contact3.Id, companyB.Id);
        associationManager.AssociateContactWithCompany(contact4.Id, companyC.Id);
        associationManager.AssociateContactWithCompany(contact4.Id, companyA.Id); // Many-to-many

        var dealFaker = new Faker();
        var dealIndex = 0;

        // Company A - 5 deals (demonstrating one company with multiple deals)
        // Deal 1-3: Contact 1 with multiple deals
        for (var i = 0; i < 3; i++)
        {
            var deal = CreateDeal(dealFaker, dealIndex++, companyA.Id, contact1.Id);
            associationManager.AddDeal(deal);
            csvRows.Add(CreateCsvRow(companyA, contact1, deal));
        }

        // Deal 4-5: Contact 2 with multiple deals
        for (var i = 0; i < 2; i++)
        {
            var deal = CreateDeal(dealFaker, dealIndex++, companyA.Id, contact2.Id);
            associationManager.AddDeal(deal);
            csvRows.Add(CreateCsvRow(companyA, contact2, deal));
        }

        // Company B - 4 deals
        // Deal 6-8: Contact 1 with Company B (demonstrating many-to-many: contact works with multiple companies)
        for (var i = 0; i < 3; i++)
        {
            var deal = CreateDeal(dealFaker, dealIndex++, companyB.Id, contact1.Id);
            associationManager.AddDeal(deal);
            csvRows.Add(CreateCsvRow(companyB, contact1, deal));
        }

        // Deal 9: Contact 3
        var deal9 = CreateDeal(dealFaker, dealIndex++, companyB.Id, contact3.Id);
        associationManager.AddDeal(deal9);
        csvRows.Add(CreateCsvRow(companyB, contact3, deal9));

        // Company C - 6 deals
        // Deal 10-12: Contact 4 with multiple deals
        for (var i = 0; i < 3; i++)
        {
            var deal = CreateDeal(dealFaker, dealIndex++, companyC.Id, contact4.Id);
            associationManager.AddDeal(deal);
            csvRows.Add(CreateCsvRow(companyC, contact4, deal));
        }

        // Deal 13-15: Contact 2 with Company C (cross-company association)
        for (var i = 0; i < 3; i++)
        {
            var deal = CreateDeal(dealFaker, dealIndex++, companyC.Id, contact2.Id);
            associationManager.AddDeal(deal);
            csvRows.Add(CreateCsvRow(companyC, contact2, deal));
        }

        // Additional deals to reach 20
        var deal16 = CreateDeal(dealFaker, dealIndex++, companyA.Id, contact4.Id);
        associationManager.AddDeal(deal16);
        csvRows.Add(CreateCsvRow(companyA, contact4, deal16));

        var deal17 = CreateDeal(dealFaker, dealIndex++, companyB.Id, contact2.Id);
        associationManager.AddDeal(deal17);
        csvRows.Add(CreateCsvRow(companyB, contact2, deal17));

        var deal18 = CreateDeal(dealFaker, dealIndex++, companyC.Id, contact1.Id);
        associationManager.AddDeal(deal18);
        csvRows.Add(CreateCsvRow(companyC, contact1, deal18));

        var deal19 = CreateDeal(dealFaker, dealIndex++, companyA.Id, contact3.Id);
        associationManager.AddDeal(deal19);
        csvRows.Add(CreateCsvRow(companyA, contact3, deal19));

        var deal20 = CreateDeal(dealFaker, dealIndex++, companyB.Id, contact4.Id);
        associationManager.AddDeal(deal20);
        csvRows.Add(CreateCsvRow(companyB, contact4, deal20));

        LogTestModeStatistics(associationManager);

        return csvRows;
    }

    private List<CsvRow> GenerateProductionData()
    {
        _logger.LogInformation("Generating production data - 10,000 records");

        var associationManager = new AssociationManager();
        var csvRows = new List<CsvRow>();
        var random = new Random();

        var companyFaker = new Faker<Company>()
            .CustomInstantiator(f => new Company(
                Guid.NewGuid(),
                f.Internet.DomainName(),
                f.Company.CompanyName(),
                f.Address.StreetAddress(),
                f.Address.City(),
                f.Address.StateAbbr(),
                f.Address.ZipCode(),
                f.Phone.PhoneNumber()
            ));

        var contactFaker = new Faker<Contact>()
            .CustomInstantiator(f => new Contact(
                Guid.NewGuid(),
                f.Internet.Email(),
                f.Name.FirstName(),
                f.Name.LastName(),
                f.Address.StreetAddress(),
                f.Address.City(),
                f.Address.StateAbbr(),
                f.Address.ZipCode(),
                f.Phone.PhoneNumber()
            ));

        var dealFaker = new Faker();

        // Generate 650 companies
        const int companyCount = 650;
        _logger.LogInformation("Generating {Count} companies", companyCount);
        for (var i = 0; i < companyCount; i++)
        {
            var company = companyFaker.Generate();
            associationManager.AddCompany(company);
        }

        // Generate 2,500 contacts
        const int contactCount = 2500;
        _logger.LogInformation("Generating {Count} contacts", contactCount);
        for (var i = 0; i < contactCount; i++)
        {
            var contact = contactFaker.Generate();
            associationManager.AddContact(contact);
        }

        var companies = associationManager.GetAllCompanies();
        var contacts = associationManager.GetAllContacts();

        // Associate contacts with companies (3-5 contacts per company on average)
        _logger.LogInformation("Creating company-contact associations");
        foreach (var company in companies)
        {
            var contactsPerCompany = random.Next(3, 6);
            var selectedContacts = contacts.OrderBy(_ => random.Next()).Take(contactsPerCompany);
            
            foreach (var contact in selectedContacts)
            {
                associationManager.AssociateContactWithCompany(contact.Id, company.Id);
            }
        }

        // Ensure 20-30% of contacts are associated with multiple companies
        var contactsForMultipleCompanies = (int)(contactCount * 0.25);
        _logger.LogInformation("Creating multi-company associations for {Count} contacts", contactsForMultipleCompanies);
        
        var shuffledContacts = contacts.OrderBy(_ => random.Next()).Take(contactsForMultipleCompanies);
        foreach (var contact in shuffledContacts)
        {
            var additionalCompanyCount = random.Next(1, 3);
            var additionalCompanies = companies.OrderBy(_ => random.Next()).Take(additionalCompanyCount);
            
            foreach (var company in additionalCompanies)
            {
                associationManager.AssociateContactWithCompany(contact.Id, company.Id);
            }
        }

        // Generate 10,000 deals
        const int dealCount = 10000;
        _logger.LogInformation("Generating {Count} deals", dealCount);
        
        for (var i = 0; i < dealCount; i++)
        {
            if (i > 0 && i % 1000 == 0)
            {
                _logger.LogInformation("Generated {Count}/{Total} deals", i, dealCount);
            }

            // Select random company
            var company = companies[random.Next(companies.Count)];
            
            // Get contacts associated with this company
            var companyContacts = associationManager.GetContactsForCompany(company.Id);
            
            // Select random contact from company's contacts
            var contact = companyContacts.Count > 0 
                ? companyContacts[random.Next(companyContacts.Count)] 
                : contacts[random.Next(contacts.Count)];

            var deal = CreateDeal(dealFaker, i, company.Id, contact.Id);
            associationManager.AddDeal(deal);
            
            csvRows.Add(CreateCsvRow(company, contact, deal));
        }

        LogProductionModeStatistics(associationManager);

        return csvRows;
    }

    private static Deal CreateDeal(Faker faker, int index, Guid companyId, Guid contactId)
    {
        var stage = faker.PickRandom(DealStages);
        var dealName = $"{faker.Commerce.ProductName()} - {faker.Commerce.Department()} #{index + 1}";
        var description = $"{faker.Lorem.Sentence()} {faker.Company.Bs()}.";
        var amount = faker.Finance.Amount(1000, 500000, 2);
        var closeDate = faker.Date.Between(DateTime.Now.AddDays(-30), DateTime.Now.AddDays(90));
        
        return new Deal(
            Guid.NewGuid(),
            dealName,
            stage,
            "default",
            companyId,
            contactId,
            description,
            amount,
            closeDate
        );
    }

    private static CsvRow CreateCsvRow(Company company, Contact contact, Deal deal)
    {
        return new CsvRow(
            company.Domain,
            company.Name,
            company.Address,
            company.City,
            company.State,
            company.Zip,
            company.PhoneNumber,
            contact.Email,
            contact.FirstName,
            contact.LastName,
            contact.Address,
            contact.City,
            contact.State,
            contact.Zip,
            contact.Phone,
            deal.Stage,
            deal.Pipeline,
            deal.Name,
            deal.Description,
            deal.Amount,
            deal.CloseDate
        );
    }

    private void LogTestModeStatistics(AssociationManager manager)
    {
        var companies = manager.GetAllCompanies();
        var contacts = manager.GetAllContacts();

        _logger.LogInformation("=== Test Mode Statistics ===");
        _logger.LogInformation("Total Companies: {Count}", companies.Count);
        _logger.LogInformation("Total Contacts: {Count}", contacts.Count);
        _logger.LogInformation("Total Deals: {Count}", manager.GetAllDeals().Count);
        
        foreach (var company in companies)
        {
            var contactCount = manager.GetContactsForCompany(company.Id).Count;
            var dealCount = manager.GetDealCountForCompany(company.Id);
            _logger.LogInformation("Company '{Name}': {ContactCount} contacts, {DealCount} deals", 
                company.Name, contactCount, dealCount);
        }

        foreach (var contact in contacts)
        {
            var companyCount = manager.GetCompanyCountForContact(contact.Id);
            var dealCount = manager.GetDealCountForContact(contact.Id);
            _logger.LogInformation("Contact '{Email}': {CompanyCount} companies, {DealCount} deals",
                contact.Email, companyCount, dealCount);
        }
    }

    private void LogProductionModeStatistics(AssociationManager manager)
    {
        var companies = manager.GetAllCompanies();
        var contacts = manager.GetAllContacts();
        var deals = manager.GetAllDeals();

        _logger.LogInformation("=== Production Mode Statistics ===");
        _logger.LogInformation("Total Companies: {Count}", companies.Count);
        _logger.LogInformation("Total Contacts: {Count}", contacts.Count);
        _logger.LogInformation("Total Deals: {Count}", deals.Count);

        var contactsWithMultipleCompanies = contacts.Count(c => manager.GetCompanyCountForContact(c.Id) > 1);
        var multipleCompanyPercentage = (double)contactsWithMultipleCompanies / contacts.Count * 100;

        _logger.LogInformation("Contacts with multiple companies: {Count} ({Percentage:F1}%)",
            contactsWithMultipleCompanies, multipleCompanyPercentage);

        var avgContactsPerCompany = companies.Average(c => manager.GetContactsForCompany(c.Id).Count);
        var avgDealsPerContact = contacts.Average(c => manager.GetDealCountForContact(c.Id));
        var avgDealsPerCompany = companies.Average(c => manager.GetDealCountForCompany(c.Id));

        _logger.LogInformation("Average contacts per company: {Avg:F2}", avgContactsPerCompany);
        _logger.LogInformation("Average deals per contact: {Avg:F2}", avgDealsPerContact);
        _logger.LogInformation("Average deals per company: {Avg:F2}", avgDealsPerCompany);
    }
}

