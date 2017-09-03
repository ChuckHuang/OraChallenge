
/****** Object:  Table [dbo].[Expense]   ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE DATABASE  OraChallengeDB;
GO

Use [OraChallengeDB]
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
	))

GO

SET IDENTITY_INSERT [dbo].[User] ON 

GO
INSERT INTO [dbo].[User] ([Id], [UserName] ) VALUES (1,'SampleUserName1')
GO
INSERT INTO [dbo].[User] ([Id], [UserName] ) VALUES (2,'SampleUserName2')
GO
INSERT INTO [dbo].[User] ([Id], [UserName] ) VALUES (3,'SampleUserName3')
GO
INSERT INTO [dbo].[User] ([Id], [UserName] ) VALUES (4,'SampleUserName4')
GO
INSERT INTO [dbo].[User] ([Id], [UserName] ) VALUES (5,'SampleUserName5')
GO
INSERT INTO [dbo].[User] ([Id], [UserName] ) VALUES (6,'SampleUserName6')
GO
INSERT INTO [dbo].[User] ([Id], [UserName]) VALUES (7,'SampleUserName7')
GO
INSERT INTO [dbo].[User] ([Id], [UserName] ) VALUES (8,'SampleUserName8')
GO
INSERT INTO [dbo].[User] ([Id], [UserName] ) VALUES (9,'SampleUserName9')
GO
INSERT INTO [dbo].[User] ([Id], [UserName]) VALUES (10,'SampleUserName10')
GO
INSERT INTO [dbo].[User] ([Id], [UserName] ) VALUES (11,'SampleUserName11')
GO
INSERT INTO [dbo].[User] ([Id], [UserName] ) VALUES (12,'SampleUserName12')
GO
SET IDENTITY_INSERT [dbo].[User] OFF
GO

CREATE TABLE [dbo].[Message](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int]  NOT NULL,
	[Description] [nvarchar](100) NOT NULL,
	[Date] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
	),
CONSTRAINT FK_UserMessage FOREIGN KEY ([UserId]) REFERENCES [dbo].[User](Id)
)
GO

SET IDENTITY_INSERT [dbo].[Message] ON 

GO
INSERT INTO [dbo].[Message] ([Id], [UserId] ,[Description],[Date]) VALUES (1,1,'SampleMessage1',CAST(N'2017-01-01' AS Date))
GO
INSERT INTO [dbo].[Message] ([Id], [UserId] ,[Description],[Date]) VALUES (2,2,'SampleMessage2',CAST(N'2017-01-02' AS Date))
GO
INSERT INTO [dbo].[Message] ([Id], [UserId] ,[Description],[Date]) VALUES (3,3,'SampleMessage3',CAST(N'2017-01-03' AS Date))
GO
INSERT INTO [dbo].[Message] ([Id], [UserId] ,[Description],[Date]) VALUES (4,4,'SampleMessage4',CAST(N'2017-01-04' AS Date))
GO
INSERT INTO [dbo].[Message] ([Id], [UserId] ,[Description],[Date]) VALUES (5,5,'SampleMessage5',CAST(N'2017-01-05' AS Date))
GO
INSERT INTO [dbo].[Message] ([Id], [UserId] ,[Description],[Date]) VALUES (6,6,'SampleMessage6',CAST(N'2017-01-06' AS Date))
GO
INSERT INTO [dbo].[Message] ([Id], [UserId] ,[Description],[Date]) VALUES (7,7,'SampleMessage7',CAST(N'2017-01-07' AS Date))
GO
INSERT INTO [dbo].[Message] ([Id], [UserId] ,[Description],[Date]) VALUES (8,8,'SampleMessage8',CAST(N'2017-01-08' AS Date))
GO
INSERT INTO [dbo].[Message] ([Id], [UserId] ,[Description],[Date]) VALUES (9,9,'SampleMessage9',CAST(N'2017-01-09' AS Date))
GO
INSERT INTO [dbo].[Message] ([Id], [UserId] ,[Description],[Date]) VALUES (10,10,'SampleMessage10',CAST(N'2017-01-10' AS Date))
GO
INSERT INTO [dbo].[Message] ([Id], [UserId] ,[Description],[Date]) VALUES (11,11,'SampleMessage11',CAST(N'2017-01-11' AS Date))
GO
INSERT INTO [dbo].[Message] ([Id], [UserId] ,[Description],[Date]) VALUES (12,12,'SampleMessage12',CAST(N'2017-01-12' AS Date))
GO
SET IDENTITY_INSERT [dbo].[Message] OFF
GO
