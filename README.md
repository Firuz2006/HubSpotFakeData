# HubSpot Fake Data Generator

Comprehensive CSV data generator for HubSpot entities (Companies, Contacts, Deals) with extensive combination testing capabilities.

## Features

- **HubSpot Import Format**: Combined CSV with all entities (Contacts, Companies, Deals) in single file matching HubSpot's import template
- **Three Entity Types**: Individual CSV files for Companies, Contacts, Deals
- **Comprehensive Test Coverage**: All property combinations and edge cases
- **Configurable Generation**: Control nulls, empty strings, data quality
- **Multiple Scenarios**: Standard, minimal, complete, mixed quality datasets
- **Size Variations**: From 10 to 10,000+ records per file
- **Realistic Data**: Uses Bogus library for authentic fake data

## Quick Start

### Generate All Combinations (Default Mode)
Generates 40+ CSV files covering all scenarios:

```cmd
dotnet run
```

Output directory: `.\output`

### Custom Generation Mode

Generate specific entities with custom parameters:

```cmd
dotnet run [output_dir] --custom [entity_type] [count] [seed] [null_prob] [empty_prob]
```

**Examples:**

```cmd
# Generate HubSpot import format with 1000 records
dotnet run output --custom combined 1000

# Generate 5000 companies with default settings
dotnet run output --custom companies 5000

# Generate 2000 contacts with high null probability (40%)
dotnet run output --custom contacts 2000 12345 0.4 0.2

# Generate all entities with 10000 records each
dotnet run output --custom all 10000 99999 0.15 0.1

# Generate deals with minimal nulls
dotnet run output --custom deals 3000 54321 0.02 0.01
```

**Parameters:**
- `output_dir`: Output directory path (default: `.\output`)
- `entity_type`: `combined`/`hubspot`/`import`, `companies`, `contacts`, `deals`, or `all` (default: `all`)
- `count`: Number of records to generate (default: `1000`)
- `seed`: Random seed for reproducibility (default: `12345`)
- `null_prob`: Probability of null values 0.0-1.0 (default: `0.1`)
- `empty_prob`: Probability of empty strings 0.0-1.0 (default: `0.1`)

## Generated File Types

### All Combinations Mode

The generator creates multiple file categories:

#### HubSpot Import Format Files (Combined Entity CSV)
- `hubspot_import_standard.csv` (1,000 records) - Standard quality combined file
- `hubspot_import_complete.csv` (500 records) - Nearly complete data
- `hubspot_import_sample_50.csv` (50 records) - Small test sample
- `hubspot_import_large_5000.csv` (5,000 records) - Large dataset
- `hubspot_import_mixed_quality.csv` (800 records) - Mixed quality data

#### Standard Files (Normal Distribution)
- `companies_standard.csv` (1,000 records)
- `contacts_standard.csv` (2,000 records)
- `deals_standard.csv` (1,500 records)

#### Edge Cases
- `*_no_nulls.csv` - All optional fields populated
- `*_high_nulls.csv` - 40% null, 30% empty strings
- `*_no_headers.csv` - CSV without header row
- `*_minimal.csv` - 80% nulls (tests required fields only)
- `*_complete.csv` - Nearly all fields populated (2% nulls)
- `*_mixed.csv` - Realistic mixed quality data (25% nulls)

#### Size Variations
- `*_tiny_10.csv` - 10 records
- `*_small_100.csv` - 100 records
- `*_large_5000.csv` - 5,000 records
- `*_xlarge_10000.csv` - 10,000 records

#### Data Variants
- `*_variant_1.csv` through `*_variant_5.csv` - Different seeds for variety

**Total: 47 CSV files per full generation (5 HubSpot import format + 42 individual entity files)**

## Entity Schemas

### HubSpot Import Format (Combined CSV)

Single CSV file with all entity types, following HubSpot's multi-object import template:

**Contact Fields (prefixed with `<CONTACT>`)**
- Email, First Name, Last Name, Phone Number
- Address, City, State/Region, Postal Code, Country/Region
- Salutation, Job Title, Degree, School, Seniority
- Lifecycle Stage, Contact Owner

**Company Fields (prefixed with `<COMPANY>`)**
- Company Domain Name, Company Name, Company Phone Number
- Company Street Address, Company Street Address 2
- Company City, Company State/Region, Company Postal Code, Company Country/Region
- Type, Annual Revenue, Number of Employees
- Industry, Description, Website
- Company Lifecycle Stage, Company Owner

**Deal Fields (prefixed with `<DEAL>`)**
- Deal Stage, Pipeline, Deal Name, Amount
- Close Date, Create Date, Deal Description
- Deal Owner

Each row can contain any combination of Contact, Company, and Deal data. HubSpot will automatically associate related records.

### Company Properties
- `hubspot_owner_id` (required)
- `external_id` (required)
- `type` - Company type (PROSPECT, PARTNER, RESELLER, VENDOR, OTHER)
- `address`, `address2`, `city`, `state`, `zip`, `country`
- `annualrevenue` - Numeric revenue
- `description` - Business description
- `domain`, `website`, `phone`
- `name` - Company name
- `lifecyclestage` - Sales lifecycle stage
- `ready_to_sync_company_to_paradigm` - Sync flag

### Contact Properties
- `hubspot_owner_id` (required)
- `external_id` (required)
- `is_paradigm_customer` - Boolean flag
- `firstname`, `lastname`, `email`, `phone`
- `address`, `city`, `state`, `zip`, `country`
- `company` - Associated company name
- `salutation` - Mr., Mrs., Dr., etc.
- `degree` - Education level
- `school` - Educational institution
- `seniority` - Job level
- `photo` - Photo URL
- `website`
- `lifecyclestage`
- `ready_to_sync_contact_to_paradigm` - Sync flag

### Deal Properties
- `dealstage` (required) - Stage ID
- `hubspot_owner_id` (required)
- `external_id` (required)
- `amount` - Deal value (decimal)
- `closedate` - Expected/actual close date
- `createdate` - Deal creation date
- `dealname` - Deal title
- `description` - Deal details

**Deal Stages:**
- appointmentscheduled
- qualifiedtobuy
- presentationscheduled
- decisionmakerboughtin
- contractsent
- closedwon
- closedlost

## Data Quality Features

### Null Handling
- Configurable null probability per generation
- Respects DefaultValue attributes
- Required fields never null

### Empty String Handling
- Separate probability from nulls
- Useful for testing validation logic

### Realistic Data Patterns
- Valid email formats
- Proper phone number formats
- Real company/person names via Bogus
- Logical date relationships (create before close)
- Realistic revenue distributions

### Edge Case Coverage
- Special characters in names/descriptions
- CSV escaping (commas, quotes, newlines)
- Mixed data quality scenarios
- Boundary values

## Project Structure

```
HubSpotFakeData/
├── Entities/
│   ├── HubSpotCompany.cs
│   ├── HubSpotContact.cs
│   └── HubSpotDeal.cs
├── Services/
│   ├── ICsvGenerator.cs
│   ├── CompanyCsvGenerator.cs
│   ├── ContactCsvGenerator.cs
│   ├── DealCsvGenerator.cs
│   ├── CombinedHubSpotCsvGenerator.cs
│   └── CombinationGenerator.cs
└── Program.cs
```

## Dependencies

- .NET 9.0
- Bogus 35.6.5 (Fake data generation)

## Use Cases

- **Import Testing**: Test HubSpot import functionality
- **Data Validation**: Verify field validation rules
- **Performance Testing**: Large dataset processing
- **Edge Case Testing**: Null handling, special characters
- **Integration Testing**: End-to-end data pipeline tests
- **Demo Data**: Sample data for demonstrations

## Performance

Generation speeds (approximate):
- 1,000 records: ~50ms per entity
- 10,000 records: ~500ms per entity
- Full combination suite (42 files): ~15-20 seconds

## CSV Format

- UTF-8 encoding
- Comma-separated values
- Quoted fields containing commas, quotes, or newlines
- ISO 8601 date format (yyyy-MM-dd)
- Invariant culture for numbers

