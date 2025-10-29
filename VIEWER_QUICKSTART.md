# 🚀 Quick Start - Data Viewer

## Generate Data & View in 3 Steps

### Step 1: Generate CSV Data
```bash
cd C:\Users\user\RiderProjects\HubSpotFakeData
dotnet run --project HubSpotFakeData -- --mode test
```

**Output**: `output/hubspot_import_test_YYYYMMDD_HHMMSS.csv`

### Step 2: Open Viewer
```bash
start viewer.html
```

Or double-click `viewer.html` in File Explorer

### Step 3: Load Your Data
1. Drag the generated CSV file from `output/` folder
2. Drop it onto the purple upload area
3. **Done!** 🎉

## What You'll See

### Tree Structure
```
🏢 Company Name (domain.com) - 7 deals, 3 contacts
  ├─ 👤 Contact 1 (email@example.com) - 3 deals
  │   ├─ 💼 Deal 1 - $50,000 - closedwon
  │   ├─ 💼 Deal 2 - $75,000 - contractsent
  │   └─ 💼 Deal 3 - $100,000 - qualifiedtobuy
  └─ 👤 Contact 2 (email2@example.com) - 4 deals
      └─ ...
```

### Features
- 🔍 **Search**: Type to filter companies, contacts, or deals
- 🎯 **Filter**: Click "All", "Companies", or "Contacts"
- 📊 **Stats**: See totals at the top
- 🎨 **Color Codes**: Different colors for each deal stage

## Tips

- **Expand/Collapse**: Click ▼/▶ arrows
- **Large Files**: Use search to narrow results
- **Many-to-Many**: See contacts working with multiple companies

## That's It!

No installation, no configuration, just drag and drop!

---

**Files**:
- `viewer.html` - The viewer (open this)
- `output/` - Your CSV files (drag these)
- `VIEWER_README.md` - Full documentation

