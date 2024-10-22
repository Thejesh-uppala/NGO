CREATE TABLE [dbo].[Organization] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [OrgName]     VARCHAR (50)  NOT NULL,
    [OrgWelMsg]   VARCHAR (MAX) NULL,
    [PayPalKey]   VARCHAR (MAX) NULL,
    [CreatedDate] DATETIME      NULL,
    CONSTRAINT [PK_Organization] PRIMARY KEY CLUSTERED ([Id] ASC)
);

