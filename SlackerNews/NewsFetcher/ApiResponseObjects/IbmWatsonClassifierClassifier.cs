using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFetcher.ApiResponseObjects
{
    class IbmWatsonClassifierClassifier
    {
        [DeserializeAs(Name = "classifier_id")]
        public string ClassifierId { get; set; }

        [DeserializeAs(Name = "url")]
        public string ClassifierUrl { get; set; }

        [DeserializeAs(Name = "name")]
        public string Name { get; set; }

        [DeserializeAs(Name = "language")]
        public string Language { get; set; }

        [DeserializeAs(Name = "created")]
        public DateTime CreateDateTimeUtc { get; set; }
    }
}
