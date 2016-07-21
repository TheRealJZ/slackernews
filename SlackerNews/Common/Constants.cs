using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Constants
    {
        public const int ScoreThresholdForTaggingApi = 50;
        public const int ScoreThresholdForSemanticSummaryApi = 50;
        public const int ScoreThresholdForClassificationApi = 50;

        public enum Section : int
        {
            General = 1,
            ProductAnnouncements = 2,
            Business = 3,
            ShowHN = 4,
            AskHN = 5,
            Persuasion = 6,
            Science = 7,
            WorldNews = 8,
            Outages = 9
        }
    }
}
