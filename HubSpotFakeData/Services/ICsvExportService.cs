using HubSpotFakeData.Models;

namespace HubSpotFakeData.Services;

public interface ICsvExportService
{
    Task<string> ExportCompanyContactsToCsvAsync(List<CsvCompanyContact> rows, string timestamp);
    Task<string> ExportCompanyDealsToCsvAsync(List<CsvCompanyDeal> rows, string timestamp);
    Task<string> ExportContactDealsToCsvAsync(List<CsvContactDeal> rows, string timestamp);
}