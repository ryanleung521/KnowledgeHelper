using ClassLibrary;
using ClassLibrary.KnowledgeEntries;
using Microsoft.Identity.Client;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        //Data Binding

        //Tree View
        public ObservableCollection<DisplayEntry> RootNodes { get; set; } = new ObservableCollection<DisplayEntry>();

        private string _CurrentNodeString;
        public string CurrentNodeString
        {
            get { return _CurrentNodeString; }
            set 
            { 
                if (value != _CurrentNodeString)
                {
                    _CurrentNodeString = value;
                    OnPropertyChanged(nameof(CurrentNodeString));
                } 
            }
        }

        //Init
        public MainWindow()
        {
            InitializeComponent();

            KnowledgeTreeHelper.init();
            TagHelper.Init();

            RootNodes.Add(new DisplayEntry(KnowledgeTreeHelper.root_node));

            this.DataContext = this;
        }

        //Navigate the TreeView
        private void NavigateTree(object sender)
        {
            TreeViewItem CurrentNode = sender as TreeViewItem;

            if (CurrentNode != null)
            {
                CurrentNode.IsExpanded = !CurrentNode.IsExpanded; //Not Gates
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

        //CRUD
        private void btn_create_Click(object sender, RoutedEventArgs e)
        {
            var _ParentNode = this.KnowledgeTreeDisplay.SelectedItem as DisplayEntry;
            var ParentNode = _ParentNode.Entry;

            var title = this.txt_Title.Text;
            var content = this.txt_Content.Text;

            KnowledgeTreeHelper.CreateEntry(title, content, ParentNode);

            //Update the TreeViewDisplay (Use DisplayEntry objects to display)
            KnowledgeEntry new_node = KnowledgeTreeHelper.EntryList.Last();
            _ParentNode.Children.Add(new DisplayEntry(new_node));
        }
        private void btn_remove_Click (object sender, RoutedEventArgs e)
        {
            var _SelectedNode = this.KnowledgeTreeDisplay.SelectedValue as DisplayEntry;
            var SelectedNode = _SelectedNode.Entry;

            KnowledgeTreeHelper.RemoveEntry(SelectedNode);

            _SelectedNode.Parent.Children.Remove(_SelectedNode);
        }
        private void btn_modify_Click(object sender, RoutedEventArgs e)
        {
            var _SelectedNode = this.KnowledgeTreeDisplay.SelectedItem as DisplayEntry;
            var SelectedNode = _SelectedNode.Entry;

            var new_title = this.txt_Title.Text;
            var new_content = this.txt_Content.Text;

            KnowledgeTreeHelper.ModifyEntry(SelectedNode, new_title, new_content);
        }

        //For INotifyPropertyChange
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
}