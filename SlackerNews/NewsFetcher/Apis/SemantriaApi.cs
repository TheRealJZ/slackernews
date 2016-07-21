using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Semantria.Com;
using Semantria.Com.Serializers;

namespace NewsFetcher.Apis
{
    class SemantriaApi
    {
        string API_KEY = Settings.Get(Settings.AppSettingKeys.SemantriaApiKey);
        string API_SECRET = Settings.Get(Settings.AppSettingKeys.SemantriaApiSecret);
        string API_CONFIGURATION = Settings.Get(Settings.AppSettingKeys.SemantriaApiConfiguration);
        
        internal string Summarize(string text)
        {
            if(string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            // Check for exceeding string length!
                       
            using ( Session session = Session.CreateSession(API_KEY, API_SECRET, API_CONFIGURATION))
            {
                session.Error += new Session.ErrorHandler(delegate (object sender, ResponseErrorEventArgs ea)
                {
                    var exception = new Exception("Error in fetching summary data from Semantria API");
                    exception.Data["Type"] = ea.Type;
                    exception.Data["Message"] = ea.Message;
                    exception.Data["Status"] = ea.Status;
                    NLog.LogManager.GetCurrentClassLogger().Error(exception);    
                });

                // A dictionary that keeps IDs of sent documents and their statuses. It's required to make sure that we get correct documents from the API.
                Dictionary<string, dynamic> docsTracker = new Dictionary<string, dynamic>(4);

                //Obtaining subscription object to get user limits applicable on server side
                dynamic subscription = session.GetSubscription();


                // Queueing requests
                // ----------------------
                string docId = Guid.NewGuid().ToString();
                dynamic doc = new
                {
                    id = docId,
                    text = text
                };

                int responseCode = session.QueueDocument(doc);
                if (responseCode == -1)
                {
                    NLog.LogManager.GetCurrentClassLogger().Error("Error queueing document for processing");
                }
                else
                {
                    docsTracker.Add(docId, "QUEUED");
                    Console.WriteLine(string.Format("Document queued successfully."));
                }
                
                // Fetching Processed Results
                // ----------------------
                // As Semantria isn't real-time solution you need to wait some time before getting of the processed results
                // In real application here can be implemented two separate jobs, one for queuing of source data another one for retreiving
                // Wait ten seconds while Semantria process queued document
                List<dynamic> results = new List<dynamic>();
                while (docsTracker.Any(item => item.Value == "QUEUED"))
                {
                    System.Threading.Thread.Sleep(500);

                    // Requests processed results from Semantria service
                    Console.WriteLine("Retrieving your processed results...");
                    dynamic incomingBatch = session.GetProcessedDocuments();

                    foreach (dynamic item in incomingBatch)
                    {
                        if (docsTracker.ContainsKey(item.id))
                        {
                            docsTracker[item.id] = item.status;
                            results.Add(item);
                        }
                    }
                }

                foreach (dynamic data in results)
                {
                    // Printing of document sentiment score
                    Console.WriteLine(string.Format("Document {0}. Sentiment score: {1}", data.id, data.sentiment_score));

                    // Printing of intentions
                    if (data.auto_categories != null && data.auto_categories.Count > 0)
                    {
                        Console.WriteLine("Document categories:");
                        foreach (dynamic category in data.auto_categories)
                            Console.WriteLine(string.Format("\t{0} (strength: {1})", category.title, category.strength_score));
                    }

                    // Printing of document themes
                    if (data.themes != null && data.themes.Count > 0)
                    {
                        Console.WriteLine("Document themes:");
                        foreach (dynamic theme in data.themes)
                            Console.WriteLine(string.Format("\t{0} (sentiment: {1})", theme.title, theme.sentiment_score));
                    }

                    // Printing of document entities
                    if (data.entities != null && data.entities.Count > 0)
                    {
                        Console.WriteLine("Entities:");
                        foreach (dynamic entity in data.entities)
                            Console.WriteLine(string.Format("\t{0} : {1} (sentiment: {2})", entity.title, entity.entity_type, entity.sentiment_score));
                    }

                    Console.WriteLine();
                }

                return "";
            }
        }
    }
}
