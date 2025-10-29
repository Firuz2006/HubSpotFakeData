# HubSpot Fake Data Generator - Quick Reference

## 🚀 Quick Start

```bash
# Test mode - 20 records demonstrating all patterns
dotnet run --project HubSpotFakeData -- --mode test

# Production mode - 10,000 records
dotnet run --project HubSpotFakeData -- --mode production

# Default (test mode)
dotnet run --project HubSpotFakeData
```

## 📊 Expected Results

### Test Mode
- **Companies**: 3
- **Contacts**: 4
- **Deals**: 20
- **Many-to-Many**: 2+ contacts with multiple companies

### Production Mode
- **Companies**: ~650
- **Contacts**: ~2,500
- **Deals**: 10,000
- **Multi-Company Contacts**: 20-30% (proven: 40.9%)

## 📁 Output Location

```
output/hubspot_import_{mode}_{timestamp}.csv
```

Example:
```
output/hubspot_import_test_20251028_160632.csv
output/hubspot_import_production_20251028_160903.csv
```

## 📋 CSV Format

```csv
Company Domain Name <COMPANY domain>,Company name <COMPANY name>,Email <CONTACT email>,First Name <CONTACT firstname>,Last Name <CONTACT lastname>,Deal Stage <DEAL dealstage>,Pipeline <DEAL pipeline>,Deal Name <DEAL dealname>
```

## 🔗 Association Rules

1. **Deal → Company**: 1:N (each deal has 1 company, company has many deals)
2. **Deal → Contact**: 1:N (each deal has 1 contact, contact has many deals)
3. **Company ↔ Contact**: M:N (many-to-many)

## 🏗️ Architecture

```
Models/          → Domain entities
Services/        → Business logic
  - DataGenerationService    → Bogus data generation
  - CsvExportService        → CSV writing
  - AssociationManager      → Relationship tracking
Program.cs       → DI + CLI entry point
```

## 📦 Dependencies

- .NET 9.0
- Bogus 35.6.5
- Microsoft.Extensions.DependencyInjection 9.0.0
- Microsoft.Extensions.Logging 9.0.0

## ✅ Validation

The generated CSV demonstrates:
- ✅ Same email = same contact across rows
- ✅ Same domain = same company across rows
- ✅ Unique deal names for each row
- ✅ Contacts working with multiple companies
- ✅ Companies with multiple contacts
- ✅ Contacts with multiple deals

## 📖 Documentation

- **USAGE.md** - Detailed usage guide
- **IMPLEMENTATION_SUMMARY.md** - Technical implementation details
- **README.md** - Project overview

