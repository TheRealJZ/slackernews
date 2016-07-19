using Common;
using NewsFetcher.ApiResponseObjects;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFetcher.Apis
{
    /// <summary>
    /// This class provides access to IBM Watson Natural Language Classifier Service.
    /// Docs are here:
    /// http://www.ibm.com/watson/developercloud/natural-language-classifier/api/v1/?curl#get_classifiers
    /// Playground is here:
    /// https://watson-api-explorer.mybluemix.net/apis/natural-language-classifier-v1#/
    /// </summary>
    class IbmWatsonClassifierApi
    {
        string API_URL = Settings.Get(Settings.AppSettingKeys.IbmWatsonClassifierApiUrl);
        string API_USERNAME = Settings.Get(Settings.AppSettingKeys.IbmWatsonClassifierApiUsername);
        string API_PASSWORD = Settings.Get(Settings.AppSettingKeys.IbmWatsonClassifierApiPassword);
        string API_CLASSIFIER_ID = Settings.Get(Settings.AppSettingKeys.IbmWatsonClassifierApiClassifierId);

        string ENDPOINT_CREATE = "/v1/classifiers";
        string ENDPOINT_LIST = "/v1/classifiers";
        string ENDPOINT_CLASSIFY;
        
        public enum ClassifierStatus
        {
            NonExistent,
            Training,
            Failed,
            Available,
            Unavailable
        }

        byte[] DefaultClassifierMetaData
        {
            get
            {
                return Encoding.ASCII.GetBytes(SimpleJson.SerializeObject(new { language = "en" }));
            }
        }

        public IbmWatsonClassifierApi()
        {
            ENDPOINT_CLASSIFY = $"/v1/classifiers/{API_CLASSIFIER_ID}/classify";
        }

        private void AddCredentials(RestClient client)
        {
            client.Authenticator = new HttpBasicAuthenticator(API_USERNAME, API_PASSWORD);
        }
        
        public IbmWatsonClassifierClassifyResponse Classify(string text)
        {
            string sanitizedText = text?.Trim();
            if(string.IsNullOrWhiteSpace(sanitizedText))
            {
                throw new Exception("Invalid text: blank");
            }

            var client = new RestClient(API_URL);
            AddCredentials(client);

            var request = new RestRequest(ENDPOINT_CLASSIFY, Method.GET);
            request.AddParameter("text", sanitizedText, ParameterType.GetOrPost);

            var response = client.Execute<IbmWatsonClassifierClassifyResponse>(request);
            if (response.ErrorException != null)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(response.ErrorException);
                throw response.ErrorException;
            }

            return response.Data;
        }

        public IbmWatsonClassifierCreateResponse CreateClassifierWithDataFile(string trainingDataCsvPath)
        {
            NLog.LogManager.GetCurrentClassLogger().Trace($"Creating classifier using file at " + trainingDataCsvPath);

            if(!System.IO.File.Exists(trainingDataCsvPath))
            {
                throw new Exception("Invalid training data file path: " + trainingDataCsvPath);
            }

            var client = new RestClient(API_URL);
            AddCredentials(client);
            
            var request = new RestRequest(ENDPOINT_CREATE, Method.POST);           
            request.AddFile("training_metadata", DefaultClassifierMetaData, "training_metadata.json");
            request.AddFile("training_data", trainingDataCsvPath, "text/csv");

            var response = client.Execute<IbmWatsonClassifierCreateResponse>(request);
            if (response.ErrorException != null)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(response.ErrorException);
                throw response.ErrorException;
            }

            return response.Data;
        }
        
        public List<IbmWatsonClassifierClassifier> ListClassifiers()
        {
            var client = new RestClient(API_URL);
            AddCredentials(client);

            var request = new RestRequest(ENDPOINT_LIST, Method.GET);
            
            var response = client.Execute<IbmWatsonClassifierListResponse>(request);

            if (response.ErrorException != null)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(response.ErrorException);
                throw response.ErrorException;
            }

            return response.Data.Classifiers;
        }
    }
}
