using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.KnowledgeEntries
{
    public class KnowledgeEntry
    {
        //instances of this class represent a node in the knowledge tree

        public int id { get; set; }
        public string title { get; set; } = "";
        public string content_text { get; set; } = "";
        public KnowledgeEntry parent_node { get; set; }
        public List<KnowledgeEntry> children_nodes { get; set; } = new List<KnowledgeEntry>();
        public List<Tag> tags { get; set; } = new List<Tag>();

        public KnowledgeEntry() { }
        public KnowledgeEntry(int id, string title, string content_text, KnowledgeEntry parent_node, List<KnowledgeEntry> children_nodes, List<string> tags)
        {
            this.id = id;
            this.title = title;
            this.content_text = content_text;
            this.parent_node = parent_node;
            this.children_nodes = children_nodes;
            this.tags = tags;
        }
    }
}