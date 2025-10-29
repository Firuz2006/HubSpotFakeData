using HubSpotFakeData.Models;

namespace HubSpotFakeData.Services;

/// <summary>
/// Service for exporting data to CSV files
/// </summary>
public interface ICsvExportService
{
    Task<string> ExportCompaniesToCsvAsync(List<CsvCompany> rows, string timestamp);

    Task<string> ExportContactsToCsvAsync(List<CsvContact> rows, string timestamp);
}