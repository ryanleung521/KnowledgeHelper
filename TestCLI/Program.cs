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

            KnowledgeEntry StartingNode = KnowledgeTreeHelper.root_node;
            KnowledgeEntry CurrentNode = new EmptyEntry();

            //DB_Operation.ResetIdentityIncrement(5);
            //return;

            while (true)
            {
                if (CurrentNode is EmptyEntry)
                {
                    Navigate_Tree(StartingNode, out CurrentNode);
                }
                else
                {
                    Navigate_Tree(CurrentNode, out CurrentNode);
                }
            }
        }
        
        static void CreateNewNode(KnowledgeEntry parent_node)
        {
            //Declare Variables
            string title;
             string content_text;

            //Fill Variables
            Console.WriteLine("New Node: \n");
            Console.Write("Title: ");
            title = Console.ReadLine();
            Console.Write("Content Text: ");
            content_text = Console.ReadLine();
            Console.WriteLine("\nEnter sl to select parent node of the new node\n");

            if (title == "-1" || title == "-1" || parent_node is EmptyEntry)
            {
                return;
            }

            //Backend operations
            KnowledgeTreeHelper.CreateEntry(title, content_text, parent_node);
        }
        static void RemoveNode(KnowledgeEntry targetentry)
        {
            if (targetentry is not EmptyEntry)
            {
                KnowledgeTreeHelper.RemoveEntry(targetentry);
            }
        }
        static void ModifyNode(KnowledgeEntry targetentry)
        {
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
        static void MoveNode(KnowledgeEntry targetentry)
        {
            Console.WriteLine("Select the destination entry for the entry to be moved to: ");
            KnowledgeEntry new_parent;
            Navigate_Tree(targetentry, out new_parent);
            Console.WriteLine("Destination selected");

            if (targetentry is not EmptyEntry && new_parent is not EmptyEntry)
            {
               KnowledgeTreeHelper.MoveEntry(targetentry, new_parent);
            }
        }

        //UI tools for development

        static void Navigate_Tree(KnowledgeEntry StartingNode, out KnowledgeEntry CurrentNode)
        {
            CurrentNode = StartingNode;

            while (true)
            {
                //Display Current Details
                DisplayCurrentNodeInfo(CurrentNode);
                DisplayCurrentNodeChildren(CurrentNode);

                //User Input -> Control, Selection, Command
                Console.WriteLine("Input sl to Select Current Node or Enter a number to access respective node");
                Console.WriteLine("Enter -1 to reset. Enter -2 to Return to Parent Node. ");
                Console.WriteLine("Enter a command (nv, cr, rm, md, mv) or 'exit' to quit:");
                Console.WriteLine();
                string user_input = Console.ReadLine();

                //Execute Commands
                if (ExecuteCommand (CurrentNode, user_input) == true)
                {
                    return;
                }

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
                        Console.WriteLine("This is the Root Node. ");
                        continue; 
                    }

                    CurrentNode = CurrentNode.parent_node;
                    continue;
                }

                //Select New Current Node
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

        //Return true when command executed
        static bool ExecuteCommand(KnowledgeEntry SelectedNode, string command)
        {
            if (command == "exit") Environment.Exit(0);

            switch (command)
            {
                case "cr":
                    CreateNewNode(SelectedNode);
                    return true;
                case "rm":
                    RemoveNode(SelectedNode);
                    return true;
                case "md":
                    ModifyNode(SelectedNode);
                    return true;
                case "mv":
                    MoveNode(SelectedNode);
                    return true;
                default:
                    return false;
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