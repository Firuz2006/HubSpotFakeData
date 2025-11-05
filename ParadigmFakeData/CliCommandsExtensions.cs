using System.CommandLine;
using ParadigmFakeData.Services;

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
            .RegisterPostOpportunitiesStep(service)
            .RegisterDeleteOpportunitiesCommand(service);
    }

    private static RootCommand CreateRootCommand()
    {
        return new RootCommand("Paradigm Fake Data Generator");
    }

    private static RootCommand RegisterDeleteCustomersCommand(this RootCommand rootCommand,
        IWorkflowOrchestrator service)
    {
        var deleteCommand = new Command("delete-customers", "Delete1 customers from Paradigm using a JSON file")
        {
            new Argument<FileInfo>("path", "Path to the customers JSON file")
        };

        deleteCommand.SetHandler(async path => await service.GetDeleteCustomersSqlQueryAsync(path.FullName),
            deleteCommand.Arguments.Cast<Argument<FileInfo>>().First());

        rootCommand.AddCommand(deleteCommand);
        return rootCommand;
    }

    private static RootCommand RegisterGenerateCustomersStep(this RootCommand rootCommand,
        IWorkflowOrchestrator service)
    {
        var generateCustomersCommand = new Command("generate-customers", "Generate customers and save to JSON file")
        {
            new Argument<DirectoryInfo>("outputPath", "Directory to save the generated customers JSON file")
        };

        generateCustomersCommand.SetHandler(async outputPath =>
                await service.GenerateCustomersAsync(outputPath.FullName),
            generateCustomersCommand.Arguments.Cast<Argument<DirectoryInfo>>().First());

        rootCommand.AddCommand(generateCustomersCommand);
        return rootCommand;
    }

    private static RootCommand RegisterPostCustomersStep(this RootCommand rootCommand, IWorkflowOrchestrator service)
    {
        var postCustomersCommand = new Command("post-customers", "Post customers from JSON file to Paradigm")
        {
            new Argument<FileInfo>("customersJsonPath", "Path to the customers JSON file")
        };

        postCustomersCommand.SetHandler(async (customersJsonPath) =>
                await service.PostCustomersAsync(customersJsonPath.FullName),
            postCustomersCommand.Arguments.Cast<Argument<FileInfo>>().First());

        rootCommand.AddCommand(postCustomersCommand);
        return rootCommand;
    }

    private static RootCommand RegisterGenerateContactsStep(this RootCommand rootCommand, IWorkflowOrchestrator service)
    {
        var generateContactsCommand = new Command("generate-contacts", "Generate contacts and save to JSON file")
        {
            new Argument<FileInfo>("customersJsonPath", "Path to the customers JSON file")
        };

        generateContactsCommand.SetHandler(async customersJsonPath =>
                await service.GenerateCustomerContactsAsync(customersJsonPath.FullName),
            generateContactsCommand.Arguments.Cast<Argument<FileInfo>>().First());

        rootCommand.AddCommand(generateContactsCommand);
        return rootCommand;
    }

    private static RootCommand RegisterPostContactsStep(this RootCommand rootCommand, IWorkflowOrchestrator service)
    {
        var postContactsCommand = new Command("post-contacts", "Post contacts from JSON file to Paradigm")
        {
            new Argument<FileInfo>("contactsJsonPath", "Path to the contacts JSON file")
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
            new Command("generate-post-opportunities", "Generate and post opportunities to Paradigm")
            {
                new Argument<FileInfo>("customerJsonPath", "Path to the customers JSON file")
            };

        generateAndPostOpportunitiesCommand.SetHandler(async customerJsonPath =>
                await service.GenerateOpportunitiesAsync(customerJsonPath.FullName),
            generateAndPostOpportunitiesCommand.Arguments.Cast<Argument<FileInfo>>().First());

        rootCommand.AddCommand(generateAndPostOpportunitiesCommand);
        return rootCommand;
    }

    private static RootCommand RegisterPostOpportunitiesStep(this RootCommand rootCommand,
        IWorkflowOrchestrator service)
    {
        var postOpportunitiesCommand =
            new Command("post-opportunities", "Post opportunities from JSON file to Paradigm")
            {
                new Argument<FileInfo>("opportunitiesJsonPath", "Path to the opportunities JSON file")
            };

        postOpportunitiesCommand.SetHandler(async opportunitiesJsonPath =>
                await service.PostOpportunitiesAsync(opportunitiesJsonPath.FullName),
            postOpportunitiesCommand.Arguments.Cast<Argument<FileInfo>>().First());

        rootCommand.AddCommand(postOpportunitiesCommand);
        return rootCommand;
    }

    private static RootCommand RegisterDeleteCustomerContactsCommand(this RootCommand rootCommand,
        IWorkflowOrchestrator service)
    {
        var deleteContactsCommand = new Command("delete-customer-contacts",
            "Generate SQL to delete customer contacts from Paradigm using a JSON file")
        {
            new Argument<FileInfo>("path", "Path to the customer contacts JSON file")
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
        var deleteOpportunitiesCommand = new Command("delete-opportunities",
            "Generate SQL to delete opportunities from Paradigm using a JSON file")
        {
            new Argument<FileInfo>("path", "Path to the opportunities JSON file")
        };

        deleteOpportunitiesCommand.SetHandler(
            async path => await service.GetOpportunitiesDeleteSqlQueryAsync(path.FullName),
            deleteOpportunitiesCommand.Arguments.Cast<Argument<FileInfo>>().First());

        rootCommand.AddCommand(deleteOpportunitiesCommand);
        return rootCommand;
    }
}