using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Configuration;

namespace PogFishInfrastructure
{
    public class PogFishContext : DbContext
    {
        
        public DbSet<Server> Servers { get; set; }
        private readonly IConfiguration _config;

        public PogFishContext(IConfiguration config)
        {
            _config = config;
        }

       

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        {
            string ConnectionString = $"server={_config["mysql:server"]};database={_config["mysql:database"]};user={_config["mysql:username"]};password={_config["mysql:password"]};Connect Timeout=5;";
            optionsBuilder.UseMySql(ConnectionString,ServerVersion.AutoDetect(ConnectionString));
        }

        
                
    }

    public class Server
    {
        public ulong Id { get; set; }
        public string Prefix { get; set; }
    }
}
