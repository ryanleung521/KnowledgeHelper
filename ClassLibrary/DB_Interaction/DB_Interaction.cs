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
        public DbSet<db_Entry> Entries { get; set; }
        public DbSet<db_Relationship> Relationships { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(DB_Connection.GetConnectionString());
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //For db_Entry
            modelBuilder.Entity<db_Entry>().
                HasKey(e => e.EID);

            //For db_Relationship
            modelBuilder.Entity<db_Relationship>().
                HasKey(r => new { r.PID, r.CID});
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
        private static KnowledgeDbContext db;

        private static string connectionString = DB_Connection.GetConnectionString();

        private static void Setup()
        {
            DbContextOptionsBuilder<KnowledgeDbContext> optionsBuilder = new DbContextOptionsBuilder<KnowledgeDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            db = new KnowledgeDbContext(optionsBuilder.Options);
        }

        private static void ToDBForm(KnowledgeEntry entry, out db_Entry db_entry, out db_Relationship db_relation)
        {
            db_entry = new db_Entry()
            {
                EID = entry.id,
                Title = entry.title,
                Content = entry.content_text
            };
            if (entry is not RootEntry)
            {
                //have parent node
                db_relation = new db_Relationship()
                {
                    PID = entry.parent_node.id,
                    CID = entry.id
                };
            }
            else
            {
                //no parent node
                db_relation = null;
            }
        }

        public static void AddNewEntry(KnowledgeEntry entry)
        {
            //Root Node should not be added to the database
            if (entry is RootEntry) return;

            Setup(); 

            db_Entry db_entry;
            db_Relationship db_relation;
            ToDBForm(entry, out db_entry, out db_relation);

            db.Add(db_entry);
            db.Add(db_relation);

            db.SaveChanges();
        }

        //build tree method
        public static void BuildTree()
        {
            Setup();
            Console.WriteLine(db.Entries.Count());
        }
        private static KnowledgeEntry GetEntry(int id)
        {
            Setup();
            db_Entry db_entry = db.Find<db_Entry>(id);
            if (db_entry == null) return null;

            KnowledgeEntry entry = new KnowledgeEntry()
            {
                id = db_entry.EID,
                title = db_entry.Title,
                content_text = db_entry.Content,
                parent_node = null,
                children_nodes = new List<KnowledgeEntry>(),
                tags = new List<string>()
            };

            return entry;
        }
    }
}