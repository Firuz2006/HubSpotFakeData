using Bogus;
using HubSpotFakeData.Models;
using Microsoft.Extensions.Logging;

namespace HubSpotFakeData.Services;

public class DataGenerationService(ILogger<DataGenerationService> logger) : IDataGenerationService
{
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

    public GenerationResult Generate(GenerationMode mode)
    {
        return mode switch
        {
            GenerationMode.Test => GenerateTestData(),
            GenerationMode.Production => GenerateProductionData(),
            _ => throw new ArgumentException($"Invalid generation mode: {mode}")
        };
    }

    private GenerationResult GenerateTestData()
    {
        logger.LogInformation("Generating test data");

        const int companyCount = 10;
        const int contactCount = 9;
        const int minDealCount = 18;

        var manager = new AssociationManager();
        var random = new Random();

        var companies = GenerateCompanies(companyCount);
        foreach (var company in companies)
            manager.AddCompany(company);

        var contacts = GenerateContacts(contactCount, companies, random, 0.9, 0.25);
        foreach (var contact in contacts)
            manager.AddContact(contact);

        AssociateContactsWithCompanies(manager, companies, contacts, random, 0.9, 0.25);

        var deals = GenerateDeals(manager, companies, contacts, random, minDealCount, 0.9, 0.05);
        foreach (var deal in deals)
            manager.AddDeal(deal);

        var result = BuildCsvRows(manager);

        LogStatistics(manager, "Test");

        return result;
    }

    private GenerationResult GenerateProductionData()
    {
        logger.LogInformation("Generating production data");

        const int companyCount = 10000;
        const int contactCount = 9000;
        const int minDealCount = 18000;

        var manager = new AssociationManager();
        var random = new Random();

        logger.LogInformation("Generating {Count} companies", companyCount);
        var companies = GenerateCompanies(companyCount);
        foreach (var company in companies)
            manager.AddCompany(company);

        logger.LogInformation("Generating {Count} contacts", contactCount);
        var contacts = GenerateContacts(contactCount, companies, random, 0.9, 0.25);
        foreach (var contact in contacts)
            manager.AddContact(contact);

        logger.LogInformation("Creating company-contact associations");
        AssociateContactsWithCompanies(manager, companies, contacts, random, 0.9, 0.25);

        logger.LogInformation("Generating deals (minimum {Count})", minDealCount);
        var deals = GenerateDeals(manager, companies, contacts, random, minDealCount, 0.9, 0.05);
        foreach (var deal in deals)
            manager.AddDeal(deal);

        logger.LogInformation("Building CSV rows");
        var result = BuildCsvRows(manager);

        LogStatistics(manager, "Production");

        return result;
    }

    private List<Company> GenerateCompanies(int count)
    {
        var faker = new Faker<Company>()
            .CustomInstantiator(f => new Company(
                Guid.NewGuid(),
                f.Internet.DomainName(),
                f.Company.CompanyName(),
                f.Address.StreetAddress(),
                f.Address.City(),
                f.Address.StateAbbr(),
                f.Address.ZipCode(),
                f.Phone.PhoneNumber("###-###-####")
            ));

        return faker.Generate(count);
    }

    private List<Contact> GenerateContacts(List<Company> companies, Random random, double oneToOnePercentage, double manyToManyPercentage)
    {
        var contactCount = (int)(companies.Count * 0.9);
        return GenerateContacts(contactCount, companies, random, oneToOnePercentage, manyToManyPercentage);
    }

    private List<Contact> GenerateContacts(int count, List<Company> companies, Random random, double oneToOnePercentage, double manyToManyPercentage)
    {
        var contacts = new List<Contact>();
        var faker = new Faker();

        var oneToOneCount = (int)(count * oneToOnePercentage);

        for (var i = 0; i < count; i++)
        {
            var email = "";
            
            if (i < oneToOneCount && i < companies.Count)
            {
                var company = companies[i];
                var username = faker.Internet.UserName().ToLower();
                email = $"{username}@{company.Domain}";
            }
            else
            {
                email = faker.Internet.Email();
            }

            var contact = new Contact(
                Guid.NewGuid(),
                email,
                faker.Name.FirstName(),
                faker.Name.LastName(),
                faker.Address.StreetAddress(),
                faker.Address.City(),
                faker.Address.StateAbbr(),
                faker.Address.ZipCode(),
                faker.Phone.PhoneNumber("###-###-####")
            );

            contacts.Add(contact);
        }

        return contacts;
    }

    private void AssociateContactsWithCompanies(AssociationManager manager, List<Company> companies, List<Contact> contacts, Random random, double oneToOnePercentage, double manyToManyPercentage)
    {
        var oneToOneCount = (int)(contacts.Count * oneToOnePercentage);
        var manyToManyCount = (int)(contacts.Count * manyToManyPercentage);

        for (var i = 0; i < contacts.Count; i++)
        {
            var contact = contacts[i];

            if (i < oneToOneCount)
            {
                var companyIndex = i < companies.Count ? i : random.Next(companies.Count);
                manager.AssociateContactWithCompany(contact.Id, companies[companyIndex].Id);
            }

            if (i < manyToManyCount)
            {
                var additionalCount = random.Next(1, 4);
                for (var j = 0; j < additionalCount; j++)
                {
                    var randomCompany = companies[random.Next(companies.Count)];
                    manager.AssociateContactWithCompany(contact.Id, randomCompany.Id);
                }
            }
        }
    }

    private List<Deal> GenerateDeals(AssociationManager manager, List<Company> companies, List<Contact> contacts, Random random, int minDealCount, double entityDealPercentage, double multipleDealPercentage)
    {
        var deals = new List<Deal>();
        var faker = new Faker();

        var companiesWithDeals = (int)(companies.Count * entityDealPercentage);
        var contactsWithDeals = (int)(contacts.Count * entityDealPercentage);
        var companiesWithMultipleDeals = (int)(companies.Count * multipleDealPercentage);
        var contactsWithMultipleDeals = (int)(contacts.Count * multipleDealPercentage);

        var dealIndex = 0;

        for (var i = 0; i < companiesWithDeals && deals.Count < minDealCount; i++)
        {
            var company = companies[i % companies.Count];
            var companyContacts = manager.GetContactsForCompany(company.Id);
            
            Contact contact;
            if (companyContacts.Count > 0)
            {
                contact = companyContacts[random.Next(companyContacts.Count)];
            }
            else
            {
                contact = contacts[random.Next(contacts.Count)];
                manager.AssociateContactWithCompany(contact.Id, company.Id);
            }

            var dealCount = i < companiesWithMultipleDeals ? random.Next(2, 4) : 1;

            for (var j = 0; j < dealCount; j++)
            {
                var deal = CreateDeal(faker, dealIndex++, company.Id, contact.Id);
                deals.Add(deal);
            }
        }

        for (var i = 0; i < contactsWithDeals && deals.Count < minDealCount; i++)
        {
            var contact = contacts[i % contacts.Count];
            var contactCompanies = manager.GetCompaniesForContact(contact.Id);

            Company company;
            if (contactCompanies.Count > 0)
            {
                company = contactCompanies[random.Next(contactCompanies.Count)];
            }
            else
            {
                company = companies[random.Next(companies.Count)];
                manager.AssociateContactWithCompany(contact.Id, company.Id);
            }

            if (manager.GetDealCountForContact(contact.Id) > 0)
                continue;

            var dealCount = i < contactsWithMultipleDeals ? random.Next(2, 4) : 1;

            for (var j = 0; j < dealCount; j++)
            {
                var deal = CreateDeal(faker, dealIndex++, company.Id, contact.Id);
                deals.Add(deal);
            }
        }

        while (deals.Count < minDealCount)
        {
            var company = companies[random.Next(companies.Count)];
            var companyContacts = manager.GetContactsForCompany(company.Id);

            Contact contact;
            if (companyContacts.Count > 0)
            {
                contact = companyContacts[random.Next(companyContacts.Count)];
            }
            else
            {
                contact = contacts[random.Next(contacts.Count)];
                manager.AssociateContactWithCompany(contact.Id, company.Id);
            }

            var deal = CreateDeal(faker, dealIndex++, company.Id, contact.Id);
            deals.Add(deal);
        }

        return deals;
    }

    private Deal CreateDeal(Faker faker, int index, Guid companyId, Guid contactId)
    {
        var stage = faker.PickRandom(DealStages);
        var dealName = $"{faker.Commerce.ProductName()} - {faker.Commerce.Department()} #{index + 1}";
        var description = $"{faker.Lorem.Sentence()} {faker.Company.Bs()}.";
        var amount = faker.Finance.Amount(1000, 500000);
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

    private GenerationResult BuildCsvRows(AssociationManager manager)
    {
        var companies = manager.GetAllCompanies();
        var contacts = manager.GetAllContacts();
        
        var companyContactRows = new List<CsvCompanyContact>();
        var companyDealRows = new List<CsvCompanyDeal>();
        var contactDealRows = new List<CsvContactDeal>();

        foreach (var company in companies)
        {
            var companyContacts = manager.GetContactsForCompany(company.Id);
            
            foreach (var contact in companyContacts)
            {
                var row = new CsvCompanyContact(
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
                    contact.Phone
                );
                companyContactRows.Add(row);
            }
        }

        foreach (var company in companies)
        {
            var deals = manager.GetDealsForCompany(company.Id);
            
            foreach (var deal in deals)
            {
                var row = new CsvCompanyDeal(
                    company.Domain,
                    deal.Stage,
                    deal.Pipeline,
                    deal.Name,
                    deal.Description,
                    deal.Amount,
                    deal.CloseDate
                );
                companyDealRows.Add(row);
            }
        }

        foreach (var contact in contacts)
        {
            var deals = manager.GetDealsForContact(contact.Id);
            
            foreach (var deal in deals)
            {
                var row = new CsvContactDeal(
                    contact.Email,
                    deal.Stage,
                    deal.Pipeline,
                    deal.Name,
                    deal.Description,
                    deal.Amount,
                    deal.CloseDate
                );
                contactDealRows.Add(row);
            }
        }

        return new GenerationResult(companyContactRows, companyDealRows, contactDealRows);
    }

    private void LogStatistics(AssociationManager manager, string mode)
    {
        var companies = manager.GetAllCompanies();
        var contacts = manager.GetAllContacts();
        var deals = manager.GetAllDeals();

        logger.LogInformation("=== {Mode} Mode Statistics ===", mode);
        logger.LogInformation("Total Companies: {Count}", companies.Count);
        logger.LogInformation("Total Contacts: {Count}", contacts.Count);
        logger.LogInformation("Total Deals: {Count}", deals.Count);

        var companiesWithDeals = companies.Count(c => manager.GetDealCountForCompany(c.Id) > 0);
        var companiesWithMultipleDeals = companies.Count(c => manager.GetDealCountForCompany(c.Id) > 1);
        var contactsWithDeals = contacts.Count(c => manager.GetDealCountForContact(c.Id) > 0);
        var contactsWithMultipleDeals = contacts.Count(c => manager.GetDealCountForContact(c.Id) > 1);
        var contactsWithCompanies = contacts.Count(c => manager.GetCompanyCountForContact(c.Id) > 0);
        var contactsWithMultipleCompanies = contacts.Count(c => manager.GetCompanyCountForContact(c.Id) > 1);

        logger.LogInformation("Companies with deals: {Count} ({Percentage:F1}%)", 
            companiesWithDeals, (double)companiesWithDeals / companies.Count * 100);
        logger.LogInformation("Companies with multiple deals: {Count} ({Percentage:F1}%)", 
            companiesWithMultipleDeals, (double)companiesWithMultipleDeals / companies.Count * 100);
        logger.LogInformation("Contacts with deals: {Count} ({Percentage:F1}%)", 
            contactsWithDeals, (double)contactsWithDeals / contacts.Count * 100);
        logger.LogInformation("Contacts with multiple deals: {Count} ({Percentage:F1}%)", 
            contactsWithMultipleDeals, (double)contactsWithMultipleDeals / contacts.Count * 100);
        logger.LogInformation("Contacts with companies: {Count} ({Percentage:F1}%)", 
            contactsWithCompanies, (double)contactsWithCompanies / contacts.Count * 100);
        logger.LogInformation("Contacts with multiple companies: {Count} ({Percentage:F1}%)", 
            contactsWithMultipleCompanies, (double)contactsWithMultipleCompanies / contacts.Count * 100);

        if (companies.Count > 0)
        {
            var avgDealsPerCompany = companies.Average(c => manager.GetDealCountForCompany(c.Id));
            logger.LogInformation("Average deals per company: {Avg:F2}", avgDealsPerCompany);
        }

        if (contacts.Count > 0)
        {
            var avgDealsPerContact = contacts.Average(c => manager.GetDealCountForContact(c.Id));
            var avgCompaniesPerContact = contacts.Average(c => manager.GetCompanyCountForContact(c.Id));
            logger.LogInformation("Average deals per contact: {Avg:F2}", avgDealsPerContact);
            logger.LogInformation("Average companies per contact: {Avg:F2}", avgCompaniesPerContact);
        }
    }
}