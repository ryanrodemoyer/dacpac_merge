CREATE TABLE [dbo].[VideoGames]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [VideoGameName] NVARCHAR(50) NULL, 
    [ReleaseYear] INT NULL, 
    [GameSystem] VARCHAR(20) NOT NULL, 
    [IsFPS] BIT NULL
)
