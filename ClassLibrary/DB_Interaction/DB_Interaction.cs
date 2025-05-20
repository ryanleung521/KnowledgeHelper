using ClassLibrary.KnowledgeEntries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DB_Interaction
{
    public class KnowledgeDbContext : DbContext
    {
        public DbSet<KnowledgeEntry> Entries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(DB_Connection.GetConnectionString());
        }

        public KnowledgeDbContext(DbContextOptions<KnowledgeDbContext> options) : base(options) { }
    }
    public static class DB_Connection
    {
        public static string GetConnectionString()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddXmlFile("ClassLibrary.dll.config", optional: false, reloadOnChange: true);

            var configuration = builder.Build();
            string connectionString = configuration["connectionStrings:add:DefaultConnection:connectionString"];

            return connectionString;
        }
    }
    public static class DB_Operation
    {
        private static DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder<KnowledgeDbContext>();
        private static DbContext dbContext;
        private static void Setup()
        {
            optionsBuilder.UseSqlServer(DB_Connection.GetConnectionString());
            dbContext = new KnowledgeDbContext(optionsBuilder.Options);
        }
        public static void AddNewEntry(KnowledgeEntry entry)
        {
            var optionsBuilder = new DbContextOptionsBuilder<KnowledgeDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            KnowledgeDbContext db = new KnowledgeDbContext(opti);
        }
    }
}