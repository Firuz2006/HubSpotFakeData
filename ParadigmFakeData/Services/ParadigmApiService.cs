using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using ParadigmFakeData.Models;

namespace ParadigmFakeData.Services;

public class ParadigmApiService(ILogger<ParadigmApiService> logger, HttpClient httpClient)
    : IParadigmApiService
{
    // Base address is configured on the HttpClient in Program.cs

    public async Task<List<Customer>> BatchCreateCustomersAsync(List<Customer> customers)
    {
        logger.LogInformation("Sending {Count} customers to Paradigm API...", customers.Count);

        try
        {
            var response = await httpClient.PostAsJsonAsync("Customer/batch-create", customers);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<Customer>>();

            if (result == null || result.Count == 0)
            {
                logger.LogError("API returned empty response");
                throw new InvalidOperationException("API returned empty response");
            }

            logger.LogInformation("Successfully created {Count} customers in Paradigm", result.Count);
            return result;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP request failed while creating customers");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to batch create customers");
            throw;
        }
    }

    public async Task<List<CustomerContact>> BatchCreateCustomerContactsAsync(List<CustomerContact> contacts)
    {
        logger.LogInformation("Sending {Count} customer contacts to Paradigm API...", contacts.Count);

        if (contacts == null || contacts.Count == 0)
            throw new InvalidOperationException("No customer contacts to post");

        try
        {
            var response = await httpClient.PostAsJsonAsync("CustomerContact/batch-create", contacts);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<CustomerContact>>();

            if (result == null || result.Count == 0)
            {
                logger.LogWarning("API returned empty response for customer contacts");
                return contacts;
            }

            logger.LogInformation("Successfully created {Count} customer contacts in Paradigm", result.Count);
            return result;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP request failed while creating customer contacts");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to batch create customer contacts");
            throw;
        }
    }

    public async Task<List<Opportunity>> BatchCreateOpportunitiesAsync(List<Opportunity> opportunities)
    {
        logger.LogInformation("Sending {Count} opportunities to Paradigm API...", opportunities.Count);

        try
        {
            var response = await httpClient.PostAsJsonAsync("Opportunity/batch-create", opportunities);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<Opportunity>>();

            if (result == null)
            {
                logger.LogWarning("API returned empty response for opportunities");
                return opportunities;
            }

            logger.LogInformation("Successfully created {Count} opportunities in Paradigm", result.Count);
            return result;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP request failed while creating opportunities");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to batch create opportunities");
            throw;
        }
    }

    public async Task DeleteCustomerAsync(string customerId)
    {
        logger.LogInformation("Deleting customer {CustomerId} from Paradigm...", customerId);

        try
        {
            var response = await httpClient.DeleteAsync($"CustomerContact/{customerId}");
            response.EnsureSuccessStatusCode();

            logger.LogInformation("Successfully deleted customer {CustomerId}", customerId);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP request failed while deleting customer {CustomerId}", customerId);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete customer {CustomerId}", customerId);
            throw;
        }
    }
}