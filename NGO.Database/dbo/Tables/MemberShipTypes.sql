CREATE TABLE [dbo].[MemberShipTypes] (
    [Id]               INT          IDENTITY (1, 1) NOT NULL,
    [UserDetailId]     INT          NOT NULL,
    [MemberShipType]   VARCHAR (50) NULL,
    [MemberShipAmount] BIGINT       NULL,
    [ValidityPeriod]   INT          NULL,
    CONSTRAINT [PK_MemberShipTypes] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_MemberShipTypes_UserDetails] FOREIGN KEY ([UserDetailId]) REFERENCES [dbo].[UserDetails] ([Id])
);



