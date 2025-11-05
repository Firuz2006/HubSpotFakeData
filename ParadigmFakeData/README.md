# Paradigm Fake Data Generator

## Overview

Production-ready application for generating and managing fake customer data for Paradigm API using Bogus library with
full DI, logging, and SOLID principles.

## Architecture

### SOLID Principles Implementation

- **Single Responsibility**: Each service handles one concern
- **Open/Closed**: Extensible through interfaces without modification
- **Liskov Substitution**: All implementations can replace their interfaces
- **Interface Segregation**: Focused, specific interfaces
- **Dependency Inversion**: All dependencies injected through DI

### Services

#### ICustomerGenerationService

Generates 300 customers for each type:

- Company (300)
- Customer (300)
- CompanyCustomer (300)
- Total: 900 customers with unique properties

#### ICustomerContactGenerationService

Generates contacts for Company and Customer types:

- 85% customers get 1 contact
- 5% customers get 0 contacts
- 10% customers get 2-4 contacts
- CompanyCustomer gets NO contacts

#### IParadigmApiService

Handles all API communication:

- Batch create customers
- Batch create customer contacts
- Delete individual customers

#### IFileService

Manages file I/O:

- JSON serialization/deserialization
- Output directory creation with timestamp
- File path management

#### IWorkflowOrchestrator

Coordinates the entire workflow with user prompts

## Usage

### Main Workflow

```cmd
cd C:\Users\user\RiderProjects\HubSpotFakeData\ParadigmFakeData
dotnet run
```

**Workflow Steps:**

1. **Generate Customers**
    - Creates 900 customers (300 of each type)
    - Saves to `output_YYYYMMDD_HHMMSS/customers.json`
    - Displays clickable file path
    - Prompts: "Did you review it and can we post it to Paradigm? (y/n)"

2. **Post Customers to Paradigm**
    - Sends to: `http://192.168.1.130:5001/api/Customer/batch-create`
    - Receives customer IDs from API
    - Updates JSON with CustomerId values
    - Saves to `output_YYYYMMDD_HHMMSS/customers_updated.json`
    - Prompts: "Do you want to continue and create customer contacts? (y/n)"

3. **Generate Customer Contacts**
    - Creates contacts based on rules (85/5/10 distribution)
    - Saves to `output_YYYYMMDD_HHMMSS/customer_contacts.json`
    - Displays clickable file path
    - Prompts: "Do you want to continue and post contacts to Paradigm? (y/n)"

4. **Post Customer Contacts to Paradigm**
    - Sends to: `http://192.168.1.130:5001/api/CustomerContact/batch-create`
    - Completes workflow

### Delete Workflow

```cmd
cd C:\Users\user\RiderProjects\HubSpotFakeData\ParadigmFakeData
dotnet run --delete "path\to\customers.json"
```

**Delete Process:**

- Reads customer JSON file
- Extracts all customers with CustomerId
- Calls DELETE endpoint for each: `/api/CustomerContact/{contactId}`
- Reports success/failure counts

## Output Structure

Each run creates a timestamped directory:

```
output_20251103_143022/
├── customers.json              # Initial generated customers
├── customers_updated.json      # Customers with API-assigned IDs
└── customer_contacts.json      # Generated contacts
```

## Data Generation Rules

### Unique Properties

All generated data ensures uniqueness:

- Email addresses (no duplicates across all customers/contacts)
- Phone numbers (no duplicates across all customers/contacts)
- Company names (no duplicates)
- Website URLs (no duplicates)

### Customer Types

**Company**

- `strCompanyName`: Unique company name
- `strPrimaryWebSite`: Unique website URL

**Customer**

- `strFirstName`: First name
- `strLastName`: Last name
- `strPrimaryEmail`: Unique email
- `strPrimaryPhone`: Unique phone

**CompanyCustomer**

- All Company fields +
- All Customer fields
- NO contacts generated

### Customer Contact Distribution

- Only for Company and Customer types
- 85% get exactly 1 contact
- 5% get 0 contacts
- 10% get 2-4 contacts (random)

## Configuration

### API Endpoints

Base URL: `http://192.168.1.130:5001/api`

- Batch Create Customers: `/Customer/batch-create`
- Batch Create Contacts: `/CustomerContact/batch-create`
- Delete Customer: `/CustomerContact/{contactId}`

### Logging

- Console logging enabled
- Minimum level: Information
- Logs all operations, API calls, and errors

## Dependencies

- .NET 9.0
- Bogus 35.6.1
- Microsoft.Extensions.DependencyInjection 9.0.0
- Microsoft.Extensions.Logging 9.0.0
- Microsoft.Extensions.Logging.Console 9.0.0
- Microsoft.Extensions.Http 9.0.0

## Error Handling

- User cancellation supported at each prompt
- API errors logged and reported
- File I/O errors logged with full context
- Delete failures tracked individually

## Build & Run

```cmd
# Restore packages
dotnet restore

# Build
dotnet build

# Run main workflow
dotnet run

# Run delete workflow
dotnet run --delete "output_20251103_143022\customers_updated.json"
```

