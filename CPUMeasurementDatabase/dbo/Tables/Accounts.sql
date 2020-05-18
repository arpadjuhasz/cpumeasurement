CREATE TABLE [dbo].[Accounts] (
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    [Username] NVARCHAR (256) NOT NULL,
    [Password] NVARCHAR (256) NOT NULL,
    [Name]     NVARCHAR (256) NULL,
    [Deleted]  BIT            NOT NULL,
    CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ([Id])
);



