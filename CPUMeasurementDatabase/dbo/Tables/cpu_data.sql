CREATE TABLE [dbo].[cpu_data] (
    [Id]                           INT           IDENTITY (1, 1) NOT NULL,
    [Received]                     DATETIME2 (3) NOT NULL,
    [Temperature]                  FLOAT (53)    NULL,
    [TemperatureMeasurementUnit]   INT           NULL,
    [AverageLoad]                  FLOAT (53)    NULL,
    [IPAddress]                    VARCHAR (15)  NOT NULL,
    [MeasurementIntervalInSeconds] INT           NOT NULL,
    [MeasurementDate]              DATETIME2 (3) NOT NULL,
    CONSTRAINT [PK_cpu_data] PRIMARY KEY CLUSTERED ([Id] ASC)
);



