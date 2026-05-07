# ServiceFlow Monitor

**Distributed Windows Service Monitoring System with Centralized Dashboard**

A comprehensive monitoring solution for tracking file processing services across multiple servers with real-time status updates, threshold-based alerting, and centralized management dashboard.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [API Documentation](#api-documentation)
- [Database Schema](#database-schema)
- [Technologies](#technologies)
- [Phase 1 Deliverables](#phase-1-deliverables)
- [Known Issues](#known-issues)
- [Roadmap](#roadmap)
- [Team](#team)

---

## 🎯 Overview

ServiceFlow Monitor is a distributed monitoring system designed to track Windows service performance across a large server estate. It monitors file processing pipelines, detects when services fall behind, and provides centralized alerting and management capabilities.

**Project Phase:** Phase 1 (Core Functionality)  
**Demo Date:** May 10, 2026  
**Coordinator:** Mihai.Abrudan@flex.com

### Key Capabilities:
- **Real-time Monitoring:** Track file processing services across multiple servers
- **Threshold Alerts:** Automatic detection when unprocessed files exceed configured limits
- **Centralized Dashboard:** Web-based UI with service status cards and detailed views
- **Configurable Services:** Web interface for adding and managing monitored services
- **Historical Tracking:** Down/recovery event history with timestamps

---

## ✨ Features

### Current (Phase 1):

#### 🖥️ **Web Dashboard**
- Service status cards with color-coded indicators (Green = OK, Red = DOWN)
- Real-time file count vs threshold display
- Search and filter by service name, status, or server
- Service details modal with verification history
- Auto-refresh every 30 seconds

#### ⚙️ **Service Management**
- Settings page for creating new monitoring services
- Configure thresholds, polling intervals, and alert settings
- Distribution list management for email alerts
- Service criticality levels (1=Critical to 4=Low)

#### 📊 **Monitoring**
- File processing verification (count files in folder)
- Threshold-based status determination
- Last check timestamp tracking
- Error message capture and display

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        Web Browser                          │
│                   http://localhost:5173                     │
└────────────────────────┬────────────────────────────────────┘
                         │
                         │ HTTP/JSON
                         │
┌────────────────────────▼────────────────────────────────────┐
│                   React Dashboard                           │
│            (Service Cards, Settings, History)               │
└────────────────────────┬────────────────────────────────────┘
                         │
                         │ REST API
                         │
┌────────────────────────▼────────────────────────────────────┐
│                  .NET 8 Web API                             │
│         (Controllers, Services, Repositories)               │
│                http://localhost:5143                        │
└────────────────────────┬────────────────────────────────────┘
                         │
                         │ ADO.NET / EF Core
                         │
┌────────────────────────▼────────────────────────────────────┐
│              SQL Server Database                            │
│            ServiceFlowMonitor (10 tables)                   │
└─────────────────────────────────────────────────────────────┘
                         ▲
                         │ Status Reports (Phase 2)
                         │
┌────────────────────────┴────────────────────────────────────┐
│              Windows Service Agents                         │
│         (File monitoring, heartbeat - Phase 2)              │
└─────────────────────────────────────────────────────────────┘
```

---

## 📦 Prerequisites

### Required:
- **SQL Server** 2016 or later
- **.NET 8 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Node.js** 18.x or later ([Download](https://nodejs.org/))
- **Git** ([Download](https://git-scm.com/))

### Recommended:
- **SQL Server Management Studio (SSMS)**
- **Visual Studio 2022** or **VS Code**
- **Windows 10/11** or **Windows Server 2016+**

---

## 🚀 Installation

### 1. Clone Repository

```bash
git clone https://github.com/YOUR-USERNAME/ServiceFlow-Monitor.git
cd ServiceFlow-Monitor
```

### 2. Database Setup

Open **SQL Server Management Studio (SSMS)** and execute scripts in order:

```sql
-- Connect to SQL Server, then run:
:r Database\01_CreateDatabase.sql
:r Database\02_CreateTables.sql
:r Database\03_CreateStoredProcedures.sql
:r Database\04_InsertTestData.sql
```

**Or manually execute each file:**
1. File → Open → `Database/01_CreateDatabase.sql` → Execute (F5)
2. File → Open → `Database/02_CreateTables.sql` → Execute (F5)
3. File → Open → `Database/03_CreateStoredProcedures.sql` → Execute (F5)
4. File → Open → `Database/04_InsertTestData.sql` → Execute (F5)

**Verify:**
```sql
USE ServiceFlowMonitor;
SELECT * FROM ServiceFlow.Services;
-- Should return 1 test service: "Invoice File Processor"
```

### 3. Backend API Setup

```powershell
cd ServiceFlow.API

# Restore dependencies
dotnet restore

# Build
dotnet build

# Run
dotnet run
```

**API will start on:** `http://localhost:5143`

**Verify:** Open `http://localhost:5143/swagger` in browser

### 4. Frontend Dashboard Setup

```powershell
cd serviceflow-web

# Install dependencies
npm install

# Start development server
npm run dev
```

**Dashboard will start on:** `http://localhost:5173`

**Verify:** Open `http://localhost:5173` in browser

---

## ⚙️ Configuration

### API Configuration

**File:** `ServiceFlow.API/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ServiceFlowMonitor;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

**Update if using:**
- Different SQL Server instance
- SQL authentication
- Remote database server

### React Configuration

**File:** `serviceflow-web/src/App.jsx` (Line 4)

```javascript
const API_URL = 'http://localhost:5143/api/services';
```

---

## 📖 Usage

### Quick Start

1. **Start API:** `cd ServiceFlow.API && dotnet run`
2. **Start Dashboard:** `cd serviceflow-web && npm run dev`
3. **Open:** `http://localhost:5173`

### Add New Service

1. Click **"Settings"** in sidebar
2. Fill in service details
3. Click **"Save to Database"**
4. Service appears in Dashboard

---

## 📁 Project Structure

```
ServiceFlow/
├── Database/                      # SQL Scripts
│   ├── 01_CreateDatabase.sql
│   ├── 02_CreateTables.sql
│   ├── 03_CreateStoredProcedures.sql
│   └── 04_InsertTestData.sql
├── ServiceFlow.API/               # .NET 8 Web API
│   ├── Controllers/
│   ├── Properties/
│   ├── appsettings.json
│   └── Program.cs
├── ServiceFlow.Core/              # Business Logic
│   └── Services/
├── ServiceFlow.Data/              # Data Access
│   ├── ServiceFlowDbContext.cs
│   └── Repositories/
├── ServiceFlow.Models/            # Data Models
│   ├── Entities/
│   └── DTOs/
├── serviceflow-web/               # React Frontend
│   ├── src/
│   │   ├── App.jsx
│   │   ├── App.css
│   │   └── main.jsx
│   └── package.json
├── .gitignore
└── README.md
```

---

## 🔌 API Documentation

### Base URL: `http://localhost:5143/api`

### Key Endpoints:

#### Get All Services
```http
GET /services
```

#### Get Service Details
```http
GET /services/{id}
```

#### Create Service
```http
POST /services
```

#### Agent Check
```http
POST /agent/check
```

**Full API documentation:** `http://localhost:5143/swagger`

---

## 🗄️ Database Schema

### Key Tables:
- **Servers** - Monitored servers
- **Services** - Service configurations
- **ServiceVerifications** - Verification settings
- **ServiceStatus** - Current status
- **ServiceHistory** - Event history

**Total:** 10 tables, 4 stored procedures

---

## 🛠️ Technologies

- **.NET 8** - Web API
- **React 18** - Frontend
- **SQL Server** - Database
- **Entity Framework Core** - ORM
- **Vite** - Build tool

---

## 🎯 Phase 1 Deliverables

### ✅ Completed:
- [x] Database schema (10 tables)
- [x] .NET 8 Web API
- [x] React dashboard
- [x] Service management
- [x] File processing monitoring scenario

### Demo Ready:
- Dashboard shows service status ✅
- Can create new services ✅
- Status updates in real-time ✅
- Color-coded alerts ✅

---

## ⚠️ Known Issues

- MailKit/MimeKit security warnings (Phase 2 feature)
- Agent not yet implemented (Phase 2)
- Okta authentication pending

---

## 🗺️ Roadmap

### Phase 2:
- [ ] Windows Service agent
- [ ] Email notifications
- [ ] Okta authentication
- [ ] Additional verification types

---

## 👥 Team

- **Developer 1 (Dragoș Bondar):** Database, API
- **Developer 2 (Denisa Sas):** Agent, Integration
- **Developer 3 (Dragoș Moldovan):** React dashboard

**Coordinator:** Cristian Herghelegiu

---

**Last Updated:** May 7, 2026  
**Version:** 1.0.0-phase1
