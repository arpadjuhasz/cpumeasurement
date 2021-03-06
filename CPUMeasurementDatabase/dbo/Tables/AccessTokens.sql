﻿CREATE TABLE [dbo].[AccessTokens]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[Token] NVARCHAR(1000) NOT NULL,
	[AccountId] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [FK_AccountId] FOREIGN KEY (AccountId) REFERENCES Accounts([Id])
)
