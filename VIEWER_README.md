# HubSpot Data Relationship Viewer

## Interactive HTML Tree Viewer

A standalone HTML file with embedded JavaScript that visualizes the relationships between Companies, Contacts, and Deals from your generated HubSpot CSV files.

## Features

### 📊 Data Visualization
- **Tree Structure**: Hierarchical view showing:
  - Companies → Contacts → Deals
  - Clear parent-child relationships
  - Expandable/collapsible nodes

### 🎨 Visual Design
- **Color-coded entities**:
  - 🏢 Companies: Purple gradient
  - 👤 Contacts: Magenta gradient  
  - 💼 Deals: Green gradient
- **Deal stages**: Color-coded badges for each stage
- **Responsive layout**: Works on desktop and mobile
- **Modern UI**: Gradient backgrounds, smooth animations

### 🔍 Interactive Features
- **Search**: Real-time search across companies, contacts, and deals
- **Filters**: View all, companies only, or contacts only
- **Expand/Collapse**: Click arrows to show/hide child nodes
- **Drag & Drop**: Drag CSV files directly onto the upload area
- **Statistics Dashboard**: Live counts of companies, contacts, deals, and rows

### 📈 Statistics Displayed
- Total Companies
- Total Contacts
- Total Deals
- Total CSV Rows
- Deals per company
- Contacts per company
- Companies per contact (for many-to-many relationships)

## Usage

### Step 1: Generate CSV Data
```bash
# Generate test data (20 rows)
dotnet run --project HubSpotFakeData -- --mode test

# Generate production data (10,000 rows)
dotnet run --project HubSpotFakeData -- --mode production
```

### Step 2: Open the Viewer
1. Open `viewer.html` in any modern web browser
2. Or double-click the file in File Explorer

### Step 3: Load Your CSV
**Option A - Drag & Drop:**
- Drag your generated CSV file from the `output/` folder
- Drop it onto the upload area

**Option B - File Browser:**
- Click "Choose File" button
- Navigate to `output/` folder
- Select your CSV file

### Step 4: Explore the Data
- **Search**: Type in the search box to filter results
- **Filter**: Click "All", "Companies", or "Contacts" buttons
- **Expand/Collapse**: Click the ▼/▶ arrows to show/hide details
- **View Details**: Each node shows relevant information

## Tree Structure Examples

### Company View (Default - "All")
```
🏢 Acme Corp (example.com)
  ├─ 👤 John Doe (john@email.com) - 3 deals
  │   ├─ 💼 Deal 1 - $50,000 - closedwon
  │   ├─ 💼 Deal 2 - $75,000 - contractsent
  │   └─ 💼 Deal 3 - $100,000 - qualifiedtobuy
  └─ 👤 Jane Smith (jane@email.com) - 2 deals
      ├─ 💼 Deal 4 - $25,000 - appointmentscheduled
      └─ 💼 Deal 5 - $150,000 - closedwon
```

### Contact View (Filter: "Contacts")
```
👤 John Doe (john@email.com)
  ├─ 💼 Deal 1 - Acme Corp - $50,000
  ├─ 💼 Deal 2 - Acme Corp - $75,000
  └─ 💼 Deal 6 - Other Corp - $200,000
```

## Data Parsing

The viewer automatically extracts fields from your CSV:

### Company Fields
- Domain (unique identifier)
- Name
- Address, City, State, Zip
- Phone

### Contact Fields
- Email (unique identifier)
- First Name, Last Name
- Address, City, State, Zip
- Phone

### Deal Fields
- Name
- Stage (with color coding)
- Pipeline
- Description
- Amount (formatted as currency)
- Close Date

## Color-Coded Deal Stages

| Stage | Color |
|-------|-------|
| appointmentscheduled | Blue |
| qualifiedtobuy | Purple |
| presentationscheduled | Yellow |
| decisionmakerboughtin | Orange |
| contractsent | Green |
| closedwon | Bright Green |
| closedlost | Red |

## Technical Details

### No Dependencies Required
- Pure HTML, CSS, and JavaScript
- No frameworks or libraries
- No server needed
- Works completely offline

### Browser Compatibility
- Chrome/Edge (recommended)
- Firefox
- Safari
- Any modern browser with ES6 support

### CSV Parsing
- Handles quoted fields with commas
- Supports escaped quotes
- Tolerates various CSV formats
- Automatically detects HubSpot format with object property tags

### Performance
- Handles 10,000+ rows efficiently
- Real-time search and filtering
- Smooth animations
- Optimized DOM manipulation

## File Location

```
HubSpotFakeData/
├── output/
│   ├── hubspot_import_test_*.csv        # Your generated data
│   └── hubspot_import_production_*.csv  # Your generated data
└── viewer.html                          # This viewer
```

## Tips

1. **Large Files**: For 10,000+ row files, initial load may take a few seconds
2. **Search**: Use search to quickly find specific companies or contacts
3. **Navigation**: Use browser back/forward if you need to reload
4. **Multiple Files**: You can load different CSV files without refreshing the page
5. **Mobile**: Works on mobile devices - use pinch to zoom on tree view

## Troubleshooting

**Problem**: File won't load
- **Solution**: Ensure it's a valid CSV file with the correct HubSpot format

**Problem**: Data looks wrong
- **Solution**: Check that CSV has proper headers with `<OBJECT property>` tags

**Problem**: Tree is empty
- **Solution**: Check browser console (F12) for errors

**Problem**: Slow performance
- **Solution**: Use filters and search to narrow down the view

## Example Screenshots

### Upload Screen
- Clean drag-and-drop interface
- Purple gradient design
- Clear instructions

### Tree View
- Hierarchical structure
- Color-coded entities
- Expandable nodes
- Badge counts

### Statistics Dashboard
- 4 key metrics
- Clean card design
- Auto-updated counts

## Future Enhancements

Possible additions:
- Export filtered data
- Print view
- Graph/network visualization
- Deal timeline view
- Custom sorting options
- Data analytics dashboard

## License

Part of the HubSpotFakeData project.

