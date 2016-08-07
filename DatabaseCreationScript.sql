-- --------------------
-- Author: John Zumbrum
-- Date: 06/25/2016
-- --------------------
IF NOT EXISTS(SELECT * FROM sys.databases WHERE name='SlackerNews')
	CREATE DATABASE SlackerNews
GO

Use SlackerNews
GO

IF NOT EXISTS(SELECT * FROM sys.tables WHERE name='articles')
	CREATE TABLE articles (
		id INT IDENTITY PRIMARY KEY,
		hn_article_id INT NOT NULL UNIQUE,
		title NVARCHAR(MAX),
		content NVARCHAR(MAX),
		create_datetime DATETIME NOT NULL,
		score INT NOT NULL DEFAULT 0,
		num_comments INT NOT NULL DEFAULT 0,
		url NVARCHAR(MAX),
		top_comment_text NVARCHAR(MAX),
		semantic_summary NVARCHAR(MAX),
		tags NVARCHAR(MAX)
	)
GO

CREATE LOGIN slackernews_user WITH PASSWORD = 'Farfignuegen3'
Use SlackerNews
GO
CREATE USER slackernews_user FOR LOGIN slackernews_user
EXECUTE sp_addrolemember N'db_datawriter', N'slackernews_user'
EXECUTE sp_addrolemember N'db_datareader', N'slackernews_user'

-- make sure to enable sql server logins for your SQL Server instance!