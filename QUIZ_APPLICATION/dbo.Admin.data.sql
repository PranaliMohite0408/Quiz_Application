SET IDENTITY_INSERT [dbo].[Admin] ON
INSERT INTO [dbo].[Admin] ([Id], [Name], [Password]) VALUES (3, N'aa', N'a123')
SET IDENTITY_INSERT [dbo].[Admin] OFF
delete from [Admin];

SET IDENTITY_INSERT [dbo].[Admin] ON
INSERT INTO [dbo].[Admin] ([Id], [Name], [Password]) VALUES (1, N'aa', N'a123')
SET IDENTITY_INSERT [dbo].[Admin] OFF
select * from [Admin];