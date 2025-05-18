using ClassLibrary.KnowledgeEntries;
using Newtonsoft.Json;
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
    }
 }