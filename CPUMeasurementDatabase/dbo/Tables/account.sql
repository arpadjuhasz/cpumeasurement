CREATE TABLE [dbo].[account] (
    [id]       INT            IDENTITY (1, 1) NOT NULL,
    [name]     NVARCHAR (256) NOT NULL,
    [password] NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_account] PRIMARY KEY CLUSTERED ([id] ASC)
);

