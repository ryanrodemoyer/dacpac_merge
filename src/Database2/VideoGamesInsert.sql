CREATE PROCEDURE [dbo].[VideoGamesInsert]
	@videoGameName nvarchar(50)
	, @releaseYear int
	, @gameSystem varchar(20)
	, @isFps bit
AS
insert dbo.VideoGames (VideoGameName,ReleaseYear,GameSystem,IsFPS)
values (@videoGameName,@releaseYear,@gameSystem,@isFps)

