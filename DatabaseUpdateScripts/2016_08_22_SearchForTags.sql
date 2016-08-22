Use SlackerNews
GO

ALTER TABLE sections
ADD seo_route VARCHAR(100)
GO

UPDATE sections
SET seo_route = 'sections/' + CASE
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
