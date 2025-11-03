using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using ParadigmFakeData.Models;
using ParadigmFakeData.Models.Customer;

namespace ParadigmFakeData.Services;

public class ParadigmApiService(ILogger<ParadigmApiService> logger, HttpClient httpClient, IFileService fileService) : IParadigmApiService
{
    private const string BaseUrl = "http://192.168.1.130:5001/api";

    public async Task<List<BaseCustomer>> BatchCreateCustomersAsync(List<BaseCustomer> customers)
    {
        logger.LogInformation("Sending {Count} customers to Paradigm API...", customers.Count);
        
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/Customer/batch-create", customers);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<List<BaseCustomer>>();
            
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

    public async Task BatchCreateCustomerContactsAsync(string jsonPath)
    {
        logger.LogInformation("Reading customer contacts from {Path}", jsonPath);
        
        var contacts = await fileService.ReadFromJsonAsync<List<CustomerContact>>(jsonPath);
        if (contacts == null || contacts.Count == 0)
        {
            logger.LogError("No customer contacts found in {Path}", jsonPath);
            throw new InvalidOperationException("No customer contacts found");
        }

        logger.LogInformation("Sending {Count} customer contacts to Paradigm API...", contacts.Count);
        
        try
        {
            var response = await httpClient.PostAsJsonAsync($"{BaseUrl}/CustomerContact/batch-create", contacts);
            response.EnsureSuccessStatusCode();
            
            logger.LogInformation("Successfully created {Count} customer contacts in Paradigm", contacts.Count);
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

    public async Task DeleteCustomerAsync(string customerId)
    {
        logger.LogInformation("Deleting customer {CustomerId} from Paradigm...", customerId);
        
        try
        {
            var response = await httpClient.DeleteAsync($"{BaseUrl}/CustomerContact/{customerId}");
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

