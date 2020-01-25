using System;
using System.Collections.Generic;

namespace UniqueFilesExtractor
{
    class Config
    {
        const int EQUAL_SIGN_LENGTH = 1;

        Dictionary<string, string> configDictionary;

        public Config()
        {
            string rawConfig = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\config.txt");
            configDictionary = RawConfigToDictionary(rawConfig);
        }

        public string FindConfigValue(string configKey)
        {
            if (configDictionary.ContainsKey(configKey))
            {
                return configDictionary[configKey];
            }
            else
            {
                return "";
            }
        }

        Dictionary<string, string> RawConfigToDictionary(string rawConfig)
        {
            string[] configEntries = rawConfig.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            Dictionary<string, string> configDictionary = new Dictionary<string, string>(configEntries.Length);

            foreach(string configEntry in configEntries)
            {
                string key = configEntry.Substring(0, configEntry.IndexOf('='));
                key = key.Replace(" ", "");

                string value = configEntry.Substring(configEntry.IndexOf('=') + EQUAL_SIGN_LENGTH);
                if (value.Contains("\""))
                {
                    value = value.Replace("\"", "");
                }
                else
                {
                    value = value.Replace(" ", "");
                }

                configDictionary.Add(key, value);
            }

            return configDictionary;
        }
    }
}
