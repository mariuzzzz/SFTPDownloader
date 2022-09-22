using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFTPDownloader.Model
{
    internal class ConfigFileModel // setting sftpConfig.json file structure
    {
        public class Config
        {
            public string Url { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
            public string LocalPathToDownload { get; set; }
            public string DBConnectionString { get; set; }
            public string RemoteFilePath { get; set; }
        }
    }
}
