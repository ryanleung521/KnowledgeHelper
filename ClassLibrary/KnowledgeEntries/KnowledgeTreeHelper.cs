using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ClassLibrary.KnowledgeEntries
{
    public static class KnowledgeTreeHelper
    {
        //This static class helps to create and maintain a knowledge tree

        public static KnowledgeEntry root_node = new KnowledgeEntry();

        public static void AddNewNodeFromCLI (string json_text, KnowledgeEntry parent_node)
        {
            KnowledgeEntry new_node = CreateNewNodeBasedOnJson (json_text);
            AddNewNodeToTree(new_node, parent_node);
        }
        private static KnowledgeEntry CreateNewNodeBasedOnJson (string json_text)
        {
            return JsonConvert.DeserializeObject<KnowledgeEntry>(json_text);
        }
        private static void AddNewNodeToTree(KnowledgeEntry new_node, KnowledgeEntry parent_node)
        {
            new_node.parent_node = parent_node;
            parent_node.children_nodes.Add(new_node);
        }

        public static string GetAllNodeJson(KnowledgeEntry root)
        {
            string json = JsonConvert.SerializeObject(root) + "\n\n";
            foreach (var node in root.children_nodes)
            {
                json += GetAllNodeJson(node);
            }
            return json;
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

        public static string GetNodeText(KnowledgeEntry node)
        {
            //get all children id 
            string children_node_id= "";  
            foreach (var child in node.children_nodes)
            {
                children_node_id += $"{child.id}, ";
            }

            if (children_node_id.Length == 0)
            {
                children_node_id = "null";
            }
            else
            {
                children_node_id.Substring(0, children_node_id.Length - 2);
            }

            if (node == root_node)
            {
                string text1 =
                $"ID: {node.id}\n" +
                $"Title: Root\n" +
                $"Content: {node.content_text}\n" +
                $"Parent ID: null\n" +
                $"Children ID: {children_node_id}\n";

                return text1;
            }

            string text2 =
                $"ID: {node.id}\n" +
                $"Title: {node.title}\n" +
                $"Content: {node.content_text}\n" +
                $"Parent ID: {node.parent_node.id}\n" +
                $"Children ID: {children_node_id}\n";

            return text2;
        }
    }
}
