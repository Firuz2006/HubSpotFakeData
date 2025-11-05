using Bogus;
using Microsoft.Extensions.Logging;
using ParadigmFakeData.Models;

namespace ParadigmFakeData.Services;

public class CustomerGenerationService(
    ILogger<CustomerGenerationService> logger,
    IFileService fileService,
    GenerationSettings settings,
    DatabaseSettings databaseSettings)
    : ICustomerGenerationService
{
    private readonly HashSet<string> _usedCompanyNames = new();
    private readonly HashSet<string> _usedEmails = new();
    private readonly HashSet<string> _usedPhones = new();
    private readonly HashSet<string> _usedWebsites = new();

    public async Task<string> GenerateCustomersAsync(string outputPath)
    {
        logger.LogInformation("Starting customer generation...");

        var companies = GenerateCompanies(settings.CompanyCount);
        var customers = GenerateCustomers(settings.CustomerCount);
        var companyCustomers = GenerateCompanyCustomers(settings.CompanyCustomerCount);

        var allCustomers = new List<Customer>();
        allCustomers.AddRange(companies);
        allCustomers.AddRange(customers);
        allCustomers.AddRange(companyCustomers);

        var filePath = await fileService.SaveToJsonAsync(allCustomers, outputPath, "customers.json");

        logger.LogInformation(
            "Generated {Total} customers: {Companies} companies, {Customers} customers, {CompanyCustomers} company-customers",
            allCustomers.Count, companies.Count, customers.Count, companyCustomers.Count);

        return filePath;
    }

    public Task<string> GetDeleteCustomersSqlQueryAsync(List<Customer> customers)
    {
        logger.LogInformation("Generating SQL delete query for {Count} customers...", customers.Count);

        var customerIds = customers
            .Where(c => !string.IsNullOrEmpty(c.CustomerId))
            .Select(c => $"'{c.CustomerId}'")
            .ToList();

        if (customerIds.Count == 0)
        {
            logger.LogWarning("No customer IDs found. Cannot generate delete query.");
            return Task.FromResult(string.Empty);
        }

        var sqlQuery =
            $"DELETE FROM {databaseSettings.CustomersTableName} WHERE {databaseSettings.CustomersIdColName} IN ({string.Join(", ", customerIds)});";

        logger.LogInformation("Generated SQL delete query.");

        return Task.FromResult(sqlQuery);
    }

    private List<Customer> GenerateCompanies(int count)
    {
        var faker = new Faker<Customer>()
            .RuleFor(c => c.CompanyName, f => GenerateUniqueCompanyName(f))
            .RuleFor(c => c.WebsiteUrl, f => GenerateUniqueWebsite(f));

        return faker.Generate(count);
    }

    private List<Customer> GenerateCustomers(int count)
    {
        var faker = new Faker<Customer>()
            .RuleFor(c => c.FirstName, f => f.Name.FirstName())
            .RuleFor(c => c.LastName, f => f.Name.LastName())
            .RuleFor(c => c.PrimaryEmail, f => GenerateUniqueEmail(f))
            .RuleFor(c => c.PrimaryPhone, f => GenerateUniquePhone(f));

        return faker.Generate(count);
    }

    private List<Customer> GenerateCompanyCustomers(int count)
    {
        var faker = new Faker<Customer>()
            .RuleFor(c => c.CompanyName, f => GenerateUniqueCompanyName(f))
            .RuleFor(c => c.WebsiteUrl, f => GenerateUniqueWebsite(f))
            .RuleFor(c => c.FirstName, f => f.Name.FirstName())
            .RuleFor(c => c.LastName, f => f.Name.LastName())
            .RuleFor(c => c.PrimaryEmail, f => GenerateUniqueEmail(f))
            .RuleFor(c => c.PrimaryPhone, f => GenerateUniquePhone(f));

        return faker.Generate(count);
    }

    private string GenerateUniqueEmail(Faker f)
    {
        string email;
        do
        {
            email = f.Internet.Email();
        } while (!_usedEmails.Add(email));

        return email;
    }

    private string GenerateUniquePhone(Faker f)
    {
        string phone;
        do
        {
            phone = f.Phone.PhoneNumber("###-###-####");
        } while (!_usedPhones.Add(phone));

        return phone;
    }

    private string GenerateUniqueCompanyName(Faker f)
    {
        string name;
        do
        {
            name = f.Company.CompanyName();
        } while (!_usedCompanyNames.Add(name));

        return name;
    }

    private string GenerateUniqueWebsite(Faker f)
    {
        string website;
        do
        {
            website = f.Internet.Url();
        } while (!_usedWebsites.Add(website));

        return website;
    }
}