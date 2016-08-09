Use SlackerNews
GO

create table slacker_weekly_issues (
	id INT PRIMARY KEY IDENTITY,
	create_datetime DATETIME NOT NULL,
	for_week_starting_datetime DATETIME NOT NULL,
	mailchimp_campaign_id VARCHAR(32) NULL
)
GO

CREATE TABLE slacker_weekly_issue_articles (
	issue_id INT FOREIGN KEY references slacker_weekly_issues,
	article_id INT FOREIGN KEY references articles,
	section_id INT NULL FOREIGN KEY references sections
)
GO