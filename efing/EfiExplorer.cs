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
    public partial class EfiExplorer : Form {
        //const string UNIX_ROOT = "//boot/efi/";
        const string WIN_ROOT = "S:\\";
        const string UNIX_ROOT = "/home/bruce/boot/efi/";
        //const string WIN_ROOT = "C:\\Users\\bruce\\boot\\efi\\";
        const string LOADING = "Loading...";
        const string FILTER = "Conf Files (*.conf)|*.conf|All Files (*.*)|*.*";

        public string efiRoot = "";

        private int childFormNumber = 0;

        public EfiExplorer() {
            InitializeComponent();
        }

        private void ShowNewForm(object sender, EventArgs e) {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "Window " + childFormNumber++;
            childForm.Show();
        }

        private void OpenFile(object sender, EventArgs e) {
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            //if (openFileDialog.ShowDialog(this) == DialogResult.OK) {
            //    string FileName = openFileDialog.FileName;
            //}
            /**
             * Either we don't have access or the folder isn't mounted
             * In either case, allow the user to select an alternate. 
             */

            var dialog = new FolderBrowserDialog();
            dialog.Description = "Select the EFI root folder";
            dialog.ShowNewFolderButton = false;
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            if (dialog.ShowDialog(this) == DialogResult.OK) {
                efiRoot = dialog.SelectedPath;
            }

            try {
                ListDirectory(this.treeView, efiRoot);
            } catch (Exception ex) {

            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK) {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e) {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e) {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e) {
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e) {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e) {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e) {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e) {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e) {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e) {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e) {
            foreach (Form childForm in MdiChildren) {
                childForm.Close();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            var aboutBox = new AboutBox();
            aboutBox.Show();

        }

        private void EfiExplorer_Load(object sender, EventArgs e) {
            efiRoot = Environment.OSVersion.Platform == PlatformID.Unix
                ? UNIX_ROOT
                : WIN_ROOT;

            try {
                ListDirectory(this.treeView, efiRoot);
            } catch (Exception ex) {

            }

        }

        private void ListDirectory(TreeView treeView, string path) {
            treeView.Nodes.Clear();
            var rootDirectoryInfo = new DirectoryInfo(path);
            treeView.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
            treeView.Nodes[0].Expand();

        }

        private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo) {
            var directoryNode = new TreeNode(directoryInfo.Name);
            foreach (var directory in directoryInfo.GetDirectories()) {
                var node = directoryNode.Nodes.Add(CreateDirectoryNode(directory));
                if (directory.Name == "EFI") {
                    directoryNode.Nodes[node].Expand();
                }
            }
            foreach (var file in directoryInfo.GetFiles()) {
                var node = new TreeNode(file.Name);
                node.Tag = file.FullName;
                directoryNode.Nodes.Add(node);
            }
            return directoryNode;
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e) {
            if (e.Node.Tag != null) {
                showFileContents(e.Node.Tag.ToString());
            }
        }

        private void showFileContents(string filename) {

            /* Check if we've already opned this file */
            foreach (var childForm in MdiChildren) {
                if (childForm.Text == filename) {
                    childForm.Activate();
                    return;
                }
            }

            /* Load the file into the correct form */
            toolStripStatusLabel.Text = filename;
            switch (Path.GetFileName(filename)) {
                case "grub.cfg":
                    Documents.Grub_cfg childForm1 = new Documents.Grub_cfg();
                    childForm1.MdiParent = this;
                    childForm1.Text = filename;
                    childForm1.WindowState = FormWindowState.Maximized;
                    childForm1.FormBorderStyle = FormBorderStyle.FixedSingle;
                    childForm1.SetText(System.IO.File.ReadAllText(filename));
                    childForm1.SetTitle(filename);
                    childForm1.Show();
                    break;
                case "refind.conf":
                    Documents.Refind_conf childForm2 = new Documents.Refind_conf();
                    childForm2.MdiParent = this;
                    childForm2.Text = filename;
                    childForm2.WindowState = FormWindowState.Maximized;
                    childForm2.FormBorderStyle = FormBorderStyle.FixedSingle;
                    childForm2.SetText(System.IO.File.ReadAllText(filename));
                    childForm2.SetTitle(filename);
                    childForm2.Show();
                    break;
                case "refind_linux.conf":
                    Documents.Refind_linux_conf childForm3 = new Documents.Refind_linux_conf();
                    childForm3.MdiParent = this;
                    childForm3.Text = filename;
                    childForm3.FormBorderStyle = FormBorderStyle.FixedSingle;
                    childForm3.FormBorderStyle = FormBorderStyle.FixedSingle;
                    childForm3.SetText(System.IO.File.ReadAllText(filename));
                    childForm3.SetTitle(filename);
                    childForm3.Show();
                    break;
                case "startup.nsh":
                    Documents.Startup_nsh childForm4 = new Documents.Startup_nsh();
                    childForm4.MdiParent = this;
                    childForm4.Text = filename;
                    childForm4.FormBorderStyle = FormBorderStyle.FixedSingle;
                    childForm4.FormBorderStyle = FormBorderStyle.FixedSingle;
                    childForm4.SetText(System.IO.File.ReadAllText(filename));
                    childForm4.SetTitle(filename);
                    childForm4.Show();
                    break;
                case "theme.cfg":
                    Documents.Theme_cfg childForm5 = new Documents.Theme_cfg();
                    childForm5.MdiParent = this;
                    childForm5.Text = filename;
                    childForm5.FormBorderStyle = FormBorderStyle.FixedSingle;
                    childForm5.FormBorderStyle = FormBorderStyle.FixedSingle;
                    childForm5.SetText(System.IO.File.ReadAllText(filename));
                    childForm5.SetTitle(filename);
                    childForm5.Show();
                    break;
                default:
                    //				fileTextBox.Text = "";
                    break;
            }
        }

    }
}
