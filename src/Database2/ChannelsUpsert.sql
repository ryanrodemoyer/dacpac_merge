CREATE PROCEDURE [dbo].[ChannelsUpsert]
	@channelName nvarchar(50)
	, @subscribers int
AS
insert dbo.YoutubeChannels (ChannelName,Subscribers)
values (@channelName,@subscribers)

