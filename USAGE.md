# HubSpot Fake Data Generator - Usage Guide

## Overview

This application generates realistic HubSpot CSV import data with proper associations between Companies, Contacts, and Deals using the Bogus library.

## Architecture

The application follows clean architecture principles with:

- **Domain Models**: Company, Contact, Deal, CsvRow
- **Services**:
  - `DataGenerationService`: Generates fake data using Bogus
  - `CsvExportService`: Exports data to CSV format
  - `AssociationManager`: Manages relationships between entities
- **Dependency Injection**: Uses Microsoft.Extensions.DependencyInjection
- **Logging**: Comprehensive logging with Microsoft.Extensions.Logging

## Association Rules

### Critical Association Rules (HubSpot Import Behavior)

1. **One-to-Many: Deal → Company**
   - Each Deal belongs to exactly ONE Company
   - Each Company can have MULTIPLE Deals

2. **One-to-Many: Deal → Contact**
   - Each Deal belongs to exactly ONE Contact
   - Each Contact can have MULTIPLE Deals

3. **Many-to-Many: Company ↔ Contact**
   - Each Company can have MULTIPLE Contacts
   - Each Contact can be associated with MULTIPLE Companies

### HubSpot CSV Deduplication

- Same email = same Contact (won't duplicate)
- Same domain = same Company (won't duplicate)
- Each unique deal name = new Deal
- Everything in same row gets associated together

## Usage

### Test Mode (20 records)

Generates 20 CSV rows demonstrating all association patterns:

```bash
dotnet run --project HubSpotFakeData -- --mode test
```

**Test Mode Demonstrates:**
- 1 Company with multiple Deals (at least 3 deals)
- 1 Contact with multiple Deals (at least 3 deals)
- 1 Contact associated with multiple Companies (at least 2 companies)
- Cross-company associations (Contact from Company A working on Company B's deal)

**Expected Output:**
- 3 Companies
- 4 Contacts
- 20 Deals
- Multiple many-to-many associations

### Production Mode (10,000 records)

Generates 10,000 CSV rows with realistic distribution:

```bash
dotnet run --project HubSpotFakeData -- --mode production
```

**Production Mode Statistics:**
- ~650 unique Companies
- ~2,500 unique Contacts
- 10,000 unique Deals (one per row)
- Average 3-5 Contacts per Company
- Average 4 Deals per Contact
- 20-30% of Contacts associated with multiple Companies

### Default Mode

Running without arguments defaults to Test mode:

```bash
dotnet run --project HubSpotFakeData
```

## Output

CSV files are generated in the `output/` directory with timestamp:

```
output/hubspot_import_test_YYYYMMDD_HHMMSS.csv
output/hubspot_import_production_YYYYMMDD_HHMMSS.csv
```

## CSV Format

The generated CSV includes HubSpot object property tags:

```csv
Company Domain Name <COMPANY domain>,Company name <COMPANY name>,Email <CONTACT email>,First Name <CONTACT firstname>,Last Name <CONTACT lastname>,Deal Stage <DEAL dealstage>,Pipeline <DEAL pipeline>,Deal Name <DEAL dealname>
```

## Data Generation Details

### Company Data (Bogus)
- Domain: `faker.Internet.DomainName()`
- Name: `faker.Company.CompanyName()`

### Contact Data (Bogus)
- Email: `faker.Internet.Email()`
- First Name: `faker.Name.FirstName()`
- Last Name: `faker.Name.LastName()`

### Deal Data (Bogus)
- Name: `{ProductName} - {Department} #{Index}`
- Stage: Random from available stages
- Pipeline: "default"

### Deal Stages
- appointmentscheduled
- qualifiedtobuy
- presentationscheduled
- decisionmakerboughtin
- contractsent
- closedwon
- closedlost

## Statistics Logging

The application provides detailed statistics after generation:

**Test Mode:**
- Shows each company's contact and deal counts
- Shows each contact's company and deal counts

**Production Mode:**
- Total counts for companies, contacts, deals
- Percentage of contacts with multiple companies
- Average contacts per company
- Average deals per contact
- Average deals per company

## Examples

### Example Test Mode Output

```
info: Total Companies: 3
info: Total Contacts: 4
info: Total Deals: 20
info: Company 'Acme Corp': 3 contacts, 7 deals
info: Contact 'john@example.com': 2 companies, 7 deals
```

### Example Production Mode Output

```
info: Total Companies: 650
info: Total Contacts: 2500
info: Total Deals: 10000
info: Contacts with multiple companies: 1023 (40.9%)
info: Average contacts per company: 5.41
info: Average deals per contact: 4.00
info: Average deals per company: 15.38
```

## Validation

The generated CSV ensures:
- Exact column headers with `<OBJECT property>` tags
- Proper deduplication (same email = same contact, same domain = same company)
- Many-to-many Company-Contact associations
- One-to-many Deal-Contact associations
- One-to-many Deal-Company associations

## Building the Project

```bash
dotnet restore
dotnet build
```

## Running the Project

```bash
# Test mode
dotnet run --project HubSpotFakeData -- --mode test

# Production mode
dotnet run --project HubSpotFakeData -- --mode production
```

## Requirements

- .NET 9.0
- Bogus (35.6.5)
- Microsoft.Extensions.DependencyInjection (9.0.0)
- Microsoft.Extensions.Logging (9.0.0)
- Microsoft.Extensions.Logging.Console (9.0.0)

