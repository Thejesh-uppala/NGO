SET IDENTITY_INSERT [dbo].[Users] ON 
GO
INSERT INTO [dbo].[Users]
           ([Id]
           ,[Name]
           ,[ContactNumber]
           ,[Email]
           ,[Password]
           ,[Status]
           ,[UnsuccessfulLoginAttempts]
           ,[LastLogin]
           ,[CreatedBy]
           ,[CreatedOn]
           ,[UpdatedBy]
           ,[UpdatedOn]
           ,[IsDeleted])
     VALUES
           (1
           ,'Admin'
           ,'7878787878'
           ,'parjanyakumar333@gmail.com'
           ,'tlwmK7lwICTYwqRIkxo+Ru2iOz2f+e9nOWk3vo1MERs='
           ,1
           ,0
           ,NULL
           ,-1
           ,CAST(N'2022-01-31 13:06:18.647' AS DateTime)
           ,-1
           , CAST(N'2022-01-31 13:06:37.440' AS DateTime)
           ,0)
GO
SET IDENTITY_INSERT [dbo].[Users] OFF
GO