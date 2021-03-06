Use SlackerNews
GO

ALTER TABLE sections
ADD			seo_name VARCHAR(100)
GO

update sections
SET	seo_name = CASE
	WHEN id = 1 THEN 'general'
	WHEN id = 2 THEN 'product-announcements'
	WHEN id = 3 THEN 'business'
	WHEN id = 4 THEN 'show-hn'
	WHEN id = 5 THEN 'ask-hn'
	WHEN id = 6 THEN 'persuasion'
	WHEN id = 7 THEN 'science'
	WHEN id = 8 THEN 'world-news'
	WHEN id = 9 THEN 'security-alerts'
END
GO

ALTER TABLE sections
ALTER COLUMN seo_name VARCHAR(100) NOT NULL
GO

create table watson_sections(
	watson_class_name VARCHAR(300) PRIMARY KEY,
	section_id INT NOT NULL FOREIGN KEY references sections(id)
)
GO

INSERT INTO watson_sections(watson_class_name, section_id)
VALUES	('surveillance',1),
		('databases',1),
		('hiring',1),
		('scalability',1),
		('releases',2),
		('business',3),
		('legal',3),
		('show-hn',4),
		('ask-hn',5),
		('persuasion',6),
		('science',7),
		('tech-advancements',7),
		('world-news',8),
		('outages',9),
		('security-exploits',9)
GO

SELECT ws.watson_class_name,
		s.name AS 'slackernews_section_name'
FROM	watson_sections ws
JOIn	sections s ON (ws.section_id = s.id)
ORDER BY s.display_order,
		ws.watson_class_name ASC