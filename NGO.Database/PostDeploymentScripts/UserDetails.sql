SET IDENTITY_INSERT [dbo].[UserDetails] ON 
GO
INSERT INTO [dbo].[UserDetails]
           ([Id]
           ,[uniqueId]
           ,[UserId]
           ,[OrgId]
           ,[ChapterId]
           ,[FirstName]
           ,[LastName]
           ,[FamilyName]
           ,[PhoneNumber]
           ,[City]
           ,[State]
           ,[WhatsAppNumber]
           ,[Address]
           ,[PostalCode]
           ,[Country]
           ,[HomeTown]
           ,[SocialMedia]
           ,[PreferredBy]
           ,[DOB]
           ,[Photo]
           ,[SpouseFirstName]
           ,[SpouseLastName]
           ,[SpouseDOB]
           ,[SpouseEmail]
           ,[SpouseCity]
           ,[SpouseState]
           ,[SpouseCountry]
           ,[SpouseHometown]
           ,[Reason]
           ,[CreatedBy]
           ,[CreatedOn]
           ,[UpdatedBy]
           ,[UpdatedOn]
           ,[IsDeleted])
     VALUES
           (1
           ,null
           ,1
           ,null
           ,null
           ,'Admin'
           ,'Admin'
           ,null
           ,'9898989890'
           ,'qwwwww'
           ,'karnataka'
           ,'9898989890'
           ,'ddsdsdsds'
           ,'11222234455'
           ,'India'
           ,'dsdssd'
           ,'ddsds'
           ,'ddsdsds'
           ,CAST(N'2022-01-31 13:06:18.647' AS DateTime)
           ,null
           ,'xddsdsds'
           ,'ewewewewew'
           ,CAST(N'2022-01-31 13:06:18.647' AS DateTime)
           ,'parjanyakumar333@gmail.com'
           ,'dsdsdsds'
           ,'fdsdsdsdss'
           ,'ewewewewee'
           ,'fdfff'
           ,'eweweweww'
           ,1
           ,CAST(N'2022-01-31 13:06:18.647' AS DateTime)
           ,1
           ,CAST(N'2022-01-31 13:06:18.647' AS DateTime)
           ,0)
GO
SET IDENTITY_INSERT [dbo].[UserDetails] OFF
GO