using ClassLibrary;
using ClassLibrary.KnowledgeEntries;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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

namespace DesktopKnowledgeHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Init
        public MainWindow()
        {
            InitializeComponent();

            KnowledgeTreeHelper.init();
            TagHelper.Init();

            RootNodes.Add(new DisplayEntry(KnowledgeTreeHelper.root_node));
            
            this.DataContext = this;

            this.radio_local.IsChecked = true;
        }

        //TreeView
        //Tree View DataBinding
        public ObservableCollection<DisplayEntry> RootNodes { get; set; } = new ObservableCollection<DisplayEntry>();

        private void NavigateTree(object sender)
        {
            TreeViewItem CurrentTreeViewItem = sender as TreeViewItem;
            var _entry = CurrentTreeViewItem.DataContext as DisplayEntry;
            var CurrentEntry = _entry.Entry;

            if (CurrentTreeViewItem != null)
            {
                CurrentTreeViewItem.IsExpanded = !CurrentTreeViewItem.IsExpanded; //Not Gates
                FillTagsList(CurrentEntry);
            }
        }
        private void TreeTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                NavigateTree(sender);
            }
            e.Handled = true;
        }
        private void TreeTitle_KeyDown (Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Enter)
            {
                NavigateTree(sender);
            }
            e.Handled = true;
        }

        //CRUD of Nodes
        private void btn_create_Click(object sender, RoutedEventArgs e)
        {
            CreateNode();
        }
        private void btn_remove_Click (object sender, RoutedEventArgs e)
        {
            RemoveNode();
        }
        private void btn_modify_Click(object sender, RoutedEventArgs e)
        {
            ModifyNode();
        }

        private void CreateNode()
        {
            if (this.KnowledgeTreeDisplay.SelectedItem == null)
            {
                return;
            }

            var _ParentNode = this.KnowledgeTreeDisplay.SelectedItem as DisplayEntry;
            var ParentNode = _ParentNode.Entry;

            var title = this.txt_Title.Text;
            var content = this.txt_Content.Text;

            //Not Same Information to prevent misclick
            if (title == ParentNode.title || content == ParentNode.content_text)
            {
                return;
            }

            KnowledgeTreeHelper.CreateEntry(title, content, ParentNode);

            //Update the TreeViewDisplay (Use DisplayEntry objects to display)
            KnowledgeEntry new_node = KnowledgeTreeHelper.EntryList.Last();
            _ParentNode.Children.Add(new DisplayEntry(new_node));
        }
        private void RemoveNode()
        {
            if (this.KnowledgeTreeDisplay.SelectedItem == null)
            {
                return;
            }

            var _SelectedNode = this.KnowledgeTreeDisplay.SelectedItem as DisplayEntry;
            var SelectedNode = _SelectedNode.Entry;

            KnowledgeTreeHelper.RemoveEntry(SelectedNode);

            _SelectedNode.Parent.Children.Remove(_SelectedNode);
        }
        private void ModifyNode()
        {
            if (this.KnowledgeTreeDisplay.SelectedItem == null)
            {
                return;
            }

            var _SelectedNode = this.KnowledgeTreeDisplay.SelectedItem as DisplayEntry;
            var SelectedNode = _SelectedNode.Entry;

            var new_title = this.txt_Title.Text;
            var new_content = this.txt_Content.Text;

            KnowledgeTreeHelper.ModifyEntry(SelectedNode, new_title, new_content);
        }

        //Tag List Databinding
        public ObservableCollection<DisplayTag> TagsList { get; set; } = new ObservableCollection<DisplayTag>();
        private void FillTagsList(KnowledgeEntry entry)
        {
            TagsList.Clear();

            foreach (var tag in entry.tags)
            {
                var new_tags = new DisplayTag(tag);
                TagsList.Add(new_tags);
            }
        }

        private void btn_addTag_Click(object sender, RoutedEventArgs e)
        {
            var _Entry = this.KnowledgeTreeDisplay.SelectedItem as DisplayEntry;
            var Entry = _Entry.Entry;

            var TagName = txt_TagName.Text;
            var Tag = TagHelper.TagList.Find(t => t.TagName == TagName);
            if (Tag == null)
            {
                TagHelper.AddNewTag(TagName);
                Tag = TagHelper.TagList.Last();
            }
            TagHelper.AddTagToEntry(Entry, Tag);
        }
        private void radio_local_Checked(object sender, RoutedEventArgs e)
        {
            TreeViewItem CurrentTreeViewItem = sender as TreeViewItem;
            if (CurrentTreeViewItem == null)
            {
                return;
            }

            var _entry = CurrentTreeViewItem.DataContext as DisplayEntry;
            var CurrentEntry = _entry.Entry;

            FillTagsList(CurrentEntry);
        }
        private void radio_global_Checked(object sender, RoutedEventArgs e)
        {
            TagsList.Clear();
            foreach (Tag tag in TagHelper.TagList)
            {
                TagsList.Add(new DisplayTag(tag));
            }
        }
    }

    public class DisplayEntry
    {
        public KnowledgeEntry Entry { get; set; } = new KnowledgeEntry();
        public ObservableCollection<DisplayEntry> Children { get; set;  } = new ObservableCollection<DisplayEntry>();
        public DisplayEntry Parent { get; set; }

        public DisplayEntry() { }
        public DisplayEntry(KnowledgeEntry entry)
        {
            this.Entry = entry;
            foreach (var child in entry.children_nodes)
            {
                var new_child = new DisplayEntry(child);
                new_child.Parent = this;
                Children.Add(new_child);
            }
        }
    }
    public class DisplayTag
    {
        public Tag tag { get; set; }

        public DisplayTag() { }
        public DisplayTag(Tag tag)
        {
            this.tag = tag;
        }
    }
}