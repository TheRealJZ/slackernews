using Common;
using NewsFetcher.Apis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFetcher
{
    class SectionClassifier
    {
        const decimal ThresholdForValidClassification = .6m;
        Dictionary<string, Constants.Section> Mappings;

        public SectionClassifier()
        {
            Mappings = new Dictionary<string, Constants.Section>()
            {
                { "surveillance", Constants.Section.General },
                { "databases", Constants.Section.General },
                { "hiring", Constants.Section.General },
                { "scalability", Constants.Section.General },
                { "releases", Constants.Section.ProductAnnouncements },
                { "business", Constants.Section.Business },
                { "legal", Constants.Section.Business },
                { "show-hn", Constants.Section.ShowHN },
                { "ask-hn", Constants.Section.AskHN },
                { "persuasion", Constants.Section.Persuasion },
                { "science", Constants.Section.Science },
                { "tech-advancements", Constants.Section.Science },
                { "world-news", Constants.Section.WorldNews },
                { "outages", Constants.Section.Outages },
                { "security-exploits", Constants.Section.Outages }
            };
        }

        public Constants.Section GetSectionFromText(string text)
        {
            NLog.LogManager.GetCurrentClassLogger().Trace($"Classifying SlackerNewsSection from text: {text}");

            var watsonApi = new IbmWatsonClassifierApi();
            var response = watsonApi.Classify(text);
            if (response.Classes != null && response.Classes.Any())
            {
                foreach (var cls in response.Classes)
                {
                    if (cls.Confidence > ThresholdForValidClassification)
                    {
                        // Break on the first one we find, since they should be ordered by confidence descending
                        return Mappings[cls.ClassName];
                    }
                }
            }

            return Constants.Section.General;
        }
    }
}
