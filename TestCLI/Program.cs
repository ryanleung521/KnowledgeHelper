using ClassLibrary.KnowledgeEntries;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using System.Data;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace TestCLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CreateNewNode();
            Console.WriteLine(KnowledgeTreeHelper.GetAllNodeText(KnowledgeTreeHelper.root_node));
        }

        static void CreateNewNode()
        {
             int id;
             string title;
             string content_text;
             KnowledgeEntry parent_node;
             List<string> tags = new List<string>();

            Console.WriteLine("id");
            Int32.TryParse(Console.ReadLine(), out id);
            Console.WriteLine("title");
            title = Console.ReadLine();
            Console.WriteLine("content_text");
            content_text = Console.ReadLine();

            var node = new
            {
                id = id,
                title = title,
                content_text = content_text
            };

            string json = JsonConvert.SerializeObject(node);

            KnowledgeTreeHelper.AddNewNodeFromCLI(json, KnowledgeTreeHelper.root_node);
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
    }
 }