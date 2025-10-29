using System.Text;
using HubSpotFakeData.Models;
using Microsoft.Extensions.Logging;

namespace HubSpotFakeData.Services;

/// <summary>
/// Service for exporting data to CSV files
/// </summary>
public class CsvExportService(ILogger<CsvExportService> logger) : ICsvExportService
{
    private readonly ILogger<CsvExportService> _logger = logger;

    /// <summary>
    /// Exports CSV rows to a file with proper HubSpot format
    /// </summary>
    public async Task ExportToCsvAsync(List<CsvRow> rows, string filePath)
    {
        if (rows == null || rows.Count == 0)
        {
            throw new ArgumentException("No rows to export", nameof(rows));
        }

        _logger.LogInformation("Exporting {Count} rows to {FilePath}", rows.Count, filePath);

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            _logger.LogInformation("Created directory: {Directory}", directory);
        }

        var csv = new StringBuilder();

        // Write headers with HubSpot object property tags
        csv.AppendLine("Company Domain Name <COMPANY domain>,Company name <COMPANY name>,Address <COMPANY address>,City <COMPANY city>,State/Region <COMPANY state>,Postal Code <COMPANY zip>,Phone Number <COMPANY phone>,Email <CONTACT email>,First Name <CONTACT firstname>,Last Name <CONTACT lastname>,Address <CONTACT address>,City <CONTACT city>,State/Region <CONTACT state>,Postal Code <CONTACT zip>,Phone Number <CONTACT phone>,Deal Stage <DEAL dealstage>,Pipeline <DEAL pipeline>,Deal Name <DEAL dealname>,Description <DEAL description>,Amount <DEAL amount>,Close Date <DEAL closedate>");

        // Write data rows
        foreach (var row in rows)
        {
            var line = $"{EscapeCsvField(row.CompanyDomain)},{EscapeCsvField(row.CompanyName)},{EscapeCsvField(row.CompanyAddress)},{EscapeCsvField(row.CompanyCity)},{EscapeCsvField(row.CompanyState)},{EscapeCsvField(row.CompanyZip)},{EscapeCsvField(row.CompanyPhone)},{EscapeCsvField(row.ContactEmail)},{EscapeCsvField(row.ContactFirstName)},{EscapeCsvField(row.ContactLastName)},{EscapeCsvField(row.ContactAddress)},{EscapeCsvField(row.ContactCity)},{EscapeCsvField(row.ContactState)},{EscapeCsvField(row.ContactZip)},{EscapeCsvField(row.ContactPhone)},{EscapeCsvField(row.DealStage)},{EscapeCsvField(row.DealPipeline)},{EscapeCsvField(row.DealName)},{EscapeCsvField(row.DealDescription)},{row.DealAmount.ToString(System.Globalization.CultureInfo.InvariantCulture)},{row.DealCloseDate:dd/MM/yyyy}";
            csv.AppendLine(line);
        }

        await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8);

        _logger.LogInformation("Successfully exported CSV to {FilePath}", filePath);
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
}

