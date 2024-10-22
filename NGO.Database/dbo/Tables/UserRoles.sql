CREATE TABLE [dbo].[UserRoles] (
    [Id]     INT IDENTITY (1, 1) NOT NULL,
    [UserId] INT NOT NULL,
    [RoleId] INT NOT NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id])
);

