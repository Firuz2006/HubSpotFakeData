using HubSpotFakeData.Models;

namespace HubSpotFakeData.Services;

/// <summary>
/// Service for generating fake data
/// </summary>
public interface IDataGenerationService
{
    /// <summary>
    /// Generates CSV rows based on the specified mode
    /// </summary>
    List<CsvRow> Generate(GenerationMode mode);
}

