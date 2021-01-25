using Microsoft.EntityFrameworkCore;
using System;

namespace PogFishInfrastructure
{
    public class PogFishContext : DbContext
    {
        private const string ConnectionString = "server=localhost;database=pog fish;user=root;Connect Timeout=5;";
        public DbSet<Server> Servers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseMySql(ConnectionString,ServerVersion.AutoDetect(ConnectionString));
    }

    public class Server
    {
        public ulong Id { get; set; }
        public string Prefix { get; set; }
    }
}
