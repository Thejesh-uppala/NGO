CREATE TABLE [dbo].[Roles] (
    [Id]   INT           IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC)
);

