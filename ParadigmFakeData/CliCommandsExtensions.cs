using System.CommandLine;
using ParadigmFakeData.Services;

namespace ParadigmFakeData;

internal static class CliCommandsExtensions
{
    internal static RootCommand RegisterCommands(IWorkflowOrchestrator service)
    {
        return CreateRootCommand().RegisterDeleteCustomersCommand(service)
            .RegisterGenerateCustomersStep(service)
            .RegisterPostCustomersStep(service)
            .RegisterGenerateContactsStep(service)
            .RegisterPostContactsStep(service)
            .RegisterGenerateAndPostOpportunitiesStep(service);
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
            new Argument<FileInfo>("customersJsonPath", "Path to the customers JSON file"),
            new Argument<DirectoryInfo>("outputPath", "Directory to save the output")
        };

        postCustomersCommand.SetHandler(async (customersJsonPath, outputPath) =>
                await service.PostCustomersAsync(customersJsonPath.FullName, outputPath.FullName),
            postCustomersCommand.Arguments.Cast<Argument<FileInfo>>().First(),
            postCustomersCommand.Arguments.Cast<Argument<DirectoryInfo>>().Skip(1).First());

        rootCommand.AddCommand(postCustomersCommand);
        return rootCommand;
    }

    private static RootCommand RegisterGenerateContactsStep(this RootCommand rootCommand, IWorkflowOrchestrator service)
    {
        var generateContactsCommand = new Command("generate-contacts", "Generate contacts and save to JSON file")
        {
            new Argument<FileInfo>("customersJsonPath", "Path to the customers JSON file"),
            new Argument<DirectoryInfo>("outputPath", "Directory to save the generated contacts JSON file")
        };

        generateContactsCommand.SetHandler(async (customersJsonPath, outputPath) =>
                await service.GenerateCustomerContactsAsync(customersJsonPath.FullName, outputPath.FullName),
            generateContactsCommand.Arguments.Cast<Argument<FileInfo>>().First(),
            generateContactsCommand.Arguments.Cast<Argument<DirectoryInfo>>().Skip(1).First());

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
                new Argument<FileInfo>("customerJsonPath", "Path to the customers JSON file"),
                new Argument<DirectoryInfo>("outputPath", "Directory to save the output")
            };

        generateAndPostOpportunitiesCommand.SetHandler(async (customerJsonPath, outputPath) =>
                await service.GenerateOpportunitiesAsync(customerJsonPath.FullName, outputPath.FullName),
            generateAndPostOpportunitiesCommand.Arguments.Cast<Argument<FileInfo>>().First(),
            generateAndPostOpportunitiesCommand.Arguments.Cast<Argument<DirectoryInfo>>().First());

        rootCommand.AddCommand(generateAndPostOpportunitiesCommand);
        return rootCommand;
    }
}