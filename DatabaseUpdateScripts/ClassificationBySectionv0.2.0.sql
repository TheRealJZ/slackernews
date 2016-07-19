Use [SlackerNews]
GO

CREATE TABLE [sections](
	[id] INT PRIMARY KEY IDENTITY,
	[name] VARCHAR(100) NOT NULL UNIQUE,
	[display_order] INT NOT NULL UNIQUE
)
GO

INSERT INTO [sections]([name],[display_order])
VALUES		('General',1),
			('Product Announcements',2),
			('Business',3),
			('Show HN',4),
			('Ask HN',5),
			('Persuasion',6),
			('Science/Future Tech',7),
			('World News',8),
			('Outages/Security Exploits',9)
GO

ALTER TABLE articles
ADD			[section_id] INT
GO

ALTER TABLE articles
ADD	CONSTRAINT FK_articles_section
FOREIGN KEY (section_id) 
REFERENCES sections(id)
GO

ALTER TABLE articles
ADD			api_fetch_date_classification DATETIME
GO

ALTER TABLE articles
ADD			api_fetch_date_classification DATETIME
GO