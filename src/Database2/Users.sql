CREATE TABLE [dbo].[Users]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] VARCHAR(100) NULL, 
    [DateJoined] DATETIME NOT NULL, 
    [AccountExpires] DATETIME NULL
)
