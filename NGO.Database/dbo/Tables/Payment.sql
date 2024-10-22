CREATE TABLE [dbo].[Payment] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [UserDetailId] INT           NOT NULL,
    [PaymentId]    VARCHAR (50)  NULL,
    [PaymentDate]  DATETIME      NULL,
    [PayPalKey]    VARCHAR (MAX) NULL,
    CONSTRAINT [PK_Payment] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Payment_Payment] FOREIGN KEY ([UserDetailId]) REFERENCES [dbo].[UserDetails] ([Id])
);

