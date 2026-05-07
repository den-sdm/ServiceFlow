-- ============================================================
-- ServiceFlow Monitor — Table Creation
-- Run this script second
-- ============================================================

USE ServiceFlowMonitor;
GO

-- Table 1: Servers
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Servers' AND schema_id = SCHEMA_ID('ServiceFlow'))
BEGIN
    CREATE TABLE ServiceFlow.Servers (
        ServerID INT IDENTITY(1,1) PRIMARY KEY,
        Hostname NVARCHAR(255) NOT NULL UNIQUE,
        IPAddress NVARCHAR(50),
        OperatingSystem NVARCHAR(100),
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT 'Table ServiceFlow.Servers created.';
END
ELSE
    PRINT 'Table ServiceFlow.Servers already exists.';
GO

-- Table 2: Services
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Services' AND schema_id = SCHEMA_ID('ServiceFlow'))
BEGIN
    CREATE TABLE ServiceFlow.Services (
        ServiceID INT IDENTITY(1,1) PRIMARY KEY,
        ServerID INT NOT NULL,
        ServiceName NVARCHAR(255) NOT NULL,
        FriendlyName NVARCHAR(255),
        Description NVARCHAR(MAX),
        CriticalityLevel INT NOT NULL DEFAULT 3,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_Services_Servers FOREIGN KEY (ServerID) REFERENCES ServiceFlow.Servers(ServerID)
    );
    PRINT 'Table ServiceFlow.Services created.';
END
ELSE
    PRINT 'Table ServiceFlow.Services already exists.';
GO

-- Table 3: VerificationTypes
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VerificationTypes' AND schema_id = SCHEMA_ID('ServiceFlow'))
BEGIN
    CREATE TABLE ServiceFlow.VerificationTypes (
        VerificationTypeID INT IDENTITY(1,1) PRIMARY KEY,
        TypeName NVARCHAR(50) NOT NULL UNIQUE,
        Description NVARCHAR(MAX),
        IsActive BIT NOT NULL DEFAULT 1
    );
    PRINT 'Table ServiceFlow.VerificationTypes created.';
END
ELSE
    PRINT 'Table ServiceFlow.VerificationTypes already exists.';
GO

-- Table 4: ServiceVerifications
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ServiceVerifications' AND schema_id = SCHEMA_ID('ServiceFlow'))
BEGIN
    CREATE TABLE ServiceFlow.ServiceVerifications (
        VerificationID INT IDENTITY(1,1) PRIMARY KEY,
        ServiceID INT NOT NULL,
        VerificationTypeID INT NOT NULL,
        ConfigurationJSON NVARCHAR(MAX),
        PollingIntervalSeconds INT NOT NULL DEFAULT 60,
        ThresholdValue INT,
        AlertRepeatMinutes INT NOT NULL DEFAULT 30,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_ServiceVerifications_Services FOREIGN KEY (ServiceID) REFERENCES ServiceFlow.Services(ServiceID),
        CONSTRAINT FK_ServiceVerifications_Types FOREIGN KEY (VerificationTypeID) REFERENCES ServiceFlow.VerificationTypes(VerificationTypeID)
    );
    PRINT 'Table ServiceFlow.ServiceVerifications created.';
END
ELSE
    PRINT 'Table ServiceFlow.ServiceVerifications already exists.';
GO

-- Table 5: ServiceStatus
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ServiceStatus' AND schema_id = SCHEMA_ID('ServiceFlow'))
BEGIN
    CREATE TABLE ServiceFlow.ServiceStatus (
        StatusID INT IDENTITY(1,1) PRIMARY KEY,
        VerificationID INT NOT NULL UNIQUE,
        CurrentValue INT,
        IsDown BIT NOT NULL DEFAULT 0,
        LastCheckTime DATETIME,
        DownSince DATETIME,
        ErrorMessage NVARCHAR(MAX),
        LastAlertSent DATETIME,
        CONSTRAINT FK_ServiceStatus_Verifications FOREIGN KEY (VerificationID) REFERENCES ServiceFlow.ServiceVerifications(VerificationID)
    );
    PRINT 'Table ServiceFlow.ServiceStatus created.';
END
ELSE
    PRINT 'Table ServiceFlow.ServiceStatus already exists.';
GO

-- Table 6: ServiceHistory
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ServiceHistory' AND schema_id = SCHEMA_ID('ServiceFlow'))
BEGIN
    CREATE TABLE ServiceFlow.ServiceHistory (
        HistoryID INT IDENTITY(1,1) PRIMARY KEY,
        ServiceID INT NOT NULL,
        VerificationID INT,
        EventType NVARCHAR(50) NOT NULL,
        EventTime DATETIME NOT NULL DEFAULT GETUTCDATE(),
        Value INT,
        ErrorMessage NVARCHAR(MAX),
        CONSTRAINT FK_ServiceHistory_Services FOREIGN KEY (ServiceID) REFERENCES ServiceFlow.Services(ServiceID),
        CONSTRAINT FK_ServiceHistory_Verifications FOREIGN KEY (VerificationID) REFERENCES ServiceFlow.ServiceVerifications(VerificationID)
    );
    PRINT 'Table ServiceFlow.ServiceHistory created.';
END
ELSE
    PRINT 'Table ServiceFlow.ServiceHistory already exists.';
GO

-- Table 7: AlertLog
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AlertLog' AND schema_id = SCHEMA_ID('ServiceFlow'))
BEGIN
    CREATE TABLE ServiceFlow.AlertLog (
        AlertID INT IDENTITY(1,1) PRIMARY KEY,
        ServiceID INT NOT NULL,
        VerificationID INT,
        AlertType NVARCHAR(50) NOT NULL,
        AlertMessage NVARCHAR(MAX),
        SentTo NVARCHAR(MAX),
        SentTime DATETIME NOT NULL DEFAULT GETUTCDATE(),
        IsSuccessful BIT NOT NULL DEFAULT 1,
        CONSTRAINT FK_AlertLog_Services FOREIGN KEY (ServiceID) REFERENCES ServiceFlow.Services(ServiceID),
        CONSTRAINT FK_AlertLog_Verifications FOREIGN KEY (VerificationID) REFERENCES ServiceFlow.ServiceVerifications(VerificationID)
    );
    PRINT 'Table ServiceFlow.AlertLog created.';
END
ELSE
    PRINT 'Table ServiceFlow.AlertLog already exists.';
GO

-- Table 8: DistributionLists
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DistributionLists' AND schema_id = SCHEMA_ID('ServiceFlow'))
BEGIN
    CREATE TABLE ServiceFlow.DistributionLists (
        DistributionListID INT IDENTITY(1,1) PRIMARY KEY,
        ServiceID INT NOT NULL,
        EmailAddress NVARCHAR(255) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_DistributionLists_Services FOREIGN KEY (ServiceID) REFERENCES ServiceFlow.Services(ServiceID)
    );
    PRINT 'Table ServiceFlow.DistributionLists created.';
END
ELSE
    PRINT 'Table ServiceFlow.DistributionLists already exists.';
GO

-- Table 9: AgentHeartbeat
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AgentHeartbeat' AND schema_id = SCHEMA_ID('ServiceFlow'))
BEGIN
    CREATE TABLE ServiceFlow.AgentHeartbeat (
        HeartbeatID INT IDENTITY(1,1) PRIMARY KEY,
        ServerID INT NOT NULL,
        AgentVersion NVARCHAR(50),
        LastHeartbeat DATETIME NOT NULL DEFAULT GETUTCDATE(),
        Status NVARCHAR(50) NOT NULL DEFAULT 'Healthy',
        CONSTRAINT FK_AgentHeartbeat_Servers FOREIGN KEY (ServerID) REFERENCES ServiceFlow.Servers(ServerID)
    );
    PRINT 'Table ServiceFlow.AgentHeartbeat created.';
END
ELSE
    PRINT 'Table ServiceFlow.AgentHeartbeat already exists.';
GO

-- Table 10: ServiceRestartLog
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ServiceRestartLog' AND schema_id = SCHEMA_ID('ServiceFlow'))
BEGIN
    CREATE TABLE ServiceFlow.ServiceRestartLog (
        RestartID INT IDENTITY(1,1) PRIMARY KEY,
        ServiceID INT NOT NULL,
        RestartedBy NVARCHAR(255),
        RestartTime DATETIME NOT NULL DEFAULT GETUTCDATE(),
        RestartMethod NVARCHAR(100),
        IsSuccessful BIT NOT NULL DEFAULT 1,
        ErrorMessage NVARCHAR(MAX),
        CONSTRAINT FK_ServiceRestartLog_Services FOREIGN KEY (ServiceID) REFERENCES ServiceFlow.Services(ServiceID)
    );
    PRINT 'Table ServiceFlow.ServiceRestartLog created.';
END
ELSE
    PRINT 'Table ServiceFlow.ServiceRestartLog already exists.';
GO

PRINT '';
PRINT '============================================================';
PRINT 'All tables created successfully!';
PRINT 'Next: Run 03_CreateStoredProcedures.sql';
PRINT '============================================================';
GO
