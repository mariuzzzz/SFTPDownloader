using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SFTPDownloader.Model.ConfigFileModel;

namespace SFTPDownloader.DB
{
    internal class Connection
    {
        public string ConString()
        {
            ConfigData data = new ConfigData();
            Config config = new Config();
            config = data.ReadConfigFile(); // reading config file to get connection string
            string con = config.DBConnectionString; // needs to be edited in sftpConfig.json
            return con; // returning connection string
        }
    }
}
