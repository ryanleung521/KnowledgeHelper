using ClassLibrary.DB_Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.KnowledgeEntries
{
    public class Tag
    {
        public string TagName { get; set; }
        public int TID { get; set; }

        public Tag() { }
    }

    public static class TagHelper
    {
        public static List<Tag> TagList = new List<Tag>();

        public static void Init()
        {
            DB_Operation.AddTagsFromDB();
        }

        public static Tag GetTag (int id)
        {
            return TagList.Find(t => t.TID == id);
        }

        public static void AddTagToEntry(KnowledgeEntry Entry, Tag Tag)
        {
            Entry.tags.Add(Tag);

            DB_Operation.AddTagToEntry(Entry, Tag);
        }
        public static void RemoveTagFromEntry(KnowledgeEntry Entry, Tag Tag)
        {
            Entry.tags.Remove(Tag);

            DB_Operation.RemoveTagFromEntry(Entry, Tag);
        }
        public static void AddNewTag(Tag tag)
        {
            TagList.Add(tag);
            DB_Operation.AddNewTag(tag);
        }
        public static void RemoveTag(Tag tag)
        {
            foreach (var entry in KnowledgeTreeHelper.EntryList)
            {
                if (entry.tags.Contains(tag) == true)
                {
                    entry.tags.Remove(tag);
                }
            }
            TagList.Remove(tag);
            DB_Operation.RemoveTag(tag);
        }
    }
}