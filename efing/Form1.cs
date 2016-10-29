using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO; //File Operations

namespace efing {
	
    public partial class mainForm : Form {
		
        public mainForm() {
            InitializeComponent();
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e) {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            AboutBox aboutBox = new AboutBox();
            aboutBox.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void AddAllFolders(TreeNode TNode, string FolderPath) {

            try {

                foreach (string FolderNode in Directory.GetDirectories(FolderPath)) { //Load All Sub Folders

                    TreeNode SubFolderNode = TNode.Nodes.Add(FolderNode.Substring(FolderNode.LastIndexOf(Path.PathSeparator) + 1)); //Add Each Sub Folder Name

                    SubFolderNode.Tag = FolderNode; //Set Tag For Each Sub Folder

                    SubFolderNode.Nodes.Add("Loading...");

                }

            }

            catch (Exception ex) {//Something Went Wrong

                MessageBox.Show(ex.Message);

            }

        }

        private void folderTreeView_AfterSelect(object sender, TreeViewEventArgs e) {

            string FileExtension = null; //Stores File Extension

            int SubItemIndex = 0; //Sub Item Counter

            string DateMod = null; //Stores Date Modified Of File

            fileListView.Items.Clear(); //Clear Existing Items


            if (folderTreeView.SelectedNode.Nodes.Count == 1 && folderTreeView.SelectedNode.Nodes[0].Text == "Loading...") {

                folderTreeView.SelectedNode.Nodes.Clear(); //Reset

                AddAllFolders(folderTreeView.SelectedNode, Convert.ToString(folderTreeView.SelectedNode.Tag));

            }

            string folder = Convert.ToString(folderTreeView.SelectedNode.Tag); //Folder Name

            if ((folder != null) && System.IO.Directory.Exists(folder)) {

                try {

                    foreach (string file in System.IO.Directory.GetFiles(folder)) {

                        FileExtension = System.IO.Path.GetExtension(file); //Get File Extension(s)

                        DateMod = System.IO.File.GetLastWriteTime(file).ToString(); //Get Date Modified For Each File

                        //AddImages(file); //Add File Icons
						fileListView.Items.Add(file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1), file.ToString()); //Add Files & File Properties To ListView
						fileListView.Items[SubItemIndex].SubItems.Add(FileExtension.ToString() + " File");
						fileListView.Items[SubItemIndex].SubItems.Add(DateMod.ToString());

                        SubItemIndex += 1;

                    }

                }

                catch (Exception ex) {

                    MessageBox.Show(ex.Message); //Something Went Wrong
					 
                }

            }

        }


        private void folderTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e) {

            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "Loading...") {
                e.Node.Nodes.Clear(); //Clear All Items

                AddAllFolders(e.Node, Convert.ToString(e.Node.Tag)); //Add All Folders

            }

        }

        private void mainForm_Load(object sender, EventArgs e) {


            folderTreeView.Sort(); //Sort Alphabetically

            TreeNode Tnode = folderTreeView.Nodes.Add("efi"); //Add Main Node

			if (Environment.OSVersion.Platform == PlatformID.Unix) {
                AddAllFolders(Tnode, "/boot/efi/EFI/"); //Add mounted share
            }
            else {
                AddAllFolders(Tnode, "S:\\"); //Add mounted drive
            }

            fileListView.View = View.Details; //Set ListView View Option

            //Add ListView Columns With Specified Width
            fileListView.Columns.Add("File Name", 150, HorizontalAlignment.Left);
            fileListView.Columns.Add("File Type", 80, HorizontalAlignment.Left);
            fileListView.Columns.Add("Date Modified", 150, HorizontalAlignment.Left);

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e) {

        }
        /**
private void copyToolStripMenuItem_Click(object sender, EventArgs e) {

    ListView.SelectedListViewItemCollection lvSel = this.fileListView.SelectedItems; //ListViewItems

    string strFileName = null; //File Name

    foreach (ListViewItem lvItem in lvSel) {

        strFileName = folderTreeView.SelectedNode.Tag + Path.DirectorySeparatorChar.ToString() + lvItem.Text; //Get Selected Filename

        DataObject clpDataObj = new DataObject(); //Create New DataObject

        string[] cbClipBoardFile = new string[1]; //Break File Apart Into A String Array

        cbClipBoardFile[0] = strFileName;

        clpDataObj.SetData(DataFormats.FileDrop, true, cbClipBoardFile); //Put String Array Onto ClipBoard

        Clipboard.SetDataObject(clpDataObj);

        MessageBox.Show("File Copied To Clipboard"); //Inform The User

    }

}

private void pasteToolStripMenuItem_Click(object sender, EventArgs e) {

    IDataObject idClipboardData = Clipboard.GetDataObject(); //Get Data Present on ClipBoard

    if (idClipboardData.GetDataPresent(DataFormats.FileDrop)) {

        string[] strClipFile = (string[])idClipboardData.GetData(DataFormats.FileDrop); //Convert String Array Back Into File

        int i = 0;


        for (i = 0; i <= strClipFile.Length - 1; i++) {
            //If File Exists, Rename COpied File
            if (File.Exists(folderTreeView.SelectedNode.Tag + Path.DirectorySeparatorChar.ToString() + Path.GetFileName(strClipFile[i]))) {

                File.Move(folderTreeView.SelectedNode.Tag + Path.DirectorySeparatorChar.ToString() + Path.GetFileName(strClipFile[i]), folderTreeView.SelectedNode.Tag + "temp");

            }

            //Copy File From ClipbBoard
            File.Copy(strClipFile[i], folderTreeView.SelectedNode.Tag + Path.DirectorySeparatorChar.ToString() + Path.GetFileName(strClipFile[i]));

        }

        MessageBox.Show("File Pasted From Clipboard"); //Inform User

    }

}

/**
private void cboView_SelectedIndexChanged(object sender, EventArgs e) {

    string strSelView = Convert.ToString(cboView.SelectedItem); //Change ListView ViewMode

    switch (strSelView) {

        case "Large Icon":

            lvFiles.View = View.LargeIcon;

            break;

        case "Details":

            lvFiles.View = View.Details;

            break;

        case "Small Icon":

            lvFiles.View = View.SmallIcon;

            break;

        case "List":

            lvFiles.View = View.List;

            break;

        case "Tile":

            lvFiles.View = View.Tile;

            break;

    }

}*/


    }
}
