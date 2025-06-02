using ClassLibrary.DB_Interaction;
using ClassLibrary.KnowledgeEntries;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.ComponentModel.Design;
using System.Data;
using System.Net.Http.Headers;
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
            KnowledgeTreeHelper.RemoveEntry(targetentry);
        }
        static void ModifyNode()
        {
            KnowledgeEntry targetentry;
            Navigate_Tree(out targetentry);

            Console.WriteLine("\nSelected Node: \n\n" + KnowledgeTreeHelper.GetNodeText(targetentry));

            Console.WriteLine("Enter nothing to keep the original information: ");
            Console.Write("Title: ");
            string new_title = Console.ReadLine().Trim();
            Console.WriteLine("Content: ");
            string new_content = Console.ReadLine().Trim();

            if (new_title == string.Empty)
            {
                new_title = targetentry.title;
            }
            if (new_content == string.Empty)
            {
                new_content = targetentry.content_text;
            }

            KnowledgeTreeHelper.ModifyEntry(targetentry, new_title, new_content);
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
            CurrentNode = KnowledgeTreeHelper.root_node;

            while (true)
            {
                //Display Current Details
                DisplayCurrentNodeInfo(CurrentNode);
                DisplayCurrentNodeChildren(CurrentNode);

                //Selection
                Console.WriteLine("Input sl to Select Current Node or Enter a number to access respective node");
                Console.WriteLine("Enter -1 to leave the program. Enter -2 to Return to Parent Node. ");
                string user_input = Console.ReadLine();

                //Select Current Node
                if (user_input == "sl")
                {
                    return;
                }

                //Exit (-1)
                if (user_input == "-1")
                {
                    CurrentNode = new EmptyEntry();
                    return;
                }

                //Return to Parent Node (-2)
                if (user_input == "-2")
                {
                    if (KnowledgeTreeHelper.IsRootNode(CurrentNode) == true)
                    {
                        return;
                    }

                    CurrentNode = CurrentNode.parent_node;
                    continue;
                }

                //Select Children Node
                KnowledgeEntry selected_entry;
                try
                {
                    int selection = Convert.ToInt32(user_input);
                    selected_entry = CurrentNode.children_nodes[selection - 1];
                    CurrentNode = selected_entry;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Invalid Input");
                    continue;
                }
            }
        }
        static void DisplayCurrentNodeInfo(KnowledgeEntry CurrentNode)
        {
            Console.WriteLine($"Current Node:\n");
            Console.WriteLine(KnowledgeTreeHelper.GetNodeText(CurrentNode));
        }
        static void DisplayCurrentNodeChildren(KnowledgeEntry CurrentNode)
        {
            Console.WriteLine("Children Nodes: ");
            for (int i = 0; i < CurrentNode.children_nodes.Count; i++)
            {
                Console.WriteLine($"{i+1}. {CurrentNode.children_nodes[i].title}");
            }
            Console.WriteLine();
        }

        //Identation
        private static string GetIndentation(string tab, int depth)
        {
            return string.Concat(Enumerable.Repeat(tab, depth));
        }
    }
 }