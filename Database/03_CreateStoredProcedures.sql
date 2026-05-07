-- ============================================================
-- ServiceFlow Monitor — Stored Procedures
-- Run this script third
-- ============================================================

USE ServiceFlowMonitor;
GO

-- ============================================================
-- Stored Procedure 1: usp_RegisterServer
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'ServiceFlow.usp_RegisterServer') AND type = 'P')
    DROP PROCEDURE ServiceFlow.usp_RegisterServer;
GO

CREATE PROCEDURE ServiceFlow.usp_RegisterServer
    @Hostname NVARCHAR(255),
    @IPAddress NVARCHAR(50) = NULL,
    @ServerID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT @ServerID = ServerID FROM ServiceFlow.Servers WHERE Hostname = @Hostname;
    
    IF @ServerID IS NULL
    BEGIN
        INSERT INTO ServiceFlow.Servers (Hostname, IPAddress, IsActive, CreatedDate, ModifiedDate)
        VALUES (@Hostname, @IPAddress, 1, GETUTCDATE(), GETUTCDATE());
        SET @ServerID = SCOPE_IDENTITY();
    END
    ELSE
    BEGIN
        UPDATE ServiceFlow.Servers
        SET IPAddress = ISNULL(@IPAddress, IPAddress), ModifiedDate = GETUTCDATE()
        WHERE ServerID = @ServerID;
    END
END;
GO

PRINT 'Stored Procedure ServiceFlow.usp_RegisterServer created.';
GO

-- ============================================================
-- Stored Procedure 2: usp_UpdateAgentHeartbeat
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'ServiceFlow.usp_UpdateAgentHeartbeat') AND type = 'P')
    DROP PROCEDURE ServiceFlow.usp_UpdateAgentHeartbeat;
GO

CREATE PROCEDURE ServiceFlow.usp_UpdateAgentHeartbeat
    @ServerID INT,
    @AgentVersion NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (SELECT 1 FROM ServiceFlow.AgentHeartbeat WHERE ServerID = @ServerID)
    BEGIN
        UPDATE ServiceFlow.AgentHeartbeat
        SET LastHeartbeat = GETUTCDATE(), AgentVersion = ISNULL(@AgentVersion, AgentVersion), Status = 'Healthy'
        WHERE ServerID = @ServerID;
    END
    ELSE
    BEGIN
        INSERT INTO ServiceFlow.AgentHeartbeat (ServerID, AgentVersion, LastHeartbeat, Status)
        VALUES (@ServerID, @AgentVersion, GETUTCDATE(), 'Healthy');
    END
END;
GO

PRINT 'Stored Procedure ServiceFlow.usp_UpdateAgentHeartbeat created.';
GO

-- ============================================================
-- Stored Procedure 3: usp_UpdateServiceStatus
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'ServiceFlow.usp_UpdateServiceStatus') AND type = 'P')
    DROP PROCEDURE ServiceFlow.usp_UpdateServiceStatus;
GO

CREATE PROCEDURE ServiceFlow.usp_UpdateServiceStatus
    @VerificationID INT,
    @CurrentValue INT,
    @IsDown BIT,
    @ErrorMessage NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ServiceID INT, @WasDown BIT;
    
    SELECT @ServiceID = ServiceID FROM ServiceFlow.ServiceVerifications WHERE VerificationID = @VerificationID;
    SELECT @WasDown = IsDown FROM ServiceFlow.ServiceStatus WHERE VerificationID = @VerificationID;
    
    IF EXISTS (SELECT 1 FROM ServiceFlow.ServiceStatus WHERE VerificationID = @VerificationID)
    BEGIN
        UPDATE ServiceFlow.ServiceStatus
        SET CurrentValue = @CurrentValue, IsDown = @IsDown, LastCheckTime = GETUTCDATE(),
            DownSince = CASE WHEN @IsDown = 1 AND @WasDown = 0 THEN GETUTCDATE()
                             WHEN @IsDown = 0 THEN NULL ELSE DownSince END,
            ErrorMessage = @ErrorMessage
        WHERE VerificationID = @VerificationID;
    END
    ELSE
    BEGIN
        INSERT INTO ServiceFlow.ServiceStatus (VerificationID, CurrentValue, IsDown, LastCheckTime, DownSince, ErrorMessage)
        VALUES (@VerificationID, @CurrentValue, @IsDown, GETUTCDATE(), 
                CASE WHEN @IsDown = 1 THEN GETUTCDATE() ELSE NULL END, @ErrorMessage);
    END
    
    IF (@WasDown IS NULL) OR (@WasDown <> @IsDown)
    BEGIN
        INSERT INTO ServiceFlow.ServiceHistory (ServiceID, VerificationID, EventType, EventTime, Value, ErrorMessage)
        VALUES (@ServiceID, @VerificationID, CASE WHEN @IsDown = 1 THEN 'Down' ELSE 'Recovery' END,
                GETUTCDATE(), @CurrentValue, @ErrorMessage);
    END
END;
GO

PRINT 'Stored Procedure ServiceFlow.usp_UpdateServiceStatus created.';
GO

-- ============================================================
-- Stored Procedure 4: usp_GetServicesForCheck
-- ============================================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'ServiceFlow.usp_GetServicesForCheck') AND type = 'P')
    DROP PROCEDURE ServiceFlow.usp_GetServicesForCheck;
GO

CREATE PROCEDURE ServiceFlow.usp_GetServicesForCheck
    @ServerID INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT s.ServiceID, s.ServiceName, s.FriendlyName, sv.VerificationID, vt.TypeName AS VerificationType,
           sv.ConfigurationJSON, sv.PollingIntervalSeconds, sv.ThresholdValue,
           COALESCE(ss.LastCheckTime, DATEADD(HOUR, -1, GETUTCDATE())) AS LastCheckTime,
           CASE WHEN ss.LastCheckTime IS NULL THEN 999999
                ELSE DATEDIFF(SECOND, ss.LastCheckTime, GETUTCDATE()) END AS SecondsSinceLastCheck
    FROM ServiceFlow.Services s
    INNER JOIN ServiceFlow.Servers srv ON s.ServerID = srv.ServerID
    INNER JOIN ServiceFlow.ServiceVerifications sv ON s.ServiceID = sv.ServiceID
    INNER JOIN ServiceFlow.VerificationTypes vt ON sv.VerificationTypeID = vt.VerificationTypeID
    LEFT JOIN ServiceFlow.ServiceStatus ss ON sv.VerificationID = ss.VerificationID
    WHERE s.IsActive = 1 AND sv.IsActive = 1 AND srv.ServerID = @ServerID
          AND (ss.LastCheckTime IS NULL OR DATEDIFF(SECOND, ss.LastCheckTime, GETUTCDATE()) >= sv.PollingIntervalSeconds)
    ORDER BY CASE WHEN ss.LastCheckTime IS NULL THEN 999999
                  ELSE DATEDIFF(SECOND, ss.LastCheckTime, GETUTCDATE()) END DESC;
END;
GO

PRINT 'Stored Procedure ServiceFlow.usp_GetServicesForCheck created.';
GO

PRINT '';
PRINT '============================================================';
PRINT 'All stored procedures created successfully!';
PRINT 'Next: Run 04_InsertTestData.sql';
PRINT '============================================================';
GO
