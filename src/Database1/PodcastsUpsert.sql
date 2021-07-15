CREATE PROCEDURE [dbo].[PodcastsUpsert]
	@podcastName nvarchar(50)
	, @releaseDate datetime2
AS
insert dbo.Podcasts (PodcastName,ReleaseDate)
values (@podcastName,@releaseDate)

