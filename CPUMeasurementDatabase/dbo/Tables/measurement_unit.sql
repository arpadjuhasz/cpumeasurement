CREATE TABLE [dbo].[measurement_unit] (
    [id]   INT        NOT NULL,
    [unit] NCHAR (10) NOT NULL,
    [name] NCHAR (10) NOT NULL,
    CONSTRAINT [PK_measurement_unit] PRIMARY KEY CLUSTERED ([id] ASC)
);

