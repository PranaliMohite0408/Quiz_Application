SET IDENTITY_INSERT [dbo].[Category] ON
INSERT INTO [dbo].[Category] ([Cat_Id], [Cat_Name], [Id]) VALUES (15, N'sdf', 1)
SET IDENTITY_INSERT [dbo].[Category] OFF
delete from [Category];
SET IDENTITY_INSERT [dbo].[Category] ON
INSERT INTO [dbo].[Category] ([Cat_Id], [Cat_Name], [Id]) VALUES (1, N'sdf', 1)
SET IDENTITY_INSERT [dbo].[Category] OFF