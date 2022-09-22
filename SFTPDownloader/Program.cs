using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SFTPDownloader.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static SFTPDownloader.Model.ConfigFileModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace SFTPDownloader
{
    class Program
    {

        private static System.Timers.Timer aTimer;
        static void Main(string[] args)
        {
            ILogger log = logs();
            try
            {
                
                SetTimer(); // Setting timer for program
                log.LogInformation("\nPress the Enter key to exit the application...\n");
                log.LogInformation("The application started at {0:HH:mm:ss.fff}" + DateTime.UtcNow + " UTC");
                log.LogInformation("Program will start working after 1 minute");
                Console.ReadLine();
                aTimer.Stop();
                aTimer.Dispose();
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
                Console.ReadLine();
            }
        }
        private static void SetTimer()
        {
            // Creating timer for 1 minute.
            aTimer = new System.Timers.Timer(60000);

            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;

            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        private static void OnTimedEvent(Object source, ElapsedEventArgs e) // event that will be executed every 1 minute.
        {
            ILogger log = logs();
            
            ConfigData config = new ConfigData();
            CheckAndDownloadFile download = new CheckAndDownloadFile(config);
            download.Download(log);
            log.LogInformation("Files in sftp checked at: " + DateTime.UtcNow + " UTC");

        }

        public static ILogger logs() // creating logger
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("NonHostConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });
            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("Info Log");
            logger.LogWarning("Warning Log");
            logger.LogError("Error Log");
            logger.LogCritical("Critical Log");
            return logger;
        }

    }



}