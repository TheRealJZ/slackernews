using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewsFetcher.Apis;
using RestSharp.Deserializers;

namespace NewsFetcher.ApiResponseObjects
{
    class IbmWatsonClassifierCreateResponse
    {
        [DeserializeAs(Name = "classifier_id")]
        public string ClassifierId { get; set; }

        [DeserializeAs(Name = "name")]
        public string Name { get; set; }

        [DeserializeAs(Name = "language")]
        public string Language { get; set; }

        [DeserializeAs(Name = "created")]
        public DateTime CreateDateTimeUtc { get; set; }

        [DeserializeAs(Name = "url")]
        public string ClassifierUrl { get; set; }

        [DeserializeAs(Name = "status")]
        public IbmWatsonClassifierApi.ClassifierStatus Status { get; set; }

        [DeserializeAs(Name = "status_description")]
        public string StatusDescription { get; set; }
    }
}
