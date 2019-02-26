using System;
using System.Collections.Generic;

namespace UniqueFilesExtractor
{
    class Config
    {
        //public string InputFolder { get { return FindConfigValue(INPUT_FOLDER); } }
        //public string OutputFolder { get { return FindConfigValue(OUTPUT_FOLDER); } }
        //public string FileFormat { get { return FindConfigValue(FILE_FORMAT); } }

        const int EQUAL_SIGN_LENGTH = 1;

        Dictionary<string, string> configDictionary;

        public Config()
        {
            string rawConfig = unique_file_extractor.Properties.Resources.config;
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
            rawConfig = rawConfig.Replace(" ", string.Empty);
            string[] configEntries = rawConfig.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            Dictionary<string, string> configDictionary = new Dictionary<string, string>(configEntries.Length);

            foreach(string configEntry in configEntries)
            {
                string key = configEntry.Substring(0, configEntry.IndexOf('='));
                string value = configEntry.Substring(configEntry.IndexOf('=') + EQUAL_SIGN_LENGTH);

                configDictionary.Add(key, value);
            }

            return configDictionary;
        }
    }
}
