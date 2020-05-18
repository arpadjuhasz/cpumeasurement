IF (NOT EXISTS (SELECT name 
FROM master.dbo.sysdatabases 
WHERE ('[' + name + ']' = '[CPUMeasurement]' 
OR name = 'CPUMeasurement')))
BEGIN
    CREATE DATABASE CPUMeasurement
    PRINT 'Database created!';
END

USE CPUMeasurement

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE   TABLE_NAME = 'Accounts'))
BEGIN
	CREATE TABLE [dbo].[Accounts] (
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    [Username] NVARCHAR (256) NOT NULL,
    [Password] NVARCHAR (256) NOT NULL,
    [Name]     NVARCHAR (256) NULL,
    [Deleted]  BIT            NOT NULL,
    CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ([Id])
);
END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE   TABLE_NAME = 'AccessTokens'))
BEGIN
	CREATE TABLE [dbo].[AccessTokens]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[Token] NVARCHAR(1000) NOT NULL,
	[AccountId] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [FK_AccountId] FOREIGN KEY (AccountId) REFERENCES Accounts([Id])
);
END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE   TABLE_NAME = 'CPUMeasurements'))
BEGIN
	CREATE TABLE [dbo].[CPUMeasurements] (
    [Id]                           UNIQUEIDENTIFIER NOT NULL,
    [Received]                     DATETIME2 (3) NOT NULL,
    [Temperature]                  FLOAT (53)    NULL,
    [TemperatureMeasurementUnit]   INT           NULL,
    [AverageLoad]                  FLOAT (53)    NULL,
    [IPAddress]                    VARCHAR (15)  NOT NULL,
    [MeasurementIntervalInSeconds] INT           NOT NULL,
    [MeasurementDate]              DATETIME2 (3) NOT NULL,
    CONSTRAINT [PK_CPUData] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END



