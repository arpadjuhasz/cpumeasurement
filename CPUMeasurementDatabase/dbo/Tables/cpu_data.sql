CREATE TABLE [dbo].[cpu_data] (
    [id]                  INT           IDENTITY (1, 1) NOT NULL,
    [received]            DATETIME2 (3) NOT NULL,
    [temperature]         FLOAT (53)    NULL,
    [temperature_unit_id] INT           NULL,
    [average_load]        FLOAT (53)    NULL,
    [ip_address]          VARCHAR (15)  NOT NULL,
    CONSTRAINT [PK_cpu_data] PRIMARY KEY CLUSTERED ([id] ASC)
);

