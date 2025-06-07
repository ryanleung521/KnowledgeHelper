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
        public static Tag GetTag(string TagName)
        {
            return TagList.Find(t => t.TagName == TagName);
        }

        public static void AddTagToEntry(KnowledgeEntry Entry, Tag Tag)
        {
            if (Entry.tags.Contains(Tag))
            {
                return; 
            }

            Entry.tags.Add(Tag);

            DB_Operation.AddTagToEntry(Entry, Tag);
        }
        public static void RemoveTagFromEntry(KnowledgeEntry Entry, Tag Tag)
        {
            DB_Operation.RemoveTagFromEntry(Entry, Tag);
            Entry.tags.Remove(Tag);
        }
        public static void AddNewTag(string tagName)
        {
            Tag new_tag = new Tag();
            new_tag.TagName = tagName;

            TagList.Add(new_tag);
            DB_Operation.AddNewTag(new_tag);
        }
        public static void RemoveTag(string tagName)
        {
            Tag tag = null;
            foreach (var item in TagList)
            {
                if (item.TagName == tagName)
                {
                    tag = item; break;
                }
            }
            if (tag == null)
            {
                return;
            }

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