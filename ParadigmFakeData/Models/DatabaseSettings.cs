namespace ParadigmFakeData.Models;

public class DatabaseSettings
{
    public string CustomersTableName { get; set; } = "Customers";
    public string CustomersIdColName { get; set; } = "CustomerId";

    public string OpportunitiesTableName { get; set; } = "Opportunities";
    public string OpportunitiesNameColName { get; set; } = "OpportunityId";

    public string CustomerContactTableName { get; set; } = "CustomerContacts";
    public string CustomerContactIdColName { get; set; } = "ContactId";
}