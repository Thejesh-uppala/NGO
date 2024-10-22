CREATE TABLE [dbo].[OrganizationChapter] (
    [Id]                    INT           IDENTITY (1, 1) NOT NULL,
    [OrgId]                 INT           NOT NULL,
    [ChapterName]           VARCHAR (50)  NOT NULL,
    [ChapterPresidentName]  VARCHAR (50)  NOT NULL,
    [ChapterPresidentEmail] VARCHAR (50)  NOT NULL,
    [ChapterPresidentPhone] INT           NOT NULL,
    [Details]               VARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_OrganizationChapter] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OrganizationChapter_OrganizationChapter] FOREIGN KEY ([OrgId]) REFERENCES [dbo].[Organization] ([Id])
);

