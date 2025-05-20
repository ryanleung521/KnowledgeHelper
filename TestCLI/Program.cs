using ClassLibrary.DB_Interaction;
using ClassLibrary.KnowledgeEntries;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.ComponentModel.Design;
using System.Data;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace TestCLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            KnowledgeTreeHelper.init();
            CLI_DB db = new CLI_DB();
            db.Init();

            DB_Operation.AddNewEntry(new EmptyEntry);

            while (true)
            {
                Console.WriteLine("Enter a command (cr, rm, md, mv) or 'exit' to quit:");
                string command = Console.ReadLine();

                if (command == "exit") break;

                switch (command)
                {
                    case "cr":
                        CreateNewNode();
                        break;
                    case "rm":
                        RemoveNode();
                        break;
                    case "md":
                        ModifyNode();
                        break;
                    case "mv":
                        MoveNode();
                        break;
                    default:
                        Console.WriteLine("Invalid command. Please try again.");
                        break;
                }
            }
            Console.WriteLine(KnowledgeTreeHelper.GetAllNodeText(KnowledgeTreeHelper.root_node));
        }
        
        static List<string> nav_keys = new List<string>() {"cr", "sl", "rm", "md", "mv"};
        static void CreateNewNode()
        {
             int id;
             string title;
             string content_text;
             KnowledgeEntry parent_node;
             List<string> tags = new List<string>();

            Console.WriteLine("Current Node: ");
            Console.WriteLine("id");
            Int32.TryParse(Console.ReadLine(), out id);
            Console.WriteLine("title");
            title = Console.ReadLine();
            Console.WriteLine("content_text");
            content_text = Console.ReadLine();
            Console.WriteLine("\nEnter sl to select parent node of the new node");
            Navigate_Tree(out parent_node);

            var node = new
            {
                id = id,
                title = title,
                content_text = content_text,
                parent_node = parent_node
            };

            string json = JsonConvert.SerializeObject(node);
            KnowledgeTreeHelper.AddNewNodeFromCLI(json, parent_node);
        }
        static void RemoveNode()
        {
            KnowledgeEntry targetentry;
            Navigate_Tree(out targetentry);
            targetentry.parent_node.children_nodes.Remove(targetentry);
        }
        static void ModifyNode()
        {
            KnowledgeEntry targetentry;
            Navigate_Tree(out targetentry);
            string selection = Console.ReadLine();
            Console.WriteLine("tt for title, ct for content");
            string new_content = Console.ReadLine();
            if (selection == "tt")
            {
                targetentry.title = new_content;
                return; 
            }
            if (selection == "ct")
            {
                targetentry.content_text = new_content;
                return;
            }
            Console.WriteLine("Nope, invalid");
        }
        static void MoveNode()
        {
            KnowledgeEntry targetentry;
            Navigate_Tree(out targetentry);
            KnowledgeEntry new_parent;
            Navigate_Tree(out new_parent);
            targetentry.parent_node.children_nodes.Remove(targetentry);
            targetentry.parent_node = new_parent;
            targetentry.parent_node.children_nodes.Add(new_parent);
        }

        //UI tools for development
        static void Navigate_Tree()
        {
            //Show Root Info
            //Show Child Title, ID, use id to select
            //-1 to exit
            //-2 back to current parent

            KnowledgeEntry CurrentNode = KnowledgeTreeHelper.root_node;
            int selection = 0;
            int depth = 0;
            const string tab = "    ";
            string current_identation = string.Concat(Enumerable.Repeat(tab, depth));
            do
            {
                if (selection == -2)
                {
                    depth--;
                    CurrentNode = CurrentNode.parent_node;
                }
                else
                {
                    depth++;
                    CurrentNode = KnowledgeTreeHelper.EntryList[selection];
                }

                string CurrentNodeInfo = KnowledgeTreeHelper.GetNodeText(CurrentNode);
                Console.WriteLine(CurrentNodeInfo);
                Console.WriteLine();
                Console.WriteLine("Child Nodes: ");
                foreach (KnowledgeEntry node in CurrentNode.children_nodes)
                {
                    Console.WriteLine($"{current_identation}ID: {node.id}: {node.title}");
                }
                
                bool ParseSuccess = Int32.TryParse(Console.ReadLine(), out selection);
                if (!ParseSuccess)
                {
                    Console.WriteLine("Parse Failed");
                    selection = CurrentNode.id;
                }
            }while (selection != -1);
        }
        static void Navigate_Tree(out KnowledgeEntry entry)
        {
            //Show Root Info
            //Show Child Title, ID, use id to select
            //-1 to exit
            //-2 back to current parent

            entry = new EmptyEntry();
            KnowledgeEntry CurrentNode = KnowledgeTreeHelper.root_node;
            int selection = 0;
            int depth = 0;
            const string tab = "    ";
            string current_identation = string.Concat(Enumerable.Repeat(tab, depth));
            Dictionary<int, KnowledgeEntry> ChildrenNodesID = new Dictionary<int, KnowledgeEntry>();
            do
            {
                if (selection == -2)
                {
                    if (CurrentNode == KnowledgeTreeHelper.root_node)
                    {
                        Console.WriteLine("THIS IS ROOT NODE!!!");
                        break;
                    }
                    depth--;
                    CurrentNode = CurrentNode.parent_node;
                }
                else
                {
                    depth++;
                    CurrentNode = KnowledgeTreeHelper.EntryList[selection];
                }

                string CurrentNodeInfo = KnowledgeTreeHelper.GetNodeText(CurrentNode);
                Console.WriteLine("Current Node:\n" + CurrentNodeInfo);
                Console.WriteLine();
                if (CurrentNode.children_nodes.Count != 0)
                {
                    Console.WriteLine("Children Nodes: ");
                    foreach (KnowledgeEntry node in CurrentNode.children_nodes)
                    {
                        Console.WriteLine($"{current_identation}ID: {node.id}: {node.title}");
                        ChildrenNodesID.Add(node.id, node);
                    }
                }

                string input = Console.ReadLine();
                if (nav_keys.Contains(input))
                {
                    entry = CurrentNode;
                    break;
                }

                bool ParseSuccess = Int32.TryParse(input, out selection);
                if (!ParseSuccess)
                {
                    Console.WriteLine("Parse Failed");
                    selection = CurrentNode.id;
                    continue;
                }
                if (selection < -2 || selection >= KnowledgeTreeHelper.EntryList.Count)
                {
                    Console.WriteLine("out of bound");
                    selection = CurrentNode.id;
                    continue;
                }
                if (ChildrenNodesID.ContainsKey(selection) == false)
                {
                    Console.WriteLine("it is not a child node of the current node");
                    continue;
                }
            } while (selection != -1);
        }
    }
 }