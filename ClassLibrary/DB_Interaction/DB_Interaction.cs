﻿using ClassLibrary.KnowledgeEntries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DB_Interaction
{
    public class KnowledgeDbContext : DbContext
    {
        public DbSet<db_Entry> Entries { get; set; }
        public DbSet<db_Relationship> Relationships { get; set; }
        public DbSet< db_Tag> Tags { get; set; }
        public DbSet<db_Tagging> Taggings { get; set; }

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

            //For db_Tag
            modelBuilder.Entity<db_Tag>().
                HasKey(t => t.TID);

            //For db_Tagging
            modelBuilder.Entity<db_Tagging>().
                HasKey(t => new { t.EID, t.TID });
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

        //CRUD
        public static void AddNewEntry(KnowledgeEntry entry, out int new_entry_id)
        {
            new_entry_id = -1;

            //Root Node should not be added to the database
            if (KnowledgeTreeHelper.IsRootNode(entry)) return;

            Setup(); 

            db_Entry db_entry = new db_Entry(); 
            db_entry.Title = entry.title;
            db_entry.Content = entry.content_text;
            db.Entries.Add(db_entry);
            db.SaveChanges();

            db_Relationship db_relation;
            db_relation = new db_Relationship();
            db_relation.PID = entry.parent_node.id;
            db_relation.CID = db.Entries.Max(a => a.EID);
            db.Relationships.Add(db_relation);
            db.SaveChanges();

            new_entry_id = db_relation.CID;
        }
        public static void DeleteEntry (KnowledgeEntry entry)
        {
            if (KnowledgeTreeHelper.IsRootNode(entry)) return; //Cannot delete root node

            Setup();

            //Remove Entry Relationships from table Relationships
            var db_relationships = db.Relationships.Where(a => a.PID == entry.id || a.CID == entry.id).ToArray();
            if (db_relationships.Length > 0)
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
        public static void ModifyEntry(KnowledgeEntry target_entry, KnowledgeEntry new_content)
        {
            Setup();
            var db_targetEntry = db.Entries.Single(e => e.EID == target_entry.id);
            db_targetEntry.Title = new_content.title;
            db_targetEntry.Content = new_content.content_text;
            db.SaveChanges();
        }
        public static void MoveEntry (KnowledgeEntry target_entry, KnowledgeEntry old_parent_entry, KnowledgeEntry new_parent_entry)
        {
            Setup();
            var old_relation = db.Relationships.Single(a => a.PID == old_parent_entry.id && a.CID == target_entry.id);
            if (old_relation != null)
            {
                db.Relationships.Remove(old_relation);
                db.SaveChanges();
            }

            db_Relationship new_relation = new db_Relationship();
            new_relation.PID = new_parent_entry.id;
            new_relation.CID = target_entry.id;
            db.Relationships.Add(new_relation);
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
                tags = new List<Tag>()
            };

            return entry;
        }
        private static List<db_Relationship> GetRelationships()
        {
            return db.Relationships.ToList();
        }
        
        //Tagging System
        public static void AddTagsFromDB()
        {
            Setup();
            var TagList =  db.Tags.ToList();
            foreach (var db_tag in TagList)
            {
                Tag tag = new Tag();
                tag.TID = db_tag.TID;
                tag.TagName = db_tag.TName;

                TagHelper.TagList.Add(tag);
            }

            var TaggingList = db.Taggings.ToList();
            foreach (var db_tagging in TaggingList)
            {
                var entry =  KnowledgeTreeHelper.GetEntry(db_tagging.EID);
                var tag = TagHelper.GetTag(db_tagging.TID);
                if (entry != null)
                {
                    entry.tags.Add(tag);
                }
            }
        }

        public static void AddTagToEntry(KnowledgeEntry entry, Tag tag)
        {
            Setup();

            var tagging = GenerateDBTagging(entry, tag);
            db.Taggings.Add(tagging);

            db.SaveChanges();
        }

        public static void RemoveTagFromEntry (KnowledgeEntry entry, Tag tag)
        {
            Setup(); 

            if (entry.tags.Contains(tag) == false)
            {
                return;
            }

            db_Tagging db_tagging = db.Taggings.Single(t => t.TID == tag.TID && t.EID == entry.id);
            db.Taggings.Remove(db_tagging);

            db.SaveChanges();
        }

        public static void AddNewTag(Tag tag)
        {
            Setup();
            var db_tag = GenerateDBTag(tag);
            db.Tags.Add(db_tag);
            db.SaveChanges();

            tag.TID = db.Tags.Max(t=> t.TID);
        }
        public static void RemoveTag(Tag tag)
        {
            Setup();

            var db_tagging = db.Taggings.Where(t => t.TID == tag.TID).ToList();
            db.Taggings.RemoveRange(db_tagging);
            db.SaveChanges();

            var db_tag = db.Tags.Find(tag.TID);
            db.Tags.Remove(db_tag);
            db.SaveChanges();
        }

        //Convert Objects to DB Version
        private static db_Entry GenerateDBEntry(KnowledgeEntry entry)
        {
            return new db_Entry() {  EID = entry.id, Content = entry.content_text , Title = entry.title };
        }
        private static db_Relationship GenerateDBRelation(KnowledgeEntry parent, KnowledgeEntry child)
        {
            return new db_Relationship() { PID = parent.id, CID = child.id };
        }
        private static db_Tag GenerateDBTag(Tag tag)
        {
            return new db_Tag() { TID = tag.TID, TName = tag.TagName };
        }
        private static db_Tagging GenerateDBTagging(KnowledgeEntry entry,  Tag tag)
        {
            return new db_Tagging() { EID = GenerateDBEntry(entry).EID, TID = GenerateDBTag(tag).TID };
        }

        //Misc Func
        public static void ResetIdentityIncrement(int new_start_value)
        {
            Setup();
            db.Database.ExecuteSqlRaw($"DBCC CHECKIDENT ('Entries', RESEED, {new_start_value})");
        }
    }
}