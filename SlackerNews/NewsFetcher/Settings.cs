using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFetcher
{
    public class Settings
    {
        public enum AppSettingKeys
        {
            HackernewsApiUrl,
            AlchemyApiUrl,
            AlchemyApiKey,
            SemantriaApiUrl,
            SemantriaApiKey,
            SemantriaApiSecret,
            SemantriaApiConfiguration
        }

        public static string Get(Settings.AppSettingKeys key)
        {
            string keyName = Enum.GetName(typeof(Settings.AppSettingKeys), key);
            string keyValue = ConfigurationManager.AppSettings[keyName];
            return keyValue;
        }
    }
}
