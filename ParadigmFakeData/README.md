Paradigm Fake Data Generator

Requirements
- .NET 9 SDK

Build

Open cmd.exe at repository root and run:

```cmd
dotnet build ParadigmFakeData\ParadigmFakeData.csproj
```

Run (examples)
- Generate customers (save JSON into a directory):

```cmd
dotnet run --project ParadigmFakeData\ParadigmFakeData.csproj -- generate-customers C:\path\to\output\dir
```

- Post customers (use previously generated JSON):

```cmd
dotnet run --project ParadigmFakeData\ParadigmFakeData.csproj -- post-customers C:\path\to\customers.json
```

- Generate customer contacts from customers JSON:

```cmd
dotnet run --project ParadigmFakeData\ParadigmFakeData.csproj -- generate-contacts C:\path\to\customers.json
```

- Generate and post opportunities from customers JSON:

```cmd
dotnet run --project ParadigmFakeData\ParadigmFakeData.csproj -- generate-post-opportunities C:\path\to\customers.json
```

- Generate SQL for deletions (example for opportunities):

```cmd
dotnet run --project ParadigmFakeData\ParadigmFakeData.csproj -- delete-opportunities C:\path\to\opportunities.json
```

Notes
- Commands are positional; supply the required file or directory path as shown.
- API key and DB settings come from `appsettings.json` or user secrets.
- Output files are saved next to the provided input file or into the given output directory.
