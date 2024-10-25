CREATE TABLE [dbo].[ChildrensDetails] (
    [Id]              INT           IDENTITY (1, 1) NOT NULL,
    [UserDetailId]    INT           NOT NULL,
    [FirstName]       NVARCHAR (50) NOT NULL,
    [LastName]        NVARCHAR (50) NOT NULL,
    [EmailId]         NVARCHAR (50) NOT NULL,
    [PhoneNo]         VARCHAR (15)  NOT NULL,
    [DOB]             DATE          NOT NULL,
    [Resident]        BIT           NOT NULL,
    [ResidentCity]    NVARCHAR (50) NOT NULL,
    [ResidentState]   NVARCHAR (50) NOT NULL,
    [ResidentCountry] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ChildrensDetails] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ChildrensDetails_ChildrensDetails] FOREIGN KEY ([UserDetailId]) REFERENCES [dbo].[UserDetails] ([Id])
);

