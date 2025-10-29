using HubSpotFakeData.Models;

namespace HubSpotFakeData.Services;

/// <summary>
/// Service for exporting data to CSV files
/// </summary>
public interface ICsvExportService
{
    /// <summary>
    /// Exports CSV rows to a file
    /// </summary>
    Task ExportToCsvAsync(List<CsvRow> rows, string filePath);
}

