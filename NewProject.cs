using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace ZIDE
{
    public partial class NewProject : Form
    {
        public delegate void ProjectLoad();
        public event ProjectLoad ProjectLoaded;
        public NewProject()
        {
            InitializeComponent();
        }
        protected virtual void ProjLoaded()
        {
            ProjectLoaded?.Invoke();
        }
        string defaultPath
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ZIDE\\Projects"); }         
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void CreateProject(string path, string type)
        {
            if(type == "C#")
            {
                Directory.CreateDirectory(path + "\\" + textBox2.Text);
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.CreateNoWindow = true;
                startInfo.FileName = "cmd.exe";
                startInfo.WorkingDirectory = path+"\\"+textBox2.Text;
                startInfo.Arguments = "/c dotnet new console";
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                ZIDESettings.currentOpenDir = startInfo.WorkingDirectory;
                ZIDESettings.crProjN = textBox2.Text;
                File.WriteAllText(path + "\\" + textBox2.Text+"\\projdetails.zicf", textBox2.Text);
                ProjLoaded();
                this.Close();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string path = defaultPath;
            if (textBox1.Text != String.Empty)
            {
                if (Directory.Exists(textBox1.Text))
                    path = textBox1.Text;
                else
                    MessageBox.Show("ZIDE Error: Such Path does not exist. 002");
                CreateProject(path, comboBox1.Text);
            }
            else
            {
                CreateProject(path, comboBox1.Text);
            }
        }
    }
}
