# Quick Start Guide - Paradigm Fake Data Generator

## Prerequisites

- .NET 9.0 SDK installed
- Access to Paradigm API at `http://192.168.1.130:5001`

## First Time Setup

```cmd
cd C:\Users\user\RiderProjects\HubSpotFakeData\ParadigmFakeData
dotnet restore
dotnet build
```

## Running the Application

### Generate and Post Data (Interactive Workflow)

```cmd
dotnet run
```

You'll be guided through 4 steps with prompts at each stage:

**Step 1:** Generate Customers

- ✓ Output: `output_TIMESTAMP/customers.json` (900 customers)
- ❓ Prompt: Review and approve to post to Paradigm

**Step 2:** Post to Paradigm & Update IDs

- ✓ Output: `output_TIMESTAMP/customers_updated.json` (with API IDs)
- ❓ Prompt: Approve to generate contacts

**Step 3:** Generate Customer Contacts

- ✓ Output: `output_TIMESTAMP/customer_contacts.json`
- ❓ Prompt: Approve to post contacts

**Step 4:** Post Contacts to Paradigm

- ✓ Complete!

### Delete Customers

```cmd
dotnet run --delete "output_20251103_143022\customers_updated.json"
```

## Example Session

```
╔════════════════════════════════════════╗
║   Paradigm Fake Data Generator         ║
╚════════════════════════════════════════╝

info: ParadigmFakeData.Services.WorkflowOrchestrator[0]
      === STEP 1: GENERATE CUSTOMERS ===

✓ Customers generated successfully!
📄 File: file:///C:/Users/user/RiderProjects/HubSpotFakeData/ParadigmFakeData/output_20251103_143022/customers.json

Did you review it and can we post it to Paradigm? (y/n): y

info: ParadigmFakeData.Services.WorkflowOrchestrator[0]
      === STEP 2: POST CUSTOMERS TO PARADIGM ===

✓ Customers posted and updated successfully!
📄 Updated file: file:///C:/Users/user/RiderProjects/HubSpotFakeData/ParadigmFakeData/output_20251103_143022/customers_updated.json

Do you want to continue and create customer contacts? (y/n): y

...
```

## Output Files

All files saved to timestamped directory: `output_YYYYMMDD_HHMMSS/`

### customers.json

```json
[
  {
    "strCustomerId": null,
    "strCompanyName": "Acme Corp",
    "strPrimaryWebSite": "https://acme.example.com"
  },
  {
    "strCustomerId": null,
    "strFirstName": "John",
    "strLastName": "Doe",
    "strPrimaryEmail": "john.doe@example.com",
    "strPrimaryPhone": "+1-555-0100"
  }
]
```

### customers_updated.json

```json
[
  {
    "strCustomerId": "CUST-001",
    "strCompanyName": "Acme Corp",
    "strPrimaryWebSite": "https://acme.example.com"
  }
]
```

### customer_contacts.json

```json
[
  {
    "strCustomerID": "CUST-001",
    "strFirstName": "Jane",
    "strLastName": "Smith",
    "strPrimaryEmail": "jane.smith@example.com",
    "strPrimaryPhone": "+1-555-0200"
  }
]
```

## Tips

- Review JSON files by clicking the `file:///` links in console
- Type 'n' at any prompt to safely cancel workflow
- Keep `customers_updated.json` for delete operations
- Each run creates new timestamped directory
- All emails, phones, company names, and URLs are unique

## Troubleshooting

**Cannot connect to API**

- Verify Paradigm API is running at `http://192.168.1.130:5001`
- Check network connectivity

**Build errors**

```cmd
dotnet clean
dotnet restore
dotnet build
```

**No output directory created**

- Check write permissions in project directory
- Verify disk space available

