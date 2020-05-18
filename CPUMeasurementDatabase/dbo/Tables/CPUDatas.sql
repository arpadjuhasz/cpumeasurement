CREATE TABLE [dbo].[CPUDatas] (
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




