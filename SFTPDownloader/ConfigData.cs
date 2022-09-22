using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static SFTPDownloader.Model.ConfigFileModel;
using Microsoft.Extensions.Logging;

namespace SFTPDownloader
{
    internal class ConfigData
    {
        private readonly string configPath = @"C:\Users\*\*\sftpConfig.json"; // needs to be edited considering where is sftpConfig.json
        public Config ReadConfigFile()
        {
            Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath, Encoding.UTF8));  // reading sftpConfig.json file data and storing it in model
            return config;  
        }
        
    }
}
