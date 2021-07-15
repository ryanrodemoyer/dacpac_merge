CREATE PROCEDURE [dbo].[VideoGamesInsert]
	@videoGameName nvarchar(50)
	, @releaseYear int
AS
insert dbo.VideoGames (VideoGameName,ReleaseYear)
values (@videoGameName,@releaseYear)

