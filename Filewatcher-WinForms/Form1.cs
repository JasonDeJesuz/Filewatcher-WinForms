using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Filewatcher_WinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            PopulateTreeView();
            this.treeView1.NodeMouseClick +=
new TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
        }

        private void PopulateTreeView()
        {
            TreeNode rootNode;

            DirectoryInfo info = new DirectoryInfo(@"../../../../");        // Looks at the whole desktop
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }
        }

        private void GetDirectories(DirectoryInfo[] subDirs,
            TreeNode nodeToAddTo)
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;
            foreach (DirectoryInfo subDir in subDirs)
            {
                aNode = new TreeNode(subDir.Name, 0, 0);
                aNode.Tag = subDir;
                aNode.ImageKey = "folder";
                subSubDirs = subDir.GetDirectories();
                if (subSubDirs.Length != 0)
                {
                    GetDirectories(subSubDirs, aNode);
                }
                nodeToAddTo.Nodes.Add(aNode);
            }
        }

        public string path;

        void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode newSelected = e.Node;
            listView1.Items.Clear();
            DirectoryInfo nodeDirInfo = (DirectoryInfo)newSelected.Tag;
            path = Path.GetFullPath(nodeDirInfo.ToString());
            ListViewItem.ListViewSubItem[] subItems;
            ListViewItem item = null;

            foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
            {
                item = new ListViewItem(dir.Name, 0);
                subItems = new ListViewItem.ListViewSubItem[]
                    {new ListViewItem.ListViewSubItem(item, "Directory"),
             new ListViewItem.ListViewSubItem(item,
                dir.LastAccessTime.ToShortDateString())};
                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }
            foreach (FileInfo file in nodeDirInfo.GetFiles())
            {
                item = new ListViewItem(file.Name, 1);
                subItems = new ListViewItem.ListViewSubItem[]
                    { new ListViewItem.ListViewSubItem(item, "File"),
             new ListViewItem.ListViewSubItem(item,
                file.LastAccessTime.ToShortDateString())};

                item.SubItems.AddRange(subItems);
                listView1.Items.Add(item);
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private CancellationTokenSource _canceller;
        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;

            _canceller = new CancellationTokenSource();
            await Task.Run(() =>
            {
                do
                {
                    // Code here jase
                    if (path == "")
                    {
                        listBox1.Items.Add("Directory could not be found!");
                    }

                    using (FileSystemWatcher watcher = new FileSystemWatcher())
                    {
                        watcher.Path = @"" + path + "";

                        watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

                        watcher.Filter = "*.*";

                        watcher.Changed += OnChanged;
                        watcher.Created += Oncreate;
                        watcher.Deleted += OnDelete;
                        watcher.Renamed += OnRenamed;

                        watcher.EnableRaisingEvents = true;
                    }

                    if (_canceller.Token.IsCancellationRequested)
                        break;
                } while (true);
            });

            _canceller.Dispose();
            button1.Enabled = true;
            button2.Enabled = false;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            _canceller.Cancel();
        }

        // Define the event handlers.
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            Form1 f1 = new Form1();
            f1.listBox1.Items.Add("Changed");
        }


        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            Form1 f1 = new Form1();
            f1.listBox1.Items.Add("Renamed");
        }


        private static void OnDelete(object source, FileSystemEventArgs e)
        {
            Form1 f1 = new Form1();
            f1.listBox1.Items.Add("Delete");
        }

        private static void Oncreate(object source, FileSystemEventArgs e)
        {
            Form1 f1 = new Form1();
            f1.listBox1.Items.Add("Create");
        }

    }
}
