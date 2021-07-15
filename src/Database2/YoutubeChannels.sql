CREATE TABLE [dbo].[YoutubeChannels]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ChannelName] NVARCHAR(50) NULL, 
    [Subscribers] INT NULL
)

GO

CREATE INDEX [IX_YoutubeChannels_ChannelName] ON [dbo].[YoutubeChannels] ([ChannelName])
