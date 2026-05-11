# ServiceFlow Monitor

Monitor your Windows file processing services from one place.

This system tracks services across multiple servers, sends alerts when file counts get too high, and gives you a web dashboard to see what's happening.

---

## What you need before starting

Install these first:

- SQL Server 2016 or newer
- .NET 8 SDK - https://dotnet.microsoft.com/download/dotnet/8.0
- Node.js 18 or newer - https://nodejs.org/

You'll also want SQL Server Management Studio (SSMS) to run the database scripts.

---

## Getting it running

### 1. Clone the repo

```bash
git clone https://github.com/den-sdm/ServiceFlow.git
cd ServiceFlow
```

Check that your tools are installed:

```powershell
dotnet --version  # needs to be 8.0.x
node --version    # needs to be v18.x.x or higher
```

### 2. Set up the database

Open SSMS and create the database:

```sql
CREATE DATABASE ServiceFlowMonitor;
GO
```

Now run the table creation script. In SSMS, open all sql files and hit F5.

**Important step:** Verify the schema is correct. Some columns are easy to miss and will cause errors later.

```sql
USE ServiceFlowMonitor;

-- Check these two columns exist
SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Servers' AND COLUMN_NAME = 'LastHeartbeat';

SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS  
WHERE TABLE_NAME = 'DistributionLists' AND COLUMN_NAME = 'ListID';
```

Both queries should return 1 row. If either returns nothing, check the troubleshooting section below.

### 3. Configure the connection string

Edit `ServiceFlow.API/appsettings.json` and update the Server name if needed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ServiceFlowMonitor;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

Change `localhost` to your SQL Server instance name if you're not using the default.

### 4. Start the API

Open a terminal:

```powershell
cd ServiceFlow.API
dotnet restore
dotnet build
dotnet run
```

You should see:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5143
```

Leave this terminal open.

### 5. Start the dashboard

Open a second terminal:

```powershell
cd serviceflow-dashboard
npm install
npm run dev
```

You should see:

```
VITE v5.x.x  ready in 500 ms
➜  Local:   http://localhost:5173/
```

Leave this terminal open too.

### 6. Open it in your browser

Go to http://localhost:5173

You should see the dashboard. If you ran the test data script, there's already a service card showing. If not, you'll need to add one.

---

## Adding a service to monitor

Click Settings in the sidebar and fill out the form:

- Service Name: `TestProcessor` (or whatever you want to call it)
- Server Hostname: Your computer name (check with `hostname` in cmd)
- Friendly Name: `Test File Processing Service`
- Criticality Level: Medium
- UNC Path: `C:\TestFolder` (create this folder first)
- File Pattern: `*.txt`
- File Count Threshold: 5 (alerts when you have more than 5 files)
- Polling Interval: 60 seconds
- Alert Repeat: 30 minutes
- Email: your.email@example.com

Click Save. The service should appear on the Dashboard page.

Status colors:
- Green = file count is below threshold
- Red = file count exceeded threshold
- Gray = not checked yet

---

## When things go wrong

### API won't start: "Invalid column name 'LastHeartbeat'"

The Servers table is missing a column. Fix it:

```sql
USE ServiceFlowMonitor;
ALTER TABLE [ServiceFlow].[Servers]
ADD [LastHeartbeat] DATETIME2 NULL;
```

### API won't start: "Invalid column name 'ListID'"

The DistributionLists table has the wrong column name. Fix it:

```sql
USE ServiceFlowMonitor;
EXEC sp_rename 
    '[ServiceFlow].[DistributionLists].[DistributionListID]', 
    'ListID', 
    'COLUMN';
```

### Can't save services: "CHECK constraint 'CK_ServiceVerifications_JSON'"

The database is checking if configuration is valid JSON. Drop the constraint:

```sql
USE ServiceFlowMonitor;
ALTER TABLE [ServiceFlow].[ServiceVerifications]
DROP CONSTRAINT CK_ServiceVerifications_JSON;
```

We can add it back later with proper validation.

### Dashboard shows one service but database has more

Old code filtered out services that don't have status records yet. Get the latest code:

```powershell
git pull origin main
cd ServiceFlow.API
dotnet clean
dotnet build
dotnet run
```

Restart the dashboard too.

### Dashboard says "Could not fetch data from API"

Check these:
1. Is the API running? Visit http://localhost:5143/swagger
2. Did SQL Server start?
3. Is the connection string correct in appsettings.json?
4. Try clearing your browser cache (Ctrl+Shift+Del) and refreshing

### Port 5143 is already in use

Find what's using it and kill it:

```powershell
netstat -ano | findstr :5143
taskkill /PID <number from above> /F
```

Then start the API again.

---

## How the code is organized

```
ServiceFlow/
├── Database/
│   ├── 01_CreateDatabase.sql
│   ├── 02_CreateTables.sql          ← Run this first
│   ├── 03_CreateStoredProcedures.sql
│   └── 04_InsertTestData.sql
│
├── ServiceFlow.API/                 ← .NET 8 backend
│   ├── Controllers/
│   ├── appsettings.json            ← Update connection string here
│   └── Program.cs
│
├── ServiceFlow.Core/                ← Business logic
│   └── Services/
│       └── MonitoringService.cs
│
├── ServiceFlow.Data/                ← Database access
│   ├── ServiceFlowDbContext.cs
│   └── Repositories/
│
├── ServiceFlow.Models/              ← Data structures
│   ├── Entities/
│   │   ├── Service.cs
│   │   ├── Server.cs
│   │   ├── ServiceVerification.cs
│   │   └── DistributionList.cs     ← This file must exist
│   └── DTOs/
│
└── serviceflow-dashboard/           ← React frontend
    ├── src/
    │   ├── App.jsx
    │   └── main.jsx
    └── package.json
```

---

## Making changes

If you want to add features or fix bugs:

```powershell
# Create a branch
git checkout -b feature/your-change

# Make your changes, then test
cd ServiceFlow.API
dotnet build
dotnet run

# Test the frontend
cd ../serviceflow-dashboard
npm run dev

# Commit when it works
git add .
git commit -m "What you changed"
git push origin feature/your-change
```

Before committing, make sure:
- API builds without errors
- Dashboard loads at http://localhost:5173
- You can create a service and it shows up on the dashboard

---

## What's not done yet

Phase 1 (the current demo) has the core features working. These are planned for later:

- Windows Service agent that actually checks files (right now status is manual)
- Email notifications when thresholds are exceeded
- Okta login
- Monitoring Event Viewer logs
- Monitoring SQL table rows
- Monitoring text log files
- Restarting services from the dashboard

---

## Quick reference for demo day (May 10)

```powershell
# Start SQL Server if it's not running

# Pull latest code
git pull origin main

# Terminal 1
cd ServiceFlow.API
dotnet run

# Terminal 2  
cd serviceflow-dashboard
npm run dev

# Open browser to http://localhost:5173
```

Test that you can create a service before the actual demo.

---

## Who built this

- Dragoș Bondar: Database and API
- Denisa Sas: Agent and system integration
- Dragoș Moldovan: React dashboard

Project coordinator: Cristian Herghelegiu  

---

## Questions?

Check the troubleshooting section first. If you're still stuck, email Mihai.Abrudan@flex.com or open an issue on GitHub.

---

Last updated: May 11, 2026  
Version: 1.0.0 (Phase 1)  
Repository: https://github.com/den-sdm/ServiceFlow
