namespace ParadigmFakeData.Services;

public interface IWorkflowOrchestrator
{
    Task RunWorkflowAsync();
    Task DeleteCustomersAsync(string jsonPath);
}