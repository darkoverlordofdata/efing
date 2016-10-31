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

namespace efing {

		
	
    public partial class MainForm : Form {

        //const string UNIX_ROOT = "//boot/efi/";
        //const string WIN_ROOT = "S:\\";
        const string UNIX_ROOT = "/home/bruce/boot/efi/";
		const string WIN_ROOT = "C:\\Users\\bruce\\boot\\efi\\";
		const string LOADING = "Loading...";

        public MainForm() {
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

        private void addAllFolders(TreeNode TNode, string FolderPath) {

            try {

                foreach (var FolderNode in Directory.GetDirectories(FolderPath)) {
					
					var folder = FolderNode.Substring(FolderNode.LastIndexOf(Path.PathSeparator) + 1);
					var displayName = folder.Replace(UNIX_ROOT, "").Replace(WIN_ROOT, "");
					var subFolderNode = TNode.Nodes.Add(displayName); 

					subFolderNode.Tag = FolderNode;
					subFolderNode.Nodes.Add(LOADING);

					if (displayName == "EFI") subFolderNode.Expand();
                }

            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }

        }

		private void showFileContents(string filename) {
			switch (Path.GetFileName(filename)) {
			case "grub.cfg":
				fileTextBox.Text = System.IO.File.ReadAllText(filename);
				break;
			case "refind.conf":
				fileTextBox.Text = System.IO.File.ReadAllText(filename);
				break;
			case "refind_linux.conf":
				fileTextBox.Text = System.IO.File.ReadAllText(filename);
				break;
			case "startup.nsh":
				fileTextBox.Text = System.IO.File.ReadAllText(filename);
				break;
			case "theme.cfg":
				fileTextBox.Text = System.IO.File.ReadAllText(filename);
				break;
			default:
				fileTextBox.Text = "";
				break;
			}
		}

        private void folderTreeView_AfterSelect(object sender, TreeViewEventArgs e) {

			var fileExtension = "";
            var subItemIndex = 0;
            var dateMod = "";

            fileListView.Items.Clear();


            if (folderTreeView.SelectedNode.Nodes.Count == 1 
				&& folderTreeView.SelectedNode.Nodes[0].Text == LOADING) {

                folderTreeView.SelectedNode.Nodes.Clear();
                addAllFolders(folderTreeView.SelectedNode, Convert.ToString(folderTreeView.SelectedNode.Tag));
            }

            var folder = Convert.ToString(folderTreeView.SelectedNode.Tag); //Folder Name

            if ((folder != null) && System.IO.Directory.Exists(folder)) {

                try {

                    foreach (var file in System.IO.Directory.GetFiles(folder)) {

                        fileExtension = System.IO.Path.GetExtension(file); 
                        dateMod = System.IO.File.GetLastWriteTime(file).ToString(); 

                        //AddImages(file); //Add File Icons
						fileListView.Items.Add(file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1), file.ToString()); //Add Files & File Properties To ListView
						fileListView.Items[subItemIndex].SubItems.Add(fileExtension.ToString() + " File");
						fileListView.Items[subItemIndex].SubItems.Add(dateMod.ToString());
						fileListView.Items[subItemIndex].Tag = file;

                        subItemIndex += 1;

                    }

                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }

            }

        }


        private void folderTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e) {

            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "Loading...") {
                e.Node.Nodes.Clear();
                addAllFolders(e.Node, Convert.ToString(e.Node.Tag)); //Add All Folders

            }

        }

        private void mainForm_Load(object sender, EventArgs e) {

            var Tnode = folderTreeView.Nodes.Add("System"); //Add Main Node

			addAllFolders(Tnode, Environment.OSVersion.Platform == PlatformID.Unix
				? UNIX_ROOT
				: WIN_ROOT
			);

			Tnode.Expand();
            fileListView.View = View.Details; //Set ListView View Option

            fileListView.Columns.Add("File Name", 150, HorizontalAlignment.Left);
            fileListView.Columns.Add("File Type", 80, HorizontalAlignment.Left);
            fileListView.Columns.Add("Date Modified", 150, HorizontalAlignment.Left);

        }

        private void fileListView_SelectedIndexChanged(object sender, EventArgs e) {

			if (fileListView.SelectedItems.Count == 1) {
				var item = fileListView.SelectedItems[0];
				toolStripStatusLabel1.Text = item.Tag.ToString();
				showFileContents(item.Tag.ToString());
				//MessageBox.Show(item.ToString());
			} else {
                //toolStripStatusLabel1.Text = "Found "+fileListView.SelectedItems.Count+" items";
			}
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
