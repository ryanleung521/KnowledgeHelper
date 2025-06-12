using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClassLibrary;
using ClassLibrary.KnowledgeEntries;
using Microsoft.Identity.Client;

namespace DesktopKnowledgeHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Testing { get; set; } = "abc";
        public ObservableCollection<DisplayEntry> RootNodes { get; set; } = new ObservableCollection<DisplayEntry>();

        public MainWindow()
        {
            InitializeComponent();

            KnowledgeTreeHelper.init();
            TagHelper.Init();

            RootNodes.Add(new DisplayEntry(KnowledgeTreeHelper.root_node));

            this.DataContext = this;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                TreeViewItem CurrentNode = sender as TreeViewItem;
                
                if (CurrentNode != null)
                {
                    CurrentNode.IsExpanded = !CurrentNode.IsExpanded; //Not Gates
                }
            }
            e.Handled = true;
        }
    }

    public class DisplayEntry
    {
        public KnowledgeEntry Entry { get; set; } = new KnowledgeEntry();
        public ObservableCollection<DisplayEntry> Children { get; set;  } = new ObservableCollection<DisplayEntry>();

        public DisplayEntry() { }
        public DisplayEntry(KnowledgeEntry entry)
        {
            this.Entry = entry;
            foreach (var child in entry.children_nodes)
            {
                Children.Add(new DisplayEntry(child));
            }
        }
    }
}