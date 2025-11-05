using Bogus;
using Microsoft.Extensions.Logging;
using ParadigmFakeData.Models;

namespace ParadigmFakeData.Services;

public class CustomerContactGenerationService(
    ILogger<CustomerContactGenerationService> logger,
    IFileService fileService,
    DatabaseSettings databaseSettings) : ICustomerContactGenerationService
{
    private readonly HashSet<string> _usedEmails = new();
    private readonly HashSet<string> _usedPhones = new();

    public async Task<string> GenerateCustomerContactsAsync(string customersJsonPath, string outputPath)
    {
        logger.LogInformation("Starting customer contact generation from {Path}", customersJsonPath);

        var customers = await fileService.ReadFromJsonAsync<List<Customer>>(customersJsonPath);
        if (customers == null || customers.Count == 0)
        {
            logger.LogError("No customers found in {Path}", customersJsonPath);
            throw new InvalidOperationException("No customers found");
        }

        var eligibleCustomers = customers
            .Where(c => !string.IsNullOrEmpty(c.CustomerId) &&
                        !(string.IsNullOrEmpty(c.CompanyName) &&
                          string.IsNullOrEmpty(c.WebsiteUrl) &&
                          !string.IsNullOrEmpty(c.FirstName) &&
                          !string.IsNullOrEmpty(c.LastName) &&
                          !string.IsNullOrEmpty(c.PrimaryEmail) &&
                          !string.IsNullOrEmpty(c.PrimaryPhone)
                            ))
            .ToList();

        if (eligibleCustomers.Count == 0)
        {
            logger.LogError("No eligible customers (Company or Customer with CustomerId) found");
            throw new InvalidOperationException("No eligible customers found");
        }

        var contacts = GenerateContacts(eligibleCustomers);

        var filePath = await fileService.SaveToJsonAsync(contacts, outputPath, "customer_contacts.json");

        logger.LogInformation("Generated {Count} customer contacts for {CustomerCount} eligible customers",
            contacts.Count, eligibleCustomers.Count);

        return filePath;
    }

    public Task<string> GetDeleteCustomerContactSqlQueryAsync(List<CustomerContact> customerContacts)
    {
        logger.LogInformation("Generating SQL delete query for {Count} customer contacts...", customerContacts.Count);

        var contactIds = customerContacts
            .Where(c => !string.IsNullOrEmpty(c.ContactId))
            .Select(c => $"'{c.ContactId}'")
            .ToList();

        if (contactIds.Count == 0)
            throw new InvalidOperationException("No customer contact IDs found. Cannot generate delete query.");

        var sqlQuery =
            $"DELETE FROM {databaseSettings.CustomerContactTableName} WHERE {databaseSettings.CustomerContactIdColName} IN ({string.Join(", ", contactIds)});";

        logger.LogInformation("Generated SQL delete query.");

        return Task.FromResult(sqlQuery);
    }

    private List<CustomerContact> GenerateContacts(List<Customer> customers)
    {
        var contacts = new List<CustomerContact>();
        var random = new Random();

        foreach (var customer in customers)
        {
            var roll = random.Next(100);

            if (roll < 5)
            {
                // 5% no contacts
                logger.LogDebug("Customer {CustomerId} will have no contacts", customer.CustomerId);
                continue;
            }

            if (roll < 15)
            {
                // 10% multiple contacts (2-4)
                var contactCount = random.Next(2, 5);
                logger.LogDebug("Customer {CustomerId} will have {Count} contacts", customer.CustomerId, contactCount);

                for (var i = 0; i < contactCount; i++) contacts.Add(GenerateContact(customer.CustomerId!));
            }
            else
            {
                // 85% single contact
                contacts.Add(GenerateContact(customer.CustomerId!));
            }
        }

        return contacts;
    }

    private CustomerContact GenerateContact(string customerId)
    {
        var faker = new Faker();

        return new CustomerContact
        {
            CustomerId = customerId,
            FirstName = faker.Name.FirstName(),
            LastName = faker.Name.LastName(),
            PrimaryEmail = GenerateUniqueEmail(faker),
            PrimaryPhone = GenerateUniquePhone(faker)
        };
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
            phone = f.Phone.PhoneNumber();
        } while (!_usedPhones.Add(phone));

        return phone;
    }
}