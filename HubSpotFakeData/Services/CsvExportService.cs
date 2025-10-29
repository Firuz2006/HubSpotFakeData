using System.Text;
using HubSpotFakeData.Models;
using Microsoft.Extensions.Logging;

namespace HubSpotFakeData.Services;

public class CsvExportService(ILogger<CsvExportService> logger) : ICsvExportService
{
    private const string CompanyContactHeader =
        "Company Domain Name <COMPANY domain>,Company name <COMPANY name>,Address <COMPANY address>,City <COMPANY city>," +
        "State/Region <COMPANY state>,Postal Code <COMPANY zip>,Phone Number <COMPANY phone>," +
        "Email <CONTACT email>,First Name <CONTACT firstname>,Last Name <CONTACT lastname>,Address <CONTACT address>," +
        "City <CONTACT city>,State/Region <CONTACT state>,Postal Code <CONTACT zip>,Phone Number <CONTACT phone>";

    private const string CompanyDealHeader =
        "Company Domain Name <COMPANY domain>,Deal Stage <DEAL dealstage>,Pipeline <DEAL pipeline>,Deal Name <DEAL dealname>," +
        "Description <DEAL description>,Amount <DEAL amount>,Close Date <DEAL closedate>";

    private const string ContactDealHeader =
        "Email <CONTACT email>,Deal Stage <DEAL dealstage>,Pipeline <DEAL pipeline>,Deal Name <DEAL dealname>," +
        "Description <DEAL description>,Amount <DEAL amount>,Close Date <DEAL closedate>";

    public async Task<string> ExportCompanyContactsToCsvAsync(List<CsvCompanyContact> rows, string timestamp)
    {
        if (rows == null || rows.Count == 0)
        {
            throw new ArgumentException("No rows to export", nameof(rows));
        }

        var filePath = GetCompanyContactsPath(timestamp);
        logger.LogInformation("Exporting {RowCount} company-contact associations to CSV at {FilePath}", rows.Count, filePath);

        var csv = new StringBuilder();
        csv.AppendLine(CompanyContactHeader);

        foreach (var row in rows)
        {
            var line =
                $"{EscapeCsvField(row.CompanyDomain)},{EscapeCsvField(row.CompanyName)},{EscapeCsvField(row.CompanyAddress)}," +
                $"{EscapeCsvField(row.CompanyCity)},{EscapeCsvField(row.CompanyState)},{EscapeCsvField(row.CompanyZip)}," +
                $"{EscapeCsvField(row.CompanyPhone)}," +
                $"{EscapeCsvField(row.ContactEmail)},{EscapeCsvField(row.ContactFirstName)},{EscapeCsvField(row.ContactLastName)}," +
                $"{EscapeCsvField(row.ContactAddress)},{EscapeCsvField(row.ContactCity)},{EscapeCsvField(row.ContactState)}," +
                $"{EscapeCsvField(row.ContactZip)},{EscapeCsvField(row.ContactPhone)}";
            csv.AppendLine(line);
        }

        await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8);
        logger.LogInformation("Successfully exported CSV to {FilePath}", filePath);
        return filePath;
    }

    public async Task<string> ExportCompanyDealsToCsvAsync(List<CsvCompanyDeal> rows, string timestamp)
    {
        if (rows == null || rows.Count == 0)
        {
            throw new ArgumentException("No rows to export", nameof(rows));
        }

        var filePath = GetCompanyDealsPath(timestamp);
        logger.LogInformation("Exporting {RowCount} company-deal associations to CSV at {FilePath}", rows.Count, filePath);

        var csv = new StringBuilder();
        csv.AppendLine(CompanyDealHeader);

        foreach (var row in rows)
        {
            var line =
                $"{EscapeCsvField(row.CompanyDomain)},{EscapeCsvField(row.DealStage)},{EscapeCsvField(row.DealPipeline)}," +
                $"{EscapeCsvField(row.DealName)},{EscapeCsvField(row.DealDescription)}," +
                $"{row.DealAmount.ToString(System.Globalization.CultureInfo.InvariantCulture)},{row.DealCloseDate:dd/MM/yyyy}";
            csv.AppendLine(line);
        }

        await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8);
        logger.LogInformation("Successfully exported CSV to {FilePath}", filePath);
        return filePath;
    }

    public async Task<string> ExportContactDealsToCsvAsync(List<CsvContactDeal> rows, string timestamp)
    {
        if (rows == null || rows.Count == 0)
        {
            throw new ArgumentException("No rows to export", nameof(rows));
        }

        var filePath = GetContactDealsPath(timestamp);
        logger.LogInformation("Exporting {RowCount} contact-deal associations to CSV at {FilePath}", rows.Count, filePath);

        var csv = new StringBuilder();
        csv.AppendLine(ContactDealHeader);

        foreach (var row in rows)
        {
            var line =
                $"{EscapeCsvField(row.ContactEmail)},{EscapeCsvField(row.DealStage)},{EscapeCsvField(row.DealPipeline)}," +
                $"{EscapeCsvField(row.DealName)},{EscapeCsvField(row.DealDescription)}," +
                $"{row.DealAmount.ToString(System.Globalization.CultureInfo.InvariantCulture)},{row.DealCloseDate:dd/MM/yyyy}";
            csv.AppendLine(line);
        }

        await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8);
        logger.LogInformation("Successfully exported CSV to {FilePath}", filePath);
        return filePath;
    }

    private static string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field))
        {
            return string.Empty;
        }

        if (field.Contains(',') || field.Contains('"') || field.Contains('\n') || field.Contains('\r'))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return field;
    }

    private static string GetCompanyContactsPath(string timestamp)
    {
        var outputDirectory = GetOutputDirectory();
        return Path.Combine(outputDirectory, $"hubspot_company_contacts_{timestamp}.csv");
    }

    private static string GetCompanyDealsPath(string timestamp)
    {
        var outputDirectory = GetOutputDirectory();
        return Path.Combine(outputDirectory, $"hubspot_company_deals_{timestamp}.csv");
    }

    private static string GetContactDealsPath(string timestamp)
    {
        var outputDirectory = GetOutputDirectory();
        return Path.Combine(outputDirectory, $"hubspot_contact_deals_{timestamp}.csv");
    }

    private static string GetOutputDirectory()
    {
        var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "output");
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        return outputDir;
    }
}