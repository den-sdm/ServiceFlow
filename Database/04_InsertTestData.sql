-- ============================================================
-- ServiceFlow Monitor — Test Data
-- Run this script last
-- ============================================================

USE ServiceFlowMonitor;
GO

-- Insert Verification Types
IF NOT EXISTS (SELECT 1 FROM ServiceFlow.VerificationTypes WHERE TypeName = 'FileProcessing')
BEGIN
    INSERT INTO ServiceFlow.VerificationTypes (TypeName, Description, IsActive)
    VALUES 
        ('FileProcessing', 'Monitor file count in a folder', 1),
        ('DatabaseTable', 'Monitor row count in a database table', 1),
        ('EventViewer', 'Monitor Windows Event Viewer for errors', 1),
        ('FileLog', 'Monitor log file for error patterns', 1),
        ('SQLTableLog', 'Monitor SQL table for error records', 1);
    PRINT 'Verification types inserted.';
END
ELSE
    PRINT 'Verification types already exist.';
GO

-- Insert Test Server
DECLARE @TestServerID INT;

IF NOT EXISTS (SELECT 1 FROM ServiceFlow.Servers WHERE Hostname = 'DESKTOP-H00BENE')
BEGIN
    INSERT INTO ServiceFlow.Servers (Hostname, IPAddress, OperatingSystem, IsActive)
    VALUES ('DESKTOP-H00BENE', '192.168.1.100', 'Windows Server 2019', 1);
    SET @TestServerID = SCOPE_IDENTITY();
    PRINT 'Test server DESKTOP-H00BENE inserted.';
END
ELSE
BEGIN
    SELECT @TestServerID = ServerID FROM ServiceFlow.Servers WHERE Hostname = 'DESKTOP-H00BENE';
    PRINT 'Test server DESKTOP-H00BENE already exists.';
END

-- Insert Test Service
DECLARE @TestServiceID INT;

IF NOT EXISTS (SELECT 1 FROM ServiceFlow.Services WHERE ServiceName = 'FileProcessorService')
BEGIN
    INSERT INTO ServiceFlow.Services (ServerID, ServiceName, FriendlyName, Description, CriticalityLevel, IsActive)
    VALUES (@TestServerID, 'FileProcessorService', 'Invoice File Processor', 'Processes invoice XML files from input folder', 1, 1);
    SET @TestServiceID = SCOPE_IDENTITY();
    PRINT 'Test service FileProcessorService inserted.';
END
ELSE
BEGIN
    SELECT @TestServiceID = ServiceID FROM ServiceFlow.Services WHERE ServiceName = 'FileProcessorService';
    PRINT 'Test service FileProcessorService already exists.';
END

-- Insert Test Verification
DECLARE @FileProcessingTypeID INT, @TestVerificationID INT;

SELECT @FileProcessingTypeID = VerificationTypeID FROM ServiceFlow.VerificationTypes WHERE TypeName = 'FileProcessing';

IF NOT EXISTS (SELECT 1 FROM ServiceFlow.ServiceVerifications WHERE ServiceID = @TestServiceID)
BEGIN
    INSERT INTO ServiceFlow.ServiceVerifications (ServiceID, VerificationTypeID, ConfigurationJSON, PollingIntervalSeconds, ThresholdValue, AlertRepeatMinutes, IsActive)
    VALUES (@TestServiceID, @FileProcessingTypeID,
            '{"FolderPath":"C:\\ProcessingQueue\\Invoices","FilePattern":"IT*.xml","CheckType":"FileCount"}',
            30, 10, 15, 1);
    SET @TestVerificationID = SCOPE_IDENTITY();
    PRINT 'Test verification inserted.';
END
ELSE
BEGIN
    SELECT @TestVerificationID = VerificationID FROM ServiceFlow.ServiceVerifications WHERE ServiceID = @TestServiceID;
    PRINT 'Test verification already exists.';
END

-- Insert Test Status
IF NOT EXISTS (SELECT 1 FROM ServiceFlow.ServiceStatus WHERE VerificationID = @TestVerificationID)
BEGIN
    INSERT INTO ServiceFlow.ServiceStatus (VerificationID, CurrentValue, IsDown, LastCheckTime, DownSince, ErrorMessage)
    VALUES (@TestVerificationID, 5, 0, DATEADD(MINUTE, -2, GETUTCDATE()), NULL, NULL);
    PRINT 'Test status inserted.';
END
ELSE
    PRINT 'Test status already exists.';
GO

-- Insert Test Distribution List
DECLARE @TestServiceID INT;
SELECT @TestServiceID = ServiceID FROM ServiceFlow.Services WHERE ServiceName = 'FileProcessorService';

IF NOT EXISTS (SELECT 1 FROM ServiceFlow.DistributionLists WHERE ServiceID = @TestServiceID)
BEGIN
    INSERT INTO ServiceFlow.DistributionLists (ServiceID, EmailAddress, IsActive)
    VALUES (@TestServiceID, 'team@flex.com', 1);
    PRINT 'Test distribution list inserted.';
END
ELSE
    PRINT 'Test distribution list already exists.';
GO

-- Insert Test History
DECLARE @TestServiceID INT, @TestVerificationID INT;

SELECT @TestServiceID = ServiceID FROM ServiceFlow.Services WHERE ServiceName = 'FileProcessorService';
SELECT @TestVerificationID = VerificationID FROM ServiceFlow.ServiceVerifications WHERE ServiceID = @TestServiceID;

IF NOT EXISTS (SELECT 1 FROM ServiceFlow.ServiceHistory WHERE ServiceID = @TestServiceID)
BEGIN
    INSERT INTO ServiceFlow.ServiceHistory (ServiceID, VerificationID, EventType, EventTime, Value, ErrorMessage)
    VALUES 
        (@TestServiceID, @TestVerificationID, 'Recovery', DATEADD(HOUR, -2, GETUTCDATE()), 0, NULL),
        (@TestServiceID, @TestVerificationID, 'Down', DATEADD(HOUR, -3, GETUTCDATE()), 12, 'File count exceeded threshold'),
        (@TestServiceID, @TestVerificationID, 'Recovery', DATEADD(HOUR, -5, GETUTCDATE()), 3, NULL);
    PRINT 'Test history inserted.';
END
ELSE
    PRINT 'Test history already exists.';
GO

PRINT '';
PRINT '============================================================';
PRINT 'Test data inserted successfully!';
PRINT '';
PRINT 'Database setup complete! You can now:';
PRINT '1. Start the ServiceFlow.API';
PRINT '2. Start the React dashboard';
PRINT '3. Access dashboard at http://localhost:5173';
PRINT '============================================================';
GO
