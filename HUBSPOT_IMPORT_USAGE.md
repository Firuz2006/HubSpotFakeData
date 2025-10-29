# HubSpot Import Format Usage

## Quick Start

### Generate HubSpot-ready import file with 1000 records
```powershell
dotnet run output --custom combined 1000
```

This creates `output/hubspot_import_custom.csv` ready for direct import into HubSpot.

### Generate different quality datasets

**High-quality data (minimal nulls):**
```powershell
dotnet run output --custom combined 500 12345 0.02 0.01
```

**Mixed quality data (realistic):**
```powershell
dotnet run output --custom combined 1000 12345 0.25 0.15
```

**Large dataset for performance testing:**
```powershell
dotnet run output --custom combined 10000 12345 0.1 0.1
```

## HubSpot Import Steps

1. Generate the CSV file using one of the commands above
2. Log into your HubSpot account
3. Navigate to **Contacts** → **Contacts** → **Import**
4. Select **Start an import**
5. Choose **File from computer**
6. Upload the generated `hubspot_import_*.csv` file
7. HubSpot will automatically detect the column mappings from the `<CONTACT>`, `<COMPANY>`, `<DEAL>` prefixes
8. Review the mappings and click **Next**
9. Configure your import settings and click **Finish import**

## File Structure

Each row in the combined CSV can contain:
- **Contact data**: Person information (email, name, address, etc.)
- **Company data**: Organization information (domain, company name, revenue, etc.)
- **Deal data**: Sales opportunity information (stage, amount, close date, etc.)

HubSpot automatically:
- Creates/updates contacts based on email
- Creates/updates companies based on domain
- Creates deals and associates them with the contact/company
- Links contacts to companies based on the company domain

## Examples of Generated Data

### Row with Contact + Company + Deal
Full row with all three entity types, HubSpot will create all three records and link them.

### Row with only Contact
Just contact information, HubSpot will create/update the contact only.

### Row with Company + Deal
Company and deal information, HubSpot will create both and link the deal to the company.

## Default Mode (All Combinations)

Run without parameters to generate 47 CSV files including 5 HubSpot import formats:
```powershell
dotnet run
```

This creates:
- `hubspot_import_standard.csv` - 1,000 records, 10% nulls
- `hubspot_import_complete.csv` - 500 records, 2% nulls, nearly complete data
- `hubspot_import_sample_50.csv` - 50 records for quick testing
- `hubspot_import_large_5000.csv` - 5,000 records for volume testing
- `hubspot_import_mixed_quality.csv` - 800 records, 25% nulls, realistic data

Plus 42 individual entity CSV files for separate imports.

