//using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SFTPDownloader.Model.ConfigFileModel;
using WinSCP;
using SFTPDownloader.DB;
using SFTPDownloader.Model;
using EnumerationOptions = WinSCP.EnumerationOptions;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace SFTPDownloader
{
    internal class CheckAndDownloadFile
    {
        private ConfigData _config;
        public CheckAndDownloadFile(ConfigData conf) // dependecy injection
        {
            _config = conf;
        }
        public void Download(ILogger log)
        {
            Config config = new Config();
            ConfigData con = new ConfigData();
            try
            {
                log.LogInformation("Getting data from config file");
                config = _config.ReadConfigFile(); // setting configuration information from sftpConfig.json
                log.LogInformation("Config data has been red successfully");
            }
            catch (Exception e)
            {
                log.LogError("Error reading config file " + e.ToString());
            }
            SftpContext sftpContext = new SftpContext();

            SessionOptions sessionOptions = new SessionOptions // setting options for WinSCP for working with sftp server
            {
                Protocol = Protocol.Sftp, // setting, that it will be sftp server
                HostName = config.Url,
                UserName = config.Login,
                Password = config.Password,
            };
            List<DataModel> data = new List<DataModel>(); // creating list with data base table structure
            try
            {
                log.LogInformation("Getting data from DB");
                data = Get(sftpContext).ToList(); // assigning data from DB to list
                log.LogInformation("Data got successfully");
            }
            catch (Exception e)
            {
                log.LogError("Error getting data from data base" + e.ToString());
            }
            
            
            using (Session session = new Session())
            {
                // Connect
                sessionOptions.GiveUpSecurityAndAcceptAnySshHostKey = true; // Giving up seccurity to not use ssh host key fingerprint. Only for testing purposes
                int fileCount = 0;
                try
                {
                    log.LogInformation("Trying to connect to sftp server");
                    session.Open(sessionOptions); // opening connection to sftp server
                    log.LogInformation("Connected");
                    
                    RemoteDirectoryInfo directoryInfo = session.ListDirectory(config.RemoteFilePath); // Getting list of files in the directory

                    IEnumerable<RemoteFileInfo> files =
                        directoryInfo.Files
                            .Where(file => !file.IsDirectory); // selecting files that are not directories
                    log.LogInformation("Checking files in sftp server");
                    foreach (var file in files) // going through files in server one by one
                    {
                        if (!data.Any(d => (sessionOptions.HostName.ToString() + (file.FullName.ToString()) == d.FilePath) && file.LastWriteTime.ToString() == d.TimeModified)) // checking if file path and modif time are not already in data base
                        {
                            session.GetFileToDirectory(file.FullName, config.LocalPathToDownload); // if information about file is not in DB, it is downloded to local directory that is set in sftpConfig.json file
                            sftpContext.Data.Add(new DataModel // adding data to DB
                            {
                                FilePath = config.Url.ToString() + file.FullName.ToString(), // adding full path of file
                                TimeModified = file.LastWriteTime.ToString() // adding modif time
                            });
                            sftpContext.SaveChanges(); // saving data in DB
                            fileCount += 1;// counting downloaded files
                        }
                    }
                    if (fileCount > 0)
                        log.LogInformation("Files downloaded: " + fileCount.ToString());
                    else
                        log.LogInformation("No new files at " + DateTime.UtcNow.ToString() + " UTC");
                }
                catch (Exception e)
                {
                    log.LogError("Error working with sftp: " + e.ToString());
                }
                

                

            }
        }
        public IEnumerable<DataModel> Get(SftpContext sftpContext)
        {
            return sftpContext.Data; // getting data from DB
        }
    }
}
