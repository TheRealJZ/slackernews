using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFetcher.ApiResponseObjects
{
    class IbmWatsonClassifierClassifyResponse
    {
        [DeserializeAs(Name = "classifier_id")]
        public string ClassifierId { get; set; }

        [DeserializeAs(Name = "url")]
        public string ClassifierUrl { get; set; }

        [DeserializeAs(Name = "text")]
        public string SubmittedText { get; set; }

        [DeserializeAs(Name = "top_class")]
        public string TopClass { get; set; }

        [DeserializeAs(Name = "classes")]
        public List<IbmWatsonClassifierClass> Classes { get; set; }
    }
}
