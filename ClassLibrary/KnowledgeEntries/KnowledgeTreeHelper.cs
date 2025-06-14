using ClassLibrary.DB_Interaction;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

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
                parent_node = "N/A";
            }
            else
            {
                parent_node = node.parent_node.title;
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
                $"Title: {node.title}\n" +
                $"Content: {node.content_text}\n" +
                $"Parent Entry: {parent_node}\n";

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
        public static int GetNodeDepth(KnowledgeEntry node)
        {
            if (EntryList.Contains(node) == false)
            {
                return -1;
            }

            int depth= -1;
            while (node is not EmptyEntry)
            {
                depth++;
                node = node.parent_node;
            }
            return depth;
        }
        public static int GetGreatestDepth()
        {
            int depth = 0;
            Queue<KnowledgeEntry> ParentNodesList = new Queue<KnowledgeEntry>();
            ParentNodesList.Enqueue(root_node);
            while (ParentNodesList.Count > 0)
            {
                int original_count = ParentNodesList.Count;
                //Change nodes to depth +1
                for (int i = 0; i < original_count; i++)
                {
                    var node = ParentNodesList.Dequeue();
                    foreach (var child in node.children_nodes)
                    {
                        if (child.children_nodes.Count != 0)
                        {
                            ParentNodesList.Enqueue(child);
                        }
                    }
                }
                depth ++;
            }

            return depth;
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

            int new_entry_id;
            DB_Operation.AddNewEntry(new_entry, out new_entry_id);
            new_entry.id = new_entry_id;
        }
        public static void RemoveEntry (KnowledgeEntry entry)
        {
            if (IsRootNode(entry) == true)
            {
                return; 
            }

            //Non-Root Node
            entry.parent_node.children_nodes.Remove(entry);
            EntryList.Remove(entry);
            DB_Operation.DeleteEntry(entry);
        }
        public static void ModifyEntry(KnowledgeEntry entry, string title, string content)
        {
            entry.title = title;
            entry.content_text = content;

            KnowledgeEntry new_entry = new KnowledgeEntry();
            new_entry.title = title;
            new_entry.content_text = content;

            DB_Operation.ModifyEntry(entry, new_entry);
        }
        public static void MoveEntry(KnowledgeEntry target_entry, KnowledgeEntry new_parent_entry)
        {
            if (target_entry == new_parent_entry)
            {
                Console.WriteLine("Entries cant be moved under itself. ");
                return;
            }

            var old_parent_entry = target_entry.parent_node;
            old_parent_entry.children_nodes.Remove(target_entry);
            new_parent_entry.children_nodes.Add(target_entry);
            target_entry.parent_node = new_parent_entry;

            DB_Operation.MoveEntry(target_entry, old_parent_entry, new_parent_entry);
        }

        //I/O
        public static KnowledgeEntry GetEntry (int EID)
        {
            return EntryList.Find(e => e.id == EID);
        }

        //Searching Methods
        private static KnowledgeEntry searchRoot = new KnowledgeEntry();
        public static void SearchEntries(string searchKey)
        {
            List<KnowledgeEntry> searchResults = new List<KnowledgeEntry> ();

            if (searchKey[0] == '#')
            {
                searchResults = new List<KnowledgeEntry>(SearchByTag(searchKey));
            }

            if (searchKey[0] != '#')
            {
                searchResults = new List<KnowledgeEntry>(SearchByText(searchKey));
            }

            AddSearchResultToRoot(searchKey, searchResults, out searchRoot);
        }
        public static void CompleteSearch()
        {
            RemoveSearchResultFromRoot(searchRoot);
        }

        private static List<KnowledgeEntry> SearchByText(string searchKey)
        {
            List<KnowledgeEntry> resultList = new List<KnowledgeEntry> ();
            string keyword = searchKey;

            foreach (KnowledgeEntry entry in EntryList)
            {
                if (entry is RootEntry || entry is EmptyEntry)
                {
                    continue;
                }

                if (entry.title.Contains(keyword) || entry.content_text.Contains(keyword))
                {
                    resultList.Add(entry);
                }
            }

            return resultList;
        }
        private static List<KnowledgeEntry> SearchByTag(string searchKey)
        {
            List<KnowledgeEntry> resultList = new List<KnowledgeEntry>();
            Tag keyTag = null;

            foreach (Tag tag in TagHelper.TagList)
            {
                if (tag.TagName == searchKey.Substring(1))
                {
                    keyTag = tag;
                    break;
                }
            }

            if (keyTag == null)
            {
                return resultList;
            }

            foreach (KnowledgeEntry entry in EntryList)
            {
                if (entry is RootEntry || entry is EmptyEntry)
                {
                    continue;
                }

                if (entry.tags.Contains(keyTag))
                {
                    resultList.Add(entry);
                }
            }

            return resultList;
        }

        private static void AddSearchResultToRoot(string searchKey, List<KnowledgeEntry> search_result, out KnowledgeEntry searchRoot)
        {
            searchRoot = new KnowledgeEntry();
            search_result = new List<KnowledgeEntry>(RemoveDescendentsFromResultList(search_result));

            //id
            searchRoot.title = $"Search Result of {searchKey}";
            searchRoot.content_text = $"A list of nodes satisfying the searching condition of {searchKey}";
            foreach (var result in search_result)
            {
                searchRoot.children_nodes.Add(result);
            }

            searchRoot.parent_node = root_node;
            root_node.children_nodes.Add (searchRoot);
            EntryList.Add(searchRoot);
        }
        private static void RemoveSearchResultFromRoot(KnowledgeEntry searchRoot)
        {
            root_node.children_nodes.Remove(searchRoot);
        }

        //Helper methods for searching
        private static List<KnowledgeEntry> RemoveDescendentsFromResultList (List<KnowledgeEntry> searchResult)
        {
            HashSet<KnowledgeEntry> hashEntry = new HashSet<KnowledgeEntry> ();
            List<KnowledgeEntry> newResult = new List<KnowledgeEntry> ();

            foreach (var subRoot in searchResult)
            {
                if (hashEntry.Contains(subRoot))
                {
                    continue;
                }

                newResult.Add (subRoot);
                hashEntry.Add(subRoot);
                foreach (var descendent in GetDescendents(subRoot))
                {
                    //If descendents of subRoot exist in the hashEntry => only highest subRoot should exist
                    if (hashEntry.Contains(descendent))
                    {
                        newResult.Remove(descendent);
                        continue;
                    }

                    hashEntry.Add(descendent);
                }
            }

            return newResult;
        }
        private static List<KnowledgeEntry> GetDescendents(KnowledgeEntry root)
        {
            List<KnowledgeEntry> descendents = new List<KnowledgeEntry> ();

            foreach (var child in root.children_nodes)
            {
                descendents.Add (child);
                descendents.AddRange(GetDescendents(child));
            }
            
            return descendents;
        }
    }
}