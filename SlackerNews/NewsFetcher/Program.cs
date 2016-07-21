using RestSharp;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using NewsFetcher.ApiResponseObjects;
using System.Net;
using NewsFetcher.Apis;

namespace NewsFetcher
{
    class Program
    {
        readonly static string HackernewsApiUrl = Settings.Get(Settings.AppSettingKeys.HackernewsApiUrl);
        
        static void Main(string[] args)
        {
            UpdateStatsForRecentArticles(24);
            GetAndStoreMissingArticles(1000);

            // Testing -------
            //GetSemantriaSummaryForUrl("foo");
            //GetAlchemyTagsForUrl("http://herokeys.com/why-link-building-is-important-in-seo/");
            //UpdateStatsForArticle(11980581);
            //var api = new IbmWatsonClassifierApi();
            //string id = api.CreateClassifierWithDataFile("C:\\GitHub\\Projects\\Personal\\SlackerNewsLogins\\IBMWatsonNaturalLanguageClassifierTrainingDatav0.2.0.csv");
            //var classifier = new SectionClassifier();
            //var section = classifier.GetSectionFromText("Foo 29.0.0 is out");
            //int x = 5;
        }

        #region Fetching New

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

        static void GetAndStoreArticle(int hnArticleId)
        {
            NLog.LogManager.GetCurrentClassLogger().Trace($"Getting object id {hnArticleId} from {HackernewsApiUrl}");

            var client = new RestClient(HackernewsApiUrl);
            var request = new RestRequest($"item/{hnArticleId}.json");
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
                    var dbEntity = response.Data.ToDbEntity();

                    using (var context = new SlackerNewsEntities())
                    {
                        if (context.articles.Any(t => t.hn_article_id == dbEntity.hn_article_id))
                        {
                            NLog.LogManager.GetCurrentClassLogger().Trace($"Object id {hnArticleId} already exists");
                            throw new Exception("Article already downloaded: " + dbEntity.hn_article_id);
                        }

                        NLog.LogManager.GetCurrentClassLogger().Trace($"Saving object id {hnArticleId} to database");
                        var article = response.Data.ToDbEntity();
                        article.section_id = (int)Constants.Section.General;
                        context.articles.Add(article);
                        context.SaveChanges();
                    }
                }
                else
                {
                    NLog.LogManager.GetCurrentClassLogger().Trace($"Object id {hnArticleId} is not a story, type is {response.Data.type}");
                }
            }
        }
        
        #endregion

        #region Updating Existing

        static void UpdateStatsForRecentArticles(int hoursSinceArticleCreated = 240)
        {
            int maxLocalArticleId = GetMaxArticleIdInLocalStore();
            int minLocalArticleIdInLastXHours = GetMinArticleIdCreatedLastXHoursInLocalStore(hoursSinceArticleCreated);

            // Only update recent articles
            if (minLocalArticleIdInLastXHours != 0)
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

                if (articleIds.Any())
                {
                    foreach (int i in articleIds)
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
        
        static void UpdateStatsForArticle(int hnArticleId)
        {
            NLog.LogManager.GetCurrentClassLogger().Trace($"Updating stats for article {hnArticleId}");

            var client = new RestClient(HackernewsApiUrl);
            var request = new RestRequest($"item/{hnArticleId}.json");
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

                        // Update article with section
                        UpdateSectionForArticle((int)dbEntity.hn_article_id);

                        //SetSemanticAttributesForArticle((int)article.hn_article_id);
                    }
                }
                else
                {
                    throw new Exception($"ObjectId {hnArticleId} is not a story, cannot update stats");
                }
            }
        }
                
        static void UpdateSectionForArticle(int hnArticleId)
        {
            NLog.LogManager.GetCurrentClassLogger().Info("Updating section for article " + hnArticleId);

            using (var context = new SlackerNewsEntities())
            {
                var article = context.articles.Single(t => t.hn_article_id == hnArticleId);

                if (article.score > Constants.ScoreThresholdForClassificationApi
                    && article.api_fetch_date_classification == null)
                {
                    var classifier = new SectionClassifier();
                    var section = classifier.GetSectionFromText(article.title);
                    article.section_id = (int)section;
                    article.api_fetch_date_classification = DateTime.UtcNow;
                    context.SaveChanges();
                }
            }
        }

        static void UpdateSemanticAttributesForArticle(int hnArticleId)
        {
            NLog.LogManager.GetCurrentClassLogger().Info("Getting semantic attributes for article " + hnArticleId);

            using (var context = new SlackerNewsEntities())
            {
                var article = context.articles.Single(t => t.hn_article_id == hnArticleId);
                               
                if(article.score < Constants.ScoreThresholdForSemanticSummaryApi)
                {
                    NLog.LogManager.GetCurrentClassLogger().Info($"Article score is {article.score}. We only run API calls to get semantic data when score exceeds {Constants.ScoreThresholdForSemanticSummaryApi}");
                    return;
                }

                if (!IsUrlValid(article.url))
                {
                    throw new Exception("Invalid url");
                }

                if (string.IsNullOrWhiteSpace(article.tags))
                {
                    try
                    {
                        article.tags = GetAlchemyTagsForUrl(article.url);
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        // Keep going so we can try the semantic summary too
                        NLog.LogManager.GetCurrentClassLogger().Error(ex);
                    }
                }
                else { 
                    NLog.LogManager.GetCurrentClassLogger().Info("Article already has tags, continuing");
                }

                if(string.IsNullOrWhiteSpace(article.semantic_summary))
                {
                    try
                    {
                        article.semantic_summary = GetSemantriaSummaryForUrl(article.url);
                        context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        NLog.LogManager.GetCurrentClassLogger().Error(ex);
                    }
                }
                else
                {
                    NLog.LogManager.GetCurrentClassLogger().Info("Article already has summary, continuing");
                }
            }
        }

        static bool IsUrlValid(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest("");
            var response = client.Execute(request);
            return response.StatusCode == HttpStatusCode.OK;
        }

        static string GetAlchemyTagsForUrl(string url)
        {
            string alchemyApiUrl = Settings.Get(Settings.AppSettingKeys.AlchemyApiUrl);
            string alchemyApiKey = Settings.Get(Settings.AppSettingKeys.AlchemyApiKey);

            var client = new RestSharp.RestClient(alchemyApiUrl);

            var request = new RestRequest("/url/URLGetRankedKeywords", Method.GET);
            request.Parameters.Add(new Parameter { Name = "apikey", Value = alchemyApiKey, Type = ParameterType.UrlSegment });
            request.Parameters.Add(new Parameter { Name = "outputMode", Value = "json", Type = ParameterType.UrlSegment });
            request.Parameters.Add(new Parameter { Name = "showSourceText", Value = "1", Type = ParameterType.UrlSegment });
            request.Parameters.Add(new Parameter { Name = "url", Value = url, Type = ParameterType.UrlSegment });

            var response = client.Execute(request);

            if (response.ErrorException != null)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(response.ErrorException);
                throw response.ErrorException;
            }
            else
            {
                return null;
            }
        }

        static string GetSemantriaSummaryForUrl(string url)
        {
            string articleText = @"
Should you encrypt or compress first?
25 June 2016

Imagine this:

You work for a big company.Your job is pretty boring.Frankly, your talents are wasted writing boilerplate code for an application whose only users are three people in accounting who can’t stand the sight of you.

Your real passion is security.You read r / netsec every day and participate in bug bounties after work.For the past three months, you’ve been playing a baroque stock trading game that you’re winning because you found a heap - based buffer overflow and wrote some AVR shellcode to help you pick stocks.

Everything changes when you discover that what you had thought was a video game was actually a cleverly disguised recruitment tool.Mont Piper, the best security consultancy in the world, is hiring — and you just landed an interview!

A classic security interview question!

Take a second and think about it.

At a high level, compression tries to use patterns in data in order to reduce its size.Encryption tries to shuffle data in such a way that without the key, you can’t find any patterns in the data at all.
aintext in any way? Is this kind of attack still possible ?” he asks.";

            var semantria = new SemantriaApi();
            return semantria.Summarize(articleText);
        }

        #endregion

        #region Synchronization Status

        static int GetMaxArticleIdFromRemote()
        {
            var client = new RestClient(HackernewsApiUrl);
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
        
        static int GetMinArticleIdCreatedLastXHoursInLocalStore(int hoursSinceArticleCreated)
        {
            using (var context = new SlackerNewsEntities())
            {
                DateTime createdSince = DateTime.Now.AddHours(-hoursSinceArticleCreated);
                return context.articles.Where(t => t.create_datetime > createdSince).Min(t => t.hn_article_id) ?? 0;
            }
        }

        #endregion
    }
}
