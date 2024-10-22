CREATE TABLE [dbo].[UserDetails] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [uniqueId]          VARCHAR (50)    NULL,
    [UserId]            INT             NOT NULL,
    [OrgId]             VARCHAR (50)    NULL,
    [ChapterId]         VARCHAR (50)    NULL,
    [FirstName]         VARCHAR (MAX)   NOT NULL,
    [LastName]          VARCHAR (50)    NOT NULL,
    [FamilyName]        VARCHAR (MAX)   NULL,
    [PhoneNumber]       VARCHAR (10)    NOT NULL,
    [City]              NVARCHAR (50)   NOT NULL,
    [State]             VARCHAR (50)    NOT NULL,
    [WhatsAppNumber]    VARCHAR (50)    NOT NULL,
    [Address]           VARCHAR (MAX)   NOT NULL,
    [PostalCode]        VARCHAR (50)    NOT NULL,
    [Country]           NVARCHAR (50)   NOT NULL,
    [HomeTown]          NVARCHAR (50)   NOT NULL,
    [SocialMedia]       NVARCHAR (15)   NULL,
    [PreferredBy]       NVARCHAR (250)  NULL,
    [DOB]               DATETIME        NULL,
    [Photo]             VARBINARY (MAX) NULL,
    [SpouseFirstName]   VARCHAR (50)    NOT NULL,
    [SpousePhoneNumber] VARCHAR (10)    NULL,
    [SpouseLastName]    VARCHAR (50)    NOT NULL,
    [SpouseDOB]         DATETIME        NOT NULL,
    [SpouseEmail]       VARCHAR (MAX)   NOT NULL,
    [SpouseCity]        VARCHAR (50)    NULL,
    [SpouseState]       VARCHAR (50)    NULL,
    [SpouseCountry]     VARCHAR (50)    NULL,
    [SpouseHometown]    VARCHAR (50)    NOT NULL,
    [Reason]            NVARCHAR (MAX)  NOT NULL,
    [CreatedBy]         INT             NOT NULL,
    [CreatedOn]         DATETIME        NOT NULL,
    [UpdatedBy]         INT             NOT NULL,
    [UpdatedOn]         DATETIME        NOT NULL,
    [IsDeleted]         BIT             NOT NULL,
    CONSTRAINT [PK_UserDetails] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserDetails_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);







