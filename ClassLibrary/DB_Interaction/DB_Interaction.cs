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
                Title = entry.title,
                Content = entry.content_text
            };
            if (! KnowledgeTreeHelper.IsRootNode(entry))
            {
                //have parent node
                db_relation = new db_Relationship()
                {
                    PID = entry.parent_node.id,
                    CID = db.Entries.Max(r => r.EID) +1
                };
            }
            else
            {
                //no parent node
                db_relation = null;
            }
        }

        //CRUD
        public static void AddNewEntry(KnowledgeEntry entry)
        {
            //Root Node should not be added to the database
            if (KnowledgeTreeHelper.IsRootNode(entry)) return;

            Setup(); 

            db_Entry db_entry;
            db_Relationship db_relation;
            ToDBForm(entry, out db_entry, out db_relation);

            db.Add(db_entry);
            db.SaveChanges();
            db.Add(db_relation);
            db.SaveChanges();
        }
        public static void DeleteEntry (KnowledgeEntry entry)
        {
            if (KnowledgeTreeHelper.IsRootNode(entry)) return; //Cannot delete root node

            Setup();

            //Remove Entry Relationships from table Relationships
            var db_relationships = db.Relationships.Where(a => a.PID == entry.id || a.CID == entry.id).ToList();
            if (db_relationships.Count > 0)
            {
                db.RemoveRange(db_relationships); //Remove all relationships related to the entry
                db.SaveChanges();
            }

            //Remove Entry from table Entries
            var db_entry = db.Find<db_Entry>(entry.id);
            if (db_entry == null) return; //Entry not found
            db.Remove(db_entry); //Remove the entry from the database if found
            db.SaveChanges();
        }

        //build tree method
        public static void BuildTree()
        {
            //Place all entries in a list
            //Get all relationships and add them to each node of the tree
            //Relationship: Parent, Child
            //For every record: Parent: Child Node add parent, Child: Parent Node add child
            Setup();
            
            //Fill the list
            for (int i = 0; i < db.Entries.Max(e => e.EID)+1; i++)
            {
                KnowledgeTreeHelper.EntryList.Add(GetEntry(i));
            }

            //Get all relationships
            List<db_Relationship> relationships = GetRelationships();

            //Add relationships to the tree
            foreach (var relationship in relationships)
            {
                KnowledgeEntry parent = KnowledgeTreeHelper.EntryList.Find(e => e.id == relationship.PID);
                KnowledgeEntry child = KnowledgeTreeHelper.EntryList.Find(e => e.id == relationship.CID);
                if (parent != null && child != null)
                {
                    parent.children_nodes.Add(child);
                    child.parent_node = parent;
                }
            }
        }
        private static KnowledgeEntry GetEntry(int id)
        {
            Setup();
            db_Entry db_entry = db.Find<db_Entry>(id);
            if (db_entry == null) return new EmptyEntry();

            //Root Entry
            if (db_entry.EID == 0)
            {
                return new RootEntry();
            }

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
        private static List<db_Relationship> GetRelationships()
        {
            return db.Relationships.ToList();
        }

        public static void ResetIdentityIncrement(int new_start_value)
        {
            Setup();
            db.Database.ExecuteSqlRaw($"DBCC CHECKIDENT ('Entries', RESEED, {new_start_value})");
        }
    }
}