-- ============================================================
-- ServiceFlow Monitor — Database and Schema Creation
-- Run this script first as sysadmin
-- ============================================================

USE master;
GO

-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ServiceFlowMonitor')
BEGIN
    CREATE DATABASE ServiceFlowMonitor
        COLLATE SQL_Latin1_General_CP1_CI_AS;
    PRINT 'Database ServiceFlowMonitor created.';
END
ELSE
    PRINT 'Database ServiceFlowMonitor already exists.';
GO

USE ServiceFlowMonitor;
GO

-- Create schema
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'ServiceFlow')
BEGIN
    EXEC('CREATE SCHEMA ServiceFlow');
    PRINT 'Schema ServiceFlow created.';
END
ELSE
    PRINT 'Schema ServiceFlow already exists.';
GO

PRINT '';
PRINT '============================================================';
PRINT 'Database and schema creation completed successfully!';
PRINT 'Next: Run 02_CreateTables.sql';
PRINT '============================================================';
GO
