using HubSpotFakeData.Models;

namespace HubSpotFakeData.Services;

/// <summary>
/// Service for generating fake data
/// </summary>
public interface IDataGenerationService
{
    GenerationResult Generate(GenerationMode mode);
}

