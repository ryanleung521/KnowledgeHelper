using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.KnowledgeEntries
{
    public class RootEntry:KnowledgeEntry
    {
        public RootEntry()
        {
            id = 0;
            title = "Root";
            content_text = "Root Entry";
            parent_node = new EmptyEntry();
            children_nodes = new List<KnowledgeEntry>();
            tags = new List<Tag>();
        }
    }

    public class EmptyEntry : KnowledgeEntry
    {
        public EmptyEntry()
        {
            id = -1;
            title = "";
            content_text = "";
            parent_node = null;
            children_nodes = new List<KnowledgeEntry>();
            tags = new List<Tag>();
        }
    }
}