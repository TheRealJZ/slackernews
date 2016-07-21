using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFetcher.ApiResponseObjects
{
    class IbmWatsonClassifierListResponse
    {
        [DeserializeAs(Name = "classifiers")]
        public List<IbmWatsonClassifierClassifier> Classifiers { get; set; }
    }
}
