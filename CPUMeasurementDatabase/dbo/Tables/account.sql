CREATE TABLE [dbo].[account] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Username] NVARCHAR (256) NOT NULL,
    [Password] NVARCHAR (256) NOT NULL,
    [Name]     NVARCHAR (256) NULL,
    [Deleted]  BIT            NOT NULL,
    CONSTRAINT [PK_account] PRIMARY KEY CLUSTERED ([Id] ASC)
);



