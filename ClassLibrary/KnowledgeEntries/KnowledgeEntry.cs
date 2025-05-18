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

        public int id = 0;
        public string title;
        public string content_text;
        public KnowledgeEntry parent_node;
        public List<KnowledgeEntry> children_nodes = new List<KnowledgeEntry>();
        public List<string> tags = new List<string>();

        public KnowledgeEntry() { }
    }
}
