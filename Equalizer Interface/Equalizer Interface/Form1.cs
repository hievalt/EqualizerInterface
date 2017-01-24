using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Equalizer_Interface
{
    public partial class Equalizer : Form
    {
        public Equalizer()
        {
            InitializeComponent();
        }




        // WRITE SETTING FILE
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (fileName.Text != "" && Directory.Exists(setSavePath.Text))
            {
                var filters = new List<string> { };
                for (int i = 1; i < 21; i++)
                {
                    Control l = this.Controls.Find("filter" + i.ToString() + "db", true).Single();
                    Control textb = this.Controls.Find("filter" + i.ToString() + "freq", true).Single();
                    string line = "Filter "+ i + ": ON PK Fc " + (textb as TextBox).Text + " Hz Gain " + (l as Label).Text + " dB Q 1,00" + Environment.NewLine;
                    filters.Add(line);
                }

                var filterValues = new List<string> { };
                for (int i = 1; i < 21; i++)
                {
                    Control l = this.Controls.Find("filter" + i.ToString() + "db", true).Single();
                    Control textb = this.Controls.Find("filter" + i.ToString() + "freq", true).Single();
                    string line = (l as Label).Text + Environment.NewLine;
                    filterValues.Add(line);
                    line = (textb as TextBox).Text + Environment.NewLine;
                    filterValues.Add(line);
                }

                string filePath = setSavePath.Text + fileName.Text + ".txt";
                string filtersPath = setSavePath.Text + fileName.Text + ".txt.filters";
                string configPath = setSavePath.Text + "config.txt";

                using (StreamWriter outputFile = new StreamWriter(filePath, false))
                {
                    outputFile.Write("Filter Settings File" + Environment.NewLine + "Equaliser: Generic" +
                        Environment.NewLine + Environment.NewLine);
                    foreach (string filter in filters)
                        outputFile.Write(filter);
                }

                using (StreamWriter outputFile = new StreamWriter(filtersPath, false))
                {
                    foreach (string filterValue in filterValues)
                        outputFile.Write(filterValue);

                }
                if (editConfig.Checked == true)
                {
                    string[] lines = File.ReadAllLines(setSavePath.Text + "config.txt");
                    lines[1] = "Include: " + fileName.Text + ".txt";
                    File.WriteAllLines(setSavePath.Text + "config.txt", lines);
                }

                
            }
            else { MessageBox.Show("Setting name missing or save directory does not exist", "Error"); }
        }

        private void resetCurrentSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetSetting();
        }

        private void ResetSetting()
        {
            for (int i = 1; i < 21; i++)
            {
                Control l = this.Controls.Find("filter" + i.ToString() + "db", true).Single();
                Control tbar = this.Controls.Find("filter" + i.ToString() + "bar", true).Single();
                (l as Label).Text = "0";
                (tbar as TrackBar).Value = 0;
            }
            fileName.Text = "";
            filter1freq.Text = "50";
            filter2freq.Text = "66";
            filter3freq.Text = "110";
            filter4freq.Text = "130";
            filter5freq.Text = "156";
            filter6freq.Text = "220";
            filter7freq.Text = "320";
            filter8freq.Text = "440";
            filter9freq.Text = "615";
            filter10freq.Text = "880";
            filter11freq.Text = "1250";
            filter12freq.Text = "1750";
            filter13freq.Text = "2500";
            filter14freq.Text = "3500";
            filter15freq.Text = "5000";
            filter16freq.Text = "7500";
            filter17freq.Text = "10000";
            filter18freq.Text = "12000";
            filter19freq.Text = "15000";
            filter20freq.Text = "20000";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void setPreamp_TextChanged(object sender, EventArgs e)
        {
            string[] lines = File.ReadAllLines(setSavePath.Text + "config.txt");
            lines[0] = "Preamp: " + setPreamp.Text + " dB";
            File.WriteAllLines(setSavePath.Text + "config.txt", lines);
        }

        private void Equalizer_Load(object sender, EventArgs e)
        {
            setPreamp.Text = Properties.Settings.Default.preampValue;
            setSavePath.Text = Properties.Settings.Default.savePathValue;
            RefreshSavedSettings();
        }

        private void RefreshSavedSettings()
        {
            savedSettings.Items.Clear();
            string loadFrom = setSavePath.Text;
            if (Directory.Exists(loadFrom))
            {
                string[] filePaths = Directory.GetFiles(loadFrom, "*.txt");

                foreach (string file in filePaths)
                {
                    if (File.Exists(file + ".filters"))
                    {
                        savedSettings.Items.Add(Path.GetFileNameWithoutExtension(file));
                    }
                }
            }
        }

        private void Equalizer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.preampValue = setPreamp.Text;
            Properties.Settings.Default.savePathValue = setSavePath.Text;
            Properties.Settings.Default.Save();
        }

        private void setSavePath_Leave(object sender, EventArgs e)
        {
            if (!File.Exists(setSavePath.Text + "config.txt"))
            {
                MessageBox.Show("Can't find config.txt in given path!", "Check file path");
            }
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            fileName.Text = savedSettings.Text;
            if (File.Exists(setSavePath.Text + savedSettings.Text + ".txt.filters"))
            {
                using (StreamReader loadedFile = new StreamReader(setSavePath.Text + savedSettings.Text + ".txt.filters"))
                {
                    // LOAD FILTER VALUES
                    for (int i = 1; i < 21; i++)
                    {
                        string line;
                        Control l = this.Controls.Find("filter" + i.ToString() + "db", true).Single();
                        Control tbar = this.Controls.Find("filter" + i.ToString() + "bar", true).Single();
                        Control textb = this.Controls.Find("filter" + i.ToString() + "freq", true).Single();

                        line = loadedFile.ReadLine();
                        (l as Label).Text = line;
                        (tbar as TrackBar).Value = Convert.ToInt32(Convert.ToDouble((l as Label).Text) * 2);
                        line = loadedFile.ReadLine();
                        (textb as TextBox).Text = line;
                    }
                }
            }
            else
            {
                MessageBox.Show("Setting not found", "Error");
            }
        }

        //
        private void deleteCurrentSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(setSavePath.Text + fileName.Text + ".txt"))
            {
                File.Delete(setSavePath.Text + fileName.Text + ".txt");
                File.Delete(setSavePath.Text + fileName.Text + ".txt.filters");
                MessageBox.Show(fileName.Text + " deleted.", "Setting deleted");
                ResetSetting();
                RefreshSavedSettings();
            }
            else { MessageBox.Show("Setting file was not found. Nothing was deleted.", "File not found"); }
        }

        private void savedSettings_Click(object sender, EventArgs e)
        {
            RefreshSavedSettings();
        }

        // Päivittää määrän tekstinä
        private void filter1bar_Scroll(object sender, EventArgs e)
        {
            filter1db.Text = (filter1bar.Value * 0.50).ToString();
        }

        private void filter2bar_Scroll(object sender, EventArgs e)
        {
            filter2db.Text = (filter2bar.Value * 0.50).ToString();
        }

        private void filter3bar_Scroll(object sender, EventArgs e)
        {
            filter3db.Text = (filter3bar.Value * 0.50).ToString();
        }

        private void filter4bar_Scroll(object sender, EventArgs e)
        {
            filter4db.Text = (filter4bar.Value * 0.50).ToString();
        }

        private void filter5bar_Scroll(object sender, EventArgs e)
        {
            filter5db.Text = (filter5bar.Value * 0.50).ToString();
        }

        private void filter6bar_Scroll(object sender, EventArgs e)
        {
            filter6db.Text = (filter6bar.Value * 0.50).ToString();
        }

        private void filter7bar_Scroll(object sender, EventArgs e)
        {
            filter7db.Text = (filter7bar.Value * 0.50).ToString();
        }

        private void filter8bar_Scroll(object sender, EventArgs e)
        {
            filter8db.Text = (filter8bar.Value * 0.50).ToString();
        }

        private void filter9bar_Scroll(object sender, EventArgs e)
        {
            filter9db.Text = (filter9bar.Value * 0.50).ToString();
        }

        private void filter10bar_Scroll(object sender, EventArgs e)
        {
            filter10db.Text = (filter10bar.Value * 0.50).ToString();
        }

        private void filter11bar_Scroll(object sender, EventArgs e)
        {
            filter11db.Text = (filter11bar.Value * 0.50).ToString();
        }

        private void filter12bar_Scroll(object sender, EventArgs e)
        {
            filter12db.Text = (filter12bar.Value * 0.50).ToString();
        }

        private void filter13bar_Scroll(object sender, EventArgs e)
        {
            filter13db.Text = (filter13bar.Value * 0.50).ToString();
        }

        private void filter14bar_Scroll(object sender, EventArgs e)
        {
            filter14db.Text = (filter14bar.Value * 0.50).ToString();
        }

        private void filter15bar_Scroll(object sender, EventArgs e)
        {
            filter15db.Text = (filter15bar.Value * 0.50).ToString();
        }

        private void filter16bar_Scroll(object sender, EventArgs e)
        {
            filter16db.Text = (filter16bar.Value * 0.50).ToString();
        }

        private void filter17bar_Scroll(object sender, EventArgs e)
        {
            filter17db.Text = (filter17bar.Value * 0.50).ToString();
        }

        private void filter18bar_Scroll(object sender, EventArgs e)
        {
            filter18db.Text = (filter18bar.Value * 0.50).ToString();
        }

        private void filter19bar_Scroll(object sender, EventArgs e)
        {
            filter19db.Text = (filter19bar.Value * 0.50).ToString();
        }

        private void filter20bar_Scroll(object sender, EventArgs e)
        {
            filter20db.Text = (filter20bar.Value * 0.50).ToString();
        }

    }
}
