using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace efing.Documents {
    struct Config {
        public string[] desc;
        public bool useDefault;
        public string name;
        public string value;
        public string[] options;

        public Config(string[] desc, bool useDefault, string name, string value, string[] options) {
            this.desc = desc;
            this.useDefault = useDefault;
            this.name = name;
            this.value = value;
            this.options = options;
        }
    }

    public partial class Refind_conf : efing.Documents.Default {
        // private int current = 0;
        // private int max = 100;
        private Config[] config;
         
        public Refind_conf() {
            InitializeComponent();
        }
        public new void SetText(string text) {
            base.SetText(text);
            ParseConfig(text);

        }

        /**
         * ParseConfig
         * 
         * @param string text
         * 
         * Config blocks are seperated by an empty line.
         * Description lines start with '# '
         * Name/Value pairs can be preceded with a single '#' to use the default value:
         * 
         * # this is a description
         * # this is another line of description
         * name value
         * #name value
         * 
         */
        void ParseConfig(string text) {
            var chunk = Regex.Split(text, "\n\n");
            var max = chunk.Length - 1;
            var current = 0;

            config = new Config[max];
            for (var l=1; l<chunk.Length; l++) {
                var lines = chunk[l];
                if (lines.IndexOf("menuentry") == -1)
                    config[current++] = ParseChunk(lines);

            }

            var table = new DataTable("config");
            table.Columns.Add("enable", typeof(bool));
            table.Columns.Add("name", typeof(string));
            table.Columns.Add("value", typeof(string));
            object[] data = new object[3];

            foreach (Config c in config) {
                data[0] = c.useDefault;
                data[1] = c.name;
                data[2] = c.value;
                table.Rows.Add(data);
            }

            this.SetData(table, (DataGridView dataGridView) => {
                dataGridView.Visible = true;
                dataGridView.Columns[1].ReadOnly = true;
                dataGridView.Columns[2].Width = 300;
            });

        }

        Config ParseChunk(string text) {
            char[] SP = { ' ' };
            char[] LF = { '\n' };
            var lines = text.Split(LF);

            int descIndex = 0;
            int optionIndex = 0;
            int descSize = 0;
            int optionSize = 0;
            string name = "";
            string value = "";
            bool useDefault = false;

            for (int l = 0; l < lines.Length; l++) {
                if (lines[l] == "#") {
                    descSize++;
                } else if (lines[l].StartsWith("# ")) {
                    descSize++;
                } else optionSize++;
            }
            string[] desc = new string[descSize];
            string[] option = new string[optionSize];

            for (int l = 0; l < lines.Length; l++) {
                if (lines[l] == "#") {
                    desc[descIndex++] = "";
                } else if (lines[l].StartsWith("# ")) {
                    desc[descIndex++] = lines[l].Substring(2);
                } else {
                    if (lines[l].StartsWith("#")) {
                        name = lines[l].Substring(1).Split(SP, 2)[0];
                        if (lines[l].Substring(1).Split(SP, 2).Length > 1) {
                            value = lines[l].Substring(1).Split(SP, 2)[1];
                            option[optionIndex++] = lines[l].Substring(1).Split(SP, 2)[1];
                        }
                    } else {
                        if (lines[l] != "") {
                            name = lines[l].Substring(1).Split(SP, 2)[0];
                            if (lines[l].Substring(1).Split(SP, 2).Length > 1) {
                                value = lines[l].Substring(1).Split(SP, 2)[1];
                            }
                            useDefault = true;
                        }
                    }
                }
            }
            return new Config(desc, useDefault, name, value, option);
        }


    }
}
