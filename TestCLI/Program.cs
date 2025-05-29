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

            while (true)
            {
                Console.WriteLine("Enter a command (nv, cr, rm, md, mv) or 'exit' to quit:");
                string command = Console.ReadLine();
                Console.WriteLine();

                if (command == "exit") break;

                switch (command)
                {
                    case "nv":
                        Navigate_Tree(out _);
                        break;
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
        }
        
        static void CreateNewNode()
        {
            //Create a new node
            //How: 
            //1. Set node info (id, title, content_text, parent_node, children_nodes)
            //2.  Set current prog info
            //3. Set db info

            //content, title -> user input (added to prog, pushedd to db)
            //id -> auto incremented by db, need to pushed in prog
            //parent -> user input, set to prog, pushed to db
            //children: N/A

            //User input title and content, select parent node
            //Add child node to parent node
            //Push to DB
            //Add ID to current program

            //Declare Variables
            string title;
             string content_text;
             KnowledgeEntry parent_node;

            //Fill Variables
            Console.WriteLine("New Node: \n");
            Console.Write("Title: ");
            title = Console.ReadLine();
            Console.Write("Content Text: ");
            content_text = Console.ReadLine();
            Console.WriteLine("\nEnter sl to select parent node of the new node\n");
            Navigate_Tree(out parent_node);

            if (title == "-1" || title == "-1" || parent_node is EmptyEntry)
            {
                return;
            }

            //Backend operations
            KnowledgeTreeHelper.CreateEntry(title, content_text, parent_node);
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
        static void Navigate_Tree(out KnowledgeEntry CurrentNode)
        {
            //Show Root Info
            //Show Child Title, ID, use id to select
            //-1 to exit
            //-2 back to current parent

            //Declare Variables

            //Node selection
            CurrentNode = KnowledgeTreeHelper.root_node;
            int selection = 0;

            //Indentation function
            int depth = 0;
            const string tab = "    ";

            do
            {
                //return to the previous node
                if (selection == -2)
                {
                    if (KnowledgeTreeHelper.IsRootNode(CurrentNode))
                    {
                        Console.WriteLine("THIS IS ROOT NODE!!!");
                        break;
                    }
                    depth--;
                    CurrentNode = CurrentNode.parent_node;
                }
                else
                {
                    //go to a deeper layer of nodes
                    depth++;
                    CurrentNode = KnowledgeTreeHelper.EntryList[selection];
                }

                //Display current node information
                string CurrentNodeInfo = KnowledgeTreeHelper.GetNodeText(CurrentNode);
                Console.WriteLine("Current Node:\n" + CurrentNodeInfo);
                Console.WriteLine();
                if (CurrentNode.children_nodes.Count != 0)
                {
                    Console.WriteLine("Children Nodes: ");
                    foreach (KnowledgeEntry node in CurrentNode.children_nodes)
                    {
                        Console.WriteLine($"{GetIndentation(tab, depth)}ID {node.id}: {node.title}");
                    }
                }

                //Input id for new node
                string input = Console.ReadLine();
                Console.WriteLine();

                //select
                if (input == "sl")
                {
                    //already assigned current node as out value 
                    return; 
                }

                //Parse and validate the selection
                bool ParseSuccess = Int32.TryParse(input, out selection);
                if (!ParseSuccess)
                {
                    Console.WriteLine("Parse Failed\n");
                    selection = CurrentNode.id;
                    continue;
                }
                if (selection == -1)
                {
                    CurrentNode = new EmptyEntry();
                    return; 
                }
                if (selection == -2)
                {
                    continue; 
                }

                if (selection < 0 || selection >= KnowledgeTreeHelper.EntryList.Count)
                {
                    Console.WriteLine("out of bound\n");
                    selection = CurrentNode.id;
                    continue;
                }
                if (CurrentNode.children_nodes.Contains(KnowledgeTreeHelper.EntryList[selection]) == false)
                {
                    Console.WriteLine("it is not a child node of the current node\n");
                    continue;
                }
            } while (true); //Enter -1 to return directly
        }

        //Identation
        private static string GetIndentation(string tab, int depth)
        {
            return string.Concat(Enumerable.Repeat(tab, depth));
        }
    }
 }