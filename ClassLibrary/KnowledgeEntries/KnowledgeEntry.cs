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

        public int id;
        public string title = "";
        public string content_text = "";
        public KnowledgeEntry parent_node;
        public List<KnowledgeEntry> children_nodes = new List<KnowledgeEntry>();
        public List<string> tags = new List<string>();

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
