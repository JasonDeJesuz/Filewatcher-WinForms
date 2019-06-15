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
using System.Windows.Threading;
using Tulpep.NotificationWindow;

namespace Filewatcher_WinForms
{
    public partial class Form1 : Form
    {
        public string path;
        public string projectpath = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()));

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
            //DriveInfo drive = new DriveInfo(@"C:\");
            //DirectoryInfo info = drive.RootDirectory;
            DirectoryInfo info = new DirectoryInfo(@"../../../../");        // Looks at the whole desktop
            
            if (info.Exists)
            {
                rootNode = new TreeNode(info.Name);
                rootNode.Tag = info;
                GetDirectories(info.GetDirectories(), rootNode);
                treeView1.Nodes.Add(rootNode);
            }
        }

        private void GetDirectories(DirectoryInfo[] subDirs, TreeNode nodeToAddTo)
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

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Monitoring";
            textBox1.ForeColor = Color.Green;
            StartFileSystemWatcher();
        }

        private void Button2_Click_1(object sender, EventArgs e)
        {
            textBox1.Text = "Not monitoring";
            textBox1.ForeColor = Color.Red;
            EndFileSystemWatcher();
        }
        private void Button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
        private void StartFileSystemWatcher()
        {
            //listBox1.Items.Add("Start");
            string folderPath = path;

            if (string.IsNullOrWhiteSpace(folderPath))
                return;

            

            fileSystemWatcher.Path = folderPath;

            fileSystemWatcher.IncludeSubdirectories = true;

            fileSystemWatcher.NotifyFilter = NotifyFilters.FileName |
                NotifyFilters.LastWrite |
                NotifyFilters.Size |
                NotifyFilters.DirectoryName;


            // Event handlers that are watching for specific event
            fileSystemWatcher.Created += new FileSystemEventHandler(OnCreate);
            fileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
            fileSystemWatcher.Deleted += new FileSystemEventHandler(OnDelete);
            fileSystemWatcher.Renamed += new RenamedEventHandler(OnRenamed);

            // NOTE: If you want to monitor specified files in folder, you can use this filter
            // fileSystemWatcher.Filter

            // START watching
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        private void EndFileSystemWatcher()
        {
            fileSystemWatcher.EnableRaisingEvents = false;
            //listBox1.Items.Add("End");
        }

        // Define the event handlers.
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            DisplayFileSystemWatcherInfo(e.ChangeType, e.Name);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            DisplayFileSystemWatcherInfo(e.ChangeType, e.Name, e.OldName);
        }


        private void OnDelete(object sender, FileSystemEventArgs e)
        {
            DisplayFileSystemWatcherInfo(e.ChangeType, e.Name);
        }

        private void OnCreate(object sender, FileSystemEventArgs e)
        {
            DisplayFileSystemWatcherInfo(e.ChangeType, e.Name);
        }

        private void DisplayFileSystemWatcherInfo(WatcherChangeTypes watcherChangeTypes, string name, string oldName = null)
        {
            if (watcherChangeTypes == WatcherChangeTypes.Renamed)
            {
                BeginInvoke(new Action(() => { AddListLine(string.Format("{0} -> {1} to {2} - {3}", watcherChangeTypes.ToString(), oldName, name, DateTime.Now), watcherChangeTypes.ToString()); }));
            }
            else
            {
                BeginInvoke(new Action(() => { AddListLine(string.Format("{0} -> {1} - {2}", watcherChangeTypes.ToString(), name, DateTime.Now), watcherChangeTypes.ToString()); }));
            }
        }
        public void AddListLine(string text, string type)
        {
            this.listBox1.Items.Add(text);
            this.listBox1.Items.Add("------------------------------");
            PopupNotifier pop = new PopupNotifier();
            pop.TitleText = "A file has been " + type;
            pop.ContentText = text;
            pop.Image = Image.FromFile(projectpath + @"\assets\infoinfo.png");
            pop.ImageSize = new Size(100, 100);
            pop.Popup();
        }
    }
}
