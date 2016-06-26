using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFetcher.ApiResponseObjects
{
    public enum ItemType
    {
        job,
        story,
        comment,
        poll,
        pollopt
    }

    public class Item
    {
        public int id { get; set; } //The item's unique id.
        public bool deleted { get; set; } 	//true if the item is deleted.
        public ItemType type { get; set; } //The type of item.One of "job", "story", "comment", "poll", or "pollopt".
        public string by { get; set; } //The username of the item's author.
        public long time { get; set; } //Creation date of the item, in Unix Time.
        public string text { get; set; } //The comment, story or poll text. HTML.
        public bool dead { get; set; } //true if the item is dead.
        public string parent { get; set; } //The item's parent. For comments, either another comment or the relevant story. For pollopts, the relevant poll.
        public string kids { get; set; } //The ids of the item's comments, in ranked display order.
        public string url { get; set; } //The URL of the story.
        public int score { get; set; } //The story's score, or the votes for a pollopt.
        public string title { get; set; } //The title of the story, poll or job.
        public string parts { get; set; } //A list of related pollopts, in display order.
        public string descendants { get; set; } //In the case of stories or polls, the total comment count.

        public article ToDbEntity()
        {
            var article = new article
            {
                hn_article_id = id,
                create_datetime = DateTimeHelpers.UnixTimeStampToDateTime(time),
                content = text,
                url = url,
                score = score,
                title = title
            };

            return article;
        }
    }   
}
