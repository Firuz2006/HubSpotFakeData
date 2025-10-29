# HubSpot Fake Data Generator - 3-File Structure Implementation

## Overview
This implementation generates HubSpot-compatible CSV data in **3 separate files** to properly handle company-contact-deal associations.

## File Structure

### File 1: Company-Contact Associations
**Filename:** `hubspot_company_contacts_[timestamp].csv`
**Purpose:** Establishes many-to-many relationships between companies and contacts
**Columns (15):**
1. Company Domain Name <COMPANY domain>
2. Company name <COMPANY name>
3. Address <COMPANY address>
4. City <COMPANY city>
5. State/Region <COMPANY state>
6. Postal Code <COMPANY zip>
7. Phone Number <COMPANY phone>
8. Email <CONTACT email>
9. First Name <CONTACT firstname>
10. Last Name <CONTACT lastname>
11. Address <CONTACT address>
12. City <CONTACT city>
13. State/Region <CONTACT state>
14. Postal Code <CONTACT zip>
15. Phone Number <CONTACT phone>

### File 2: Company-Deal Associations
**Filename:** `hubspot_company_deals_[timestamp].csv`
**Purpose:** Links deals to companies (one-to-many: company to deals)
**Columns (7):**
1. Company Domain Name <COMPANY domain>
2. Deal Stage <DEAL dealstage>
3. Pipeline <DEAL pipeline>
4. Deal Name <DEAL dealname>
5. Description <DEAL description>
6. Amount <DEAL amount>
7. Close Date <DEAL closedate>

### File 3: Contact-Deal Associations
**Filename:** `hubspot_contact_deals_[timestamp].csv`
**Purpose:** Links deals to contacts (one-to-many: contact to deals)
**Columns (7):**
1. Email <CONTACT email>
2. Deal Stage <DEAL dealstage>
3. Pipeline <DEAL pipeline>
4. Deal Name <DEAL dealname>
5. Description <DEAL description>
6. Amount <DEAL amount>
7. Close Date <DEAL closedate>

## Data Generation Rules

### Production Mode (10,000 companies)
- **Companies:** 10,000
- **Contacts:** 9,000
- **Deals:** 18,000+
- **Company-Contact Associations:** ~13,000 rows (due to many-to-many)

### Association Rules
1. **Contact-Company (Many-to-Many):**
   - 90% of contacts have one primary company association
   - 25% of contacts have additional companies (2-4 total)
   - Email domains match company domains for 90% of contacts

2. **Company-Deal (One-to-Many):**
   - 90%+ of companies have at least one deal
   - 5%+ of companies have multiple deals (2-4)

3. **Contact-Deal (One-to-Many):**
   - 90%+ of contacts have at least one deal
   - 5%+ of contacts have multiple deals (2-4)

4. **Deal Association:**
   - Each deal is associated with exactly one company AND one contact
   - Contact must be associated with the company for the deal

## Key Benefits of 3-File Structure

### ✅ Proper Association Handling
- HubSpot can correctly import many-to-many company-contact relationships
- Deals properly link to both companies and contacts
- No duplicate deal data across entities

### ✅ Clean Data Separation
- Company and contact master data in File 1
- Deal-company relationships isolated in File 2
- Deal-contact relationships isolated in File 3

### ✅ Import Flexibility
- Can import files in any order
- Can re-import individual files without affecting others
- Easy to verify associations

## Usage

### Generate Data
```cmd
# Test mode (10 companies, 9 contacts, 18 deals)
dotnet run --project HubSpotFakeData\HubSpotFakeData.csproj -- --mode Test

# Production mode (10,000 companies, 9,000 contacts, 18,000+ deals)
dotnet run --project HubSpotFakeData\HubSpotFakeData.csproj -- --mode Production
```

### View Data
1. Open `data-viewer.html` in a web browser
2. Upload all 3 CSV files:
   - `hubspot_company_contacts_[timestamp].csv`
   - `hubspot_company_deals_[timestamp].csv`
   - `hubspot_contact_deals_[timestamp].csv`
3. Click "Load and Display Tree"
4. View the hierarchical tree structure

## HubSpot Import Order

**Recommended import sequence:**

1. **First:** Import `hubspot_company_contacts_[timestamp].csv`
   - Creates companies with all their properties
   - Creates contacts with all their properties
   - Establishes company-contact associations

2. **Second:** Import `hubspot_company_deals_[timestamp].csv`
   - Creates deals with all their properties
   - Links deals to existing companies

3. **Third:** Import `hubspot_contact_deals_[timestamp].csv`
   - Links existing deals to existing contacts

## File Locations
All CSV files are generated in the `output/` directory at the project root.

## Example Data Flow

### Company-Contact (File 1)
```
acme.com, Acme Corp, ..., john.doe@acme.com, John, Doe, ...
acme.com, Acme Corp, ..., jane.smith@acme.com, Jane, Smith, ...
```

### Company-Deal (File 2)
```
acme.com, closedwon, default, Widget Sale #1, ..., 50000, 01/12/2025
acme.com, qualifiedtobuy, default, Service Contract #2, ..., 25000, 15/12/2025
```

### Contact-Deal (File 3)
```
john.doe@acme.com, closedwon, default, Widget Sale #1, ..., 50000, 01/12/2025
jane.smith@acme.com, qualifiedtobuy, default, Service Contract #2, ..., 25000, 15/12/2025
```

## Architecture

### Models
- `Company.cs` - Company entity
- `Contact.cs` - Contact entity
- `Deal.cs` - Deal entity with CompanyId and ContactId
- `CsvCompanyContact.cs` - Company-Contact CSV row
- `CsvCompanyDeal.cs` - Company-Deal CSV row
- `CsvContactDeal.cs` - Contact-Deal CSV row
- `GenerationResult.cs` - Contains all 3 CSV lists

### Services
- `DataGenerationService.cs` - Generates entities and builds associations
- `AssociationManager.cs` - Manages relationships between entities
- `CsvExportService.cs` - Exports 3 separate CSV files

## Statistics Validation

Production mode generates:
- ✅ 10,000 companies
- ✅ 9,000 contacts
- ✅ 18,000+ deals
- ✅ ~13,000 company-contact association rows (many-to-many)
- ✅ 18,000 company-deal rows (one per deal)
- ✅ 18,000 contact-deal rows (one per deal)
- ✅ 90%+ companies with deals
- ✅ 90%+ contacts with deals
- ✅ 90%+ contacts associated with companies
- ✅ 25%+ contacts with multiple companies
- ✅ Email domains match company domains

All association rules are properly enforced!

