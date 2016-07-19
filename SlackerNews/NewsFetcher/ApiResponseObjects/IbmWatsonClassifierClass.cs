using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFetcher.ApiResponseObjects
{
    class IbmWatsonClassifierClass
    {
        [DeserializeAs(Name = "class_name")]
        public string ClassName { get; set; }

        [DeserializeAs(Name = "confidence")]
        public decimal Confidence { get; set; }
    }
}
