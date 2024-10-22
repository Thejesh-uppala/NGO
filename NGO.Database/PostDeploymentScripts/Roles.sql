
SET IDENTITY_INSERT [dbo].[Roles] ON 
GO
INSERT INTO [dbo].[Roles]([Id],[Name]) VALUES  (1,'User')
GO
INSERT INTO [dbo].[Roles]([Id],[Name]) VALUES  (2,'Administrator')
GO
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO