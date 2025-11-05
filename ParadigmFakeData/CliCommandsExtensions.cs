using System.CommandLine;
using ParadigmFakeData.Services;
using static ParadigmFakeData.CommandConstants;

namespace ParadigmFakeData;

internal static class CliCommandsExtensions
{
    internal static RootCommand RegisterCommands(IWorkflowOrchestrator service)
    {
        return CreateRootCommand()
            // Customer
            .RegisterGenerateCustomersStep(service)
            .RegisterPostCustomersStep(service)
            .RegisterDeleteCustomersCommand(service)
            // Contact
            .RegisterGenerateContactsStep(service)
            .RegisterPostContactsStep(service)
            .RegisterDeleteCustomerContactsCommand(service)
            // Opportunity
            .RegisterGenerateAndPostOpportunitiesStep(service)
            .RegisterDeleteOpportunitiesCommand(service);
    }

    private static RootCommand CreateRootCommand()
    {
        return new RootCommand(RootDescription);
    }

    private static RootCommand RegisterDeleteCustomersCommand(this RootCommand rootCommand,
        IWorkflowOrchestrator service)
    {
        var deleteCommand = new Command(DeleteCustomersName, DeleteCustomersDescription)
        {
            new Argument<FileInfo>(ArgPath, ArgPathDescription)
        };

        deleteCommand.SetHandler(async path => await service.GetDeleteCustomersSqlQueryAsync(path.FullName),
            deleteCommand.Arguments.Cast<Argument<FileInfo>>().First());

        rootCommand.AddCommand(deleteCommand);
        return rootCommand;
    }

    private static RootCommand RegisterGenerateCustomersStep(this RootCommand rootCommand,
        IWorkflowOrchestrator service)
    {
        var generateCustomersCommand = new Command(GenerateCustomersName, GenerateCustomersDescription)
        {
            new Argument<DirectoryInfo>(ArgOutputPath, ArgOutputPathDescription)
        };

        generateCustomersCommand.SetHandler(async outputPath =>
                await service.GenerateCustomersAsync(outputPath.FullName),
            generateCustomersCommand.Arguments.Cast<Argument<DirectoryInfo>>().First());

        rootCommand.AddCommand(generateCustomersCommand);
        return rootCommand;
    }

    private static RootCommand RegisterPostCustomersStep(this RootCommand rootCommand, IWorkflowOrchestrator service)
    {
        var postCustomersCommand = new Command(PostCustomersName, PostCustomersDescription)
        {
            new Argument<FileInfo>(ArgCustomersJsonPath, ArgCustomersJsonPathDescription)
        };

        postCustomersCommand.SetHandler(async customersJsonPath =>
                await service.PostCustomersAsync(customersJsonPath.FullName),
            postCustomersCommand.Arguments.Cast<Argument<FileInfo>>().First());

        rootCommand.AddCommand(postCustomersCommand);
        return rootCommand;
    }

    private static RootCommand RegisterGenerateContactsStep(this RootCommand rootCommand, IWorkflowOrchestrator service)
    {
        var generateContactsCommand = new Command(GenerateContactsName, GenerateContactsDescription)
        {
            new Argument<FileInfo>(ArgCustomersJsonPath, ArgCustomersJsonPathDescription)
        };

        generateContactsCommand.SetHandler(async customersJsonPath =>
                await service.GenerateCustomerContactsAsync(customersJsonPath.FullName),
            generateContactsCommand.Arguments.Cast<Argument<FileInfo>>().First());

        rootCommand.AddCommand(generateContactsCommand);
        return rootCommand;
    }

    private static RootCommand RegisterPostContactsStep(this RootCommand rootCommand, IWorkflowOrchestrator service)
    {
        var postContactsCommand = new Command(PostContactsName, PostContactsDescription)
        {
            new Argument<FileInfo>(ArgContactsJsonPath, ArgContactsJsonPathDescription)
        };

        postContactsCommand.SetHandler(async contactsJsonPath =>
                await service.PostCustomerContactsAsync(contactsJsonPath.FullName),
            postContactsCommand.Arguments.Cast<Argument<FileInfo>>().First());

        rootCommand.AddCommand(postContactsCommand);
        return rootCommand;
    }

    private static RootCommand RegisterGenerateAndPostOpportunitiesStep(this RootCommand rootCommand,
        IWorkflowOrchestrator service)
    {
        var generateAndPostOpportunitiesCommand =
            new Command(GenerateAndPostOpportunitiesName, GenerateAndPostOpportunitiesDescription)
            {
                new Argument<FileInfo>(ArgCustomerJsonPath, ArgCustomerJsonPathDescription)
            };

        generateAndPostOpportunitiesCommand.SetHandler(async customerJsonPath =>
                await service.GenerateOpportunitiesAsync(customerJsonPath.FullName),
            generateAndPostOpportunitiesCommand.Arguments.Cast<Argument<FileInfo>>().First());

        rootCommand.AddCommand(generateAndPostOpportunitiesCommand);
        return rootCommand;
    }

    private static RootCommand RegisterDeleteCustomerContactsCommand(this RootCommand rootCommand,
        IWorkflowOrchestrator service)
    {
        var deleteContactsCommand = new Command(DeleteCustomerContactsName,
            DeleteCustomerContactsDescription)
        {
            new Argument<FileInfo>(ArgPath, ArgPathDescription)
        };

        deleteContactsCommand.SetHandler(
            async path => await service.GetDeleteCustomerContactsSqlQueryAsync(path.FullName),
            deleteContactsCommand.Arguments.Cast<Argument<FileInfo>>().First());

        rootCommand.AddCommand(deleteContactsCommand);
        return rootCommand;
    }

    private static RootCommand RegisterDeleteOpportunitiesCommand(this RootCommand rootCommand,
        IWorkflowOrchestrator service)
    {
        var deleteOpportunitiesCommand = new Command(DeleteOpportunitiesName,
            DeleteOpportunitiesDescription)
        {
            new Argument<FileInfo>(ArgPath, ArgPathDescription)
        };

        deleteOpportunitiesCommand.SetHandler(
            async path => await service.GetOpportunitiesDeleteSqlQueryAsync(path.FullName),
            deleteOpportunitiesCommand.Arguments.Cast<Argument<FileInfo>>().First());

        rootCommand.AddCommand(deleteOpportunitiesCommand);
        return rootCommand;
    }
}