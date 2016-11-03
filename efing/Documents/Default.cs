using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace efing.Documents {
    public partial class Default : Form {

        public Default() {
            InitializeComponent();
        }

        public void SetText(string text) {
			this.richTextBox.Text = text;
            for (int i = 0; i < richTextBox.Lines.Count(); i++)
                highlightLineContaining(richTextBox, i, "#", Color.Green);
        }

        public void SetTitle(string text) {
            this.tabPage1.Text = text;
        }

        public void SetData(DataTable data) {
            this.dataGridView.DataSource = data;
            this.dataGridView.Visible = true;

            this.dataGridView.Columns[2].Width = 300;
        }

        public void SetDescription(string text) {
            this.description.Text = text;
        }

        void highlightLineContaining(RichTextBox rtb, int line, string search, Color color) {
            int c0 = rtb.GetFirstCharIndexFromLine(line);
            int c1 = rtb.GetFirstCharIndexFromLine(line + 1);
            if (c1 < 0) c1 = rtb.Text.Length;
            rtb.SelectionStart = c0;
            rtb.SelectionLength = c1 - c0;
            if (rtb.SelectedText.Contains(search))
                rtb.SelectionColor = color;
            rtb.SelectionLength = 0;
        }

        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e) {

        }
    }

}
