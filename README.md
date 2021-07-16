# dacpac_merge

# Build Steps
1. Load the solution (`src\dacpacs.sln`) and build.
2. Open the LINQPad file (`src\dacpac_merge.linq`) and run.
3. Generate the deployment script by running the `scripts\sqlpackage_script.bat` file.
4. Open the file `scripts\merged.output.sql` to see the contents of the merged dacpacs.

# Attribution
Based on the code at https://github.com/GoEddie/DacpacMerge / https://the.agilesql.club/2019/03/how-can-we-merge-multiple-dacpacs-into-one/

# Merging Dacpacs
This merge strategy currently uses the "last in wins" methodology. Consider that `Database1.dacpac` is the "base" dacpac and then we overlay all user defined objects from `Database2.dacpac`. If an object is defined in both dacpacs then the object from the last dacpac is present in the merged dacpac.

Pre and Post deployment scripts are different. Each script is built during the initial build of Database1.dacpac and Database2.dacpac. Consider these files to be "compiled" and we cannot reflect in to individual scripts that were used to generate. In this case, our easiest option is to determine the order in which the scripts can run and then simply append them together in that order.

A secondary option exists if we could reliably parse the SQL script to extract the individual scripts. In this case, we could more intelligently merge the scripts together.

The final output is available at https://github.com/ryanrodemoyer/dacpac_merge/blob/main/scripts/merged.output.sql.

## Database1.dacpac
### dbo.Podcasts (net new)
```sql
CREATE TABLE [dbo].[Podcasts]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [PodcastName] NVARCHAR(50) NULL, 
    [ReleaseDate] DATETIME2 NULL
)

GO

CREATE INDEX [IX_Podcasts_PodcastName] ON [dbo].[Podcasts] ([PodcastName])
```

### dbo.Users (exists in both)
```sql
CREATE TABLE [dbo].[Users]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] VARCHAR(100) NULL, 
    [DateJoined] DATETIME NULL
)
```

### dbo.PodcastUpsert (net new, stored procedure)
```sql
CREATE PROCEDURE [dbo].[PodcastsUpsert]
	@podcastName nvarchar(50)
	, @releaseDate datetime2
AS
insert dbo.Podcasts (PodcastName,ReleaseDate)
values (@podcastName,@releaseDate)
```

### Script.PreDeployment.sql (Pre Deploy action)
```sql
print 'Database1 PreDeploy'
```

### Script.PostDeployment.sql (Post Deploy action)
```sql
print 'Database1 PostDeploy'
```

## Database2.dacpac

### dbo.Users (exists in both)
```sql
CREATE TABLE [dbo].[Users]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] VARCHAR(100) NULL, 
    [DateJoined] DATETIME NOT NULL, 
    [AccountExpires] DATETIME NULL
)
```

### dbo.VideoGames (net new)
```sql
CREATE TABLE [dbo].[VideoGames]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [VideoGameName] NVARCHAR(50) NULL, 
    [ReleaseYear] INT NULL, 
    [GameSystem] VARCHAR(20) NOT NULL, 
    [IsFPS] BIT NULL
)
```

### dbo.YoutubeChannels (net new)
```sql
CREATE TABLE [dbo].[YoutubeChannels]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ChannelName] NVARCHAR(50) NULL, 
    [Subscribers] INT NULL
)

GO

CREATE INDEX [IX_YoutubeChannels_ChannelName] ON [dbo].[YoutubeChannels] ([ChannelName])
```

### dbo.ChannelsUpsert (net new, stored procedure)
```sql
CREATE PROCEDURE [dbo].[ChannelsUpsert]
	@channelName nvarchar(50)
	, @subscribers int
AS
insert dbo.YoutubeChannels (ChannelName,Subscribers)
values (@channelName,@subscribers)
```

### dbo.VideoGamesInsert (net new, stored procedure)
```sql
CREATE PROCEDURE [dbo].[VideoGamesInsert]
	@videoGameName nvarchar(50)
	, @releaseYear int
	, @gameSystem varchar(20)
	, @isFps bit
AS
insert dbo.VideoGames (VideoGameName,ReleaseYear,GameSystem,IsFPS)
values (@videoGameName,@releaseYear,@gameSystem,@isFps)
```

### Script.PreDeployment.sql (Pre Deploy action)
```sql
print 'Database2 PreDeploy'
```

### Script.PostDeployment.sql (Post Deploy action)
```sql
print 'Database2 PostDeploy'
```