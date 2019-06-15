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

        //our popuatetreeview method will specify where the application will start populating the treeview from, in this case we used desktop.
        //Due to rights and complications we could not get the C drive to be watched as we can see in the commented lines.
        //Beacause we are using treeviews we will need to associate nodes, we use nodes to get the directories and know where the application should look at and which files to use.
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

        //This is a method we use accross all of the other 2 methods that we use, this will serve as a shared method for getting our main directories.
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

        //when the user clicks on the treeview items we initialize this method, this method will make sure we receive all of the subdirectories and the files inside of the directory
        //when the program receives them it adds each subdirectory folder to the listview as well as the files after that.
        //in the designer we created the columns Name, Type and last Modified - in the method we get the Name, Type and Last Modified from each of the files and then include it in the application
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

        //The StartFileSystemWatcher method is started when the Start Monitoring button is clicked, the method will use the path defined by the nodemouseclick event.
        //When the path is empty we simple return out of the thread, this will cause the monitoring system to not run the thread, yet we know that the directories are always displayed to the user thus
        //we can confirm that there will always be directories that exist.
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


            // All event handlers we will be monitoring during the runtime
            fileSystemWatcher.Created += new FileSystemEventHandler(OnCreate);
            fileSystemWatcher.Changed += new FileSystemEventHandler(OnChanged);
            fileSystemWatcher.Deleted += new FileSystemEventHandler(OnDelete);
            fileSystemWatcher.Renamed += new RenamedEventHandler(OnRenamed);

            //Starting our filesystemwatcher and have it run in the background.
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

        //Because we are using different threads for the back-end filesystemwatcher we will not be able to just add the text to the listbox, we will have to use something like
        //BeginInvoke in order to call a method to add the text to the listbox.
        //Below we  use the BeginInvoke to send an action to a method to add the text to the listbox and display the popup.
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
        // Disolaying the text inside of the listbox and show the popup
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
