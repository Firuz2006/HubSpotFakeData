namespace ParadigmFakeData;

internal static class CommandConstants
{
    // Root
    public const string RootDescription = "Paradigm Fake Data Generator";

    // Customer commands
    public const string GenerateCustomersName = "generate-customers";
    public const string GenerateCustomersDescription = "Generate customers and save to JSON file";
    public const string PostCustomersName = "post-customers";
    public const string PostCustomersDescription = "Post customers from JSON file to Paradigm";
    public const string DeleteCustomersName = "delete-customers";
    public const string DeleteCustomersDescription = "Generate SQL to delete customers from Paradigm";

    // Contact commands
    public const string GenerateContactsName = "generate-contacts";
    public const string GenerateContactsDescription = "Generate contacts for customers and save to JSON file";
    public const string PostContactsName = "post-contacts";
    public const string PostContactsDescription = "Post contacts from JSON file to Paradigm";
    public const string DeleteCustomerContactsName = "delete-contacts";

    public const string DeleteCustomerContactsDescription =
        "Generate SQL to delete customer contacts from Paradigm using a JSON file";

    // Opportunity commands
    public const string GenerateAndPostOpportunitiesName = "generate-post-opportunities";

    public const string GenerateAndPostOpportunitiesDescription =
        "Generate opportunities for customers and save to JSON file";

    public const string DeleteOpportunitiesName = "delete-opportunities";

    public const string DeleteOpportunitiesDescription =
        "Generate SQL to delete opportunities from Paradigm using a JSON file";

    // Common argument names and descriptions
    public const string ArgPath = "path";
    public const string ArgPathDescription = "Path to the JSON file";
    public const string ArgOutputPath = "outputPath";
    public const string ArgOutputPathDescription = "Directory to save the generated JSON file";
    public const string ArgCustomersJsonPath = "customersJsonPath";
    public const string ArgCustomersJsonPathDescription = "Path to the customers JSON file";
    public const string ArgContactsJsonPath = "contactsJsonPath";
    public const string ArgContactsJsonPathDescription = "Path to the contacts JSON file";
    public const string ArgCustomerJsonPath = "customerJsonPath";
    public const string ArgCustomerJsonPathDescription = "Path to the customers JSON file";
}