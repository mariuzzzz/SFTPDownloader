using Microsoft.EntityFrameworkCore;
using SFTPDownloader.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SFTPDownloader.Model.ConfigFileModel;

namespace SFTPDownloader.DB
{
    public class SftpContext : DbContext 
    {

        public SftpContext()
        {

        }
        Connection con = new Connection();

        public DbSet<DataModel> Data { get; set; } // creating data table
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(con.ConString()); //postgresql configuration using entity framework
    }
}
//// data table in data base can be created using migrations
//// Tools -> NuGet Package Manager -> Package Manager Console
//// Add-Migration migration name -> Enter // it creates DB
//// Update-Database // it saves tables and DB in DB server
