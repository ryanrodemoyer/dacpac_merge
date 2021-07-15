CREATE TABLE [dbo].[Podcasts]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [PodcastName] NVARCHAR(50) NULL, 
    [ReleaseDate] DATETIME2 NULL
)

GO

CREATE INDEX [IX_Podcasts_PodcastName] ON [dbo].[Podcasts] ([PodcastName])
