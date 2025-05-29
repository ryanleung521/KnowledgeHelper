using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ClassLibrary.DB_Interaction;
using Newtonsoft.Json;

namespace ClassLibrary.KnowledgeEntries
{
    public static class KnowledgeTreeHelper
    {
        //This static class helps to create and maintain a knowledge tree

        //Properties
        public static KnowledgeEntry root_node;
        public static List<KnowledgeEntry> EntryList = new List<KnowledgeEntry>();

        //Init
        public static void init()
        {
            DB_Operation.BuildTree();
            root_node = EntryList[0];
        }
        
        //Basic Functions
        public static string GetNodeText(KnowledgeEntry node)
        {
            string parent_node = "";
            string children_node_id = "";

            //set parent node text when the node is root
            if (IsRootNode(node))
            {
                parent_node = "This is the Root Node, there is the parent node";
            }
            else
            {
                parent_node = node.parent_node.id.ToString();
            }

            //get all children id 
            foreach (var child in node.children_nodes)
            {
                children_node_id += $"{child.id}, ";
            }

            //Remove the last ", " in string
            if (children_node_id.Length == 0)
            {
                children_node_id = "null";
            }
            else
            {
                children_node_id = children_node_id.Substring(0, children_node_id.Length - 2);
            }

            //Generate Text
            string text =
                $"ID: {node.id}\n" +
                $"Title: {node.title}\n" +
                $"Content: {node.content_text}\n" +
                $"Parent ID: {parent_node}\n" +
                $"Children ID: {children_node_id}\n";

            return text;
        }
        public static string GetAllNodeText(KnowledgeEntry root)
        {
            string nodetext = GetNodeText(root) + "\n\n";
            foreach (var node in root.children_nodes)
            {
                nodetext += GetAllNodeText(node);
            }
            return nodetext;
        }

        public static bool IsRootNode(KnowledgeEntry entry)
        {
            return entry == root_node;
        }

        //Create/Move/Delete/Modify Nodes
        public static void CreateEntry(string title, string content, KnowledgeEntry parent_node)
        {
            KnowledgeEntry new_entry = new KnowledgeEntry
            {
                title = title,
                content_text = content,
                parent_node = parent_node
            };

            EntryList.Add(new_entry);
            parent_node.children_nodes.Add(new_entry);
            DB_Operation.AddNewEntry(new_entry);
            new_entry.id = EntryList.Count-1; 
        }
    }
}