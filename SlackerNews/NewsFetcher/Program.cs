using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using NewsFetcher.ApiResponseObjects;

namespace NewsFetcher
{
    class Program
    {
        const string ApiUrl = "https://hacker-news.firebaseio.com/v0/";

        static void Main(string[] args)
        {
            UpdateStatsForRecentArticles();
            GetAndStoreMissingArticles(1000);
        }

        static void GetAndStoreMissingArticles(int? max = null)
        {
            int maxLocalArticleId = GetMaxArticleIdInLocalStore();
            int maxRemoteArticleId = GetMaxArticleIdFromRemote();

            int startingArticleId = maxLocalArticleId;

            if(max != null && maxRemoteArticleId - startingArticleId > (int)max)
            {
                NLog.LogManager.GetCurrentClassLogger().Warn("Tried to pull more articles than maximum allowed: " + (int)max);
                startingArticleId = maxRemoteArticleId - (int)max;
            }

            for (int i = startingArticleId + 1; i <= maxRemoteArticleId; i++)
            {
                try
                {
                    GetAndStoreArticle(i);
                }
                catch (Exception ex)
                {
                    // Log and continue
                    NLog.LogManager.GetCurrentClassLogger().Error(ex);
                }
            }
        }
        
        static void UpdateStatsForRecentArticles(int hoursSinceArticleCreated = 240)
        {
            int maxLocalArticleId = GetMaxArticleIdInLocalStore();
            int minLocalArticleIdInLastXHours = GetMinArticleIdCreatedLastXHoursInLocalStore(hoursSinceArticleCreated);

            // Only update recent articles
            if(minLocalArticleIdInLastXHours != 0)
            {
                List<int> articleIds;

                // Only run API queries for the subset of objects that are actually articles
                // Furthermore, only run update stats for articles we have successfully saved to the database
                using (var context = new SlackerNewsEntities())
                {
                    articleIds = context.
                        articles.
                        Where(t => t.hn_article_id >= minLocalArticleIdInLastXHours).
                        Select(t => (int)t.hn_article_id).
                        ToList();
                }

                if(articleIds.Any())
                {
                    foreach(int i in articleIds)
                    {
                        try
                        {
                            UpdateStatsForArticle(i);
                        }
                        catch (Exception ex)
                        {
                            // Log and continue
                            NLog.LogManager.GetCurrentClassLogger().Error(ex);
                        }
                    }
                }
            }
        }

        static int GetMinArticleIdCreatedLastXHoursInLocalStore(int hoursSinceArticleCreated)
        {
            using (var context = new SlackerNewsEntities())
            {
                DateTime createdSince = DateTime.Now.AddHours(-hoursSinceArticleCreated);
                return context.articles.Where(t => t.create_datetime > createdSince).Min(t => t.hn_article_id) ?? 0;
            }
        }

        static void UpdateStatsForArticle(int articleId)
        {
            NLog.LogManager.GetCurrentClassLogger().Trace($"Updating stats for article {articleId}");

            var client = new RestClient(ApiUrl);
            var request = new RestRequest($"item/{articleId}.json");
            var response = client.Execute<Item>(request);

            if (response.ErrorException != null)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(response.ErrorException);
                throw response.ErrorException;
            }
            else
            {
                if (response.Data.type == ItemType.story)
                {
                    using (var context = new SlackerNewsEntities())
                    {
                        var dbEntity = context.articles.Single(t => t.hn_article_id == response.Data.id);
                        dbEntity.score = response.Data.score;
                        context.SaveChanges();
                    }
                }
                else
                {
                    throw new Exception($"ObjectId {articleId} is not a story, cannot update stats");
                }
            }
        }

        static int GetMaxArticleIdFromRemote()
        {
            var client = new RestClient(ApiUrl);
            var request = new RestRequest("maxitem.json");
            var response = client.Execute<int>(request);
            return response.Data;
        }

        static int GetMaxArticleIdInLocalStore()
        {
            using (var context = new SlackerNewsEntities())
            {
                return context.articles.Max(t => t.hn_article_id) ?? 0;
            }
        }

        static void GetAndStoreArticle(int articleId)
        {
            NLog.LogManager.GetCurrentClassLogger().Trace($"Getting object id {articleId} from {ApiUrl}");

            var client = new RestClient(ApiUrl);
            var request = new RestRequest($"item/{articleId}.json");
            var response = client.Execute<Item>(request);

            if(response.ErrorException != null)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(response.ErrorException);
                throw response.ErrorException;
            }
            else
            {
                if(response.Data.type == ItemType.story)
                {
                    var dbEntity = response.Data.ToDbEntity();

                    using (var context = new SlackerNewsEntities())
                    {
                        if(context.articles.Any(t => t.hn_article_id == dbEntity.hn_article_id ))
                        {
                            NLog.LogManager.GetCurrentClassLogger().Trace($"Object id {articleId} already exists");
                            throw new Exception("Article already downloaded: " + dbEntity.hn_article_id);
                        }

                        NLog.LogManager.GetCurrentClassLogger().Trace($"Saving object id {articleId} to database");
                        context.articles.Add(response.Data.ToDbEntity());
                        context.SaveChanges();
                    }
                }
                else
                {
                    NLog.LogManager.GetCurrentClassLogger().Trace($"Object id {articleId} is not a story, type is {response.Data.type}");
                }
            }
        }
    }
}
